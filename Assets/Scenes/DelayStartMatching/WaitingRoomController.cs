using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public class WaitingRoomController : MonoBehaviourPunCallbacks
{
    private PhotonView myPhotonView;

    [SerializeField]
    private int multiplayerSceneIndex;
    [SerializeField]
    private int menuSceneIndex;

    private int playerCount;
    private int roomSize;
    [SerializeField]
    private int minPlayersToStart = 2;

    [SerializeField]private Text playerCountDisplay;
    [SerializeField]private Text timerToStartDisplay;

    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;

    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    [SerializeField] private float maxWaitTime;
    [SerializeField] private float maxFullGameWaitTime;
    // Start is called before the first frame update
    void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;
        
        PlayerCountUpdate();
    }

    private void PlayerCountUpdate(){
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        playerCountDisplay.text = playerCount + ":" + roomSize;

        if(playerCount == roomSize)
            readyToStart = true;
        else if(playerCount >= minPlayersToStart) readyToCountDown = true ;
        else{
            readyToCountDown = false;
            readyToStart = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        // called whenever a new player joins room
        PlayerCountUpdate();

        // send master clients countdown time to all other player to sync time
        if(PhotonNetwork.IsMasterClient)
        myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn){
        // RPC for syncing countdown time
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        if(timeIn < fullGameTimer)
        fullGameTimer = timeIn;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        WaitinfForMorePlayers();
    }

    void WaitinfForMorePlayers(){
        // if only one player in room, reset and stop timer
        if(playerCount <= 1) ResetTimer();

        if(readyToStart){
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if(readyToCountDown){
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }

        timerToStartDisplay.text = string.Format("{0:00}", timerToStartGame);

        if(timerToStartGame <= 0f){
            if(startingGame) return;
            StartGame();
        }
    }

    void StartGame(){
        startingGame = true;
        if(!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }

    void ResetTimer(){
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;
    }

    public void DelayCancel(){
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuSceneIndex);
    }
}
