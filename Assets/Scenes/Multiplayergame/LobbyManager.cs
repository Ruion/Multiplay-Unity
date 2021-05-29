using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject findMatchBtn;
    [SerializeField] private GameObject cancelFindMatchBtn;

    // Start is called before the first frame update
    private void Start()
    {
        findMatchBtn.SetActive(false);
        cancelFindMatchBtn.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        findMatchBtn.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log($"Connected to {PhotonNetwork.CloudRegion} server!");
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void FindMatch()
    {
        findMatchBtn.SetActive(false);
        cancelFindMatchBtn.SetActive(true);

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Could not find room - creating a room");
        MakeRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MakeRoom();
    }

    private void MakeRoom()
    {
        int randomRange = Random.Range(0, 50000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)(int)2 };

        Hastable roomCustomProps = new Hastable();
        roomCustomProps.Add("P1SCORE", 0);
        roomCustomProps.Add("P2SCORE", 0);
        roomOps.CustomRoomProperties = roomCustomProps;

        PhotonNetwork.CreateRoom($"Room-{randomRange}", roomOps, null);
        Debug.Log("Room created, waiting for another player");
    }

    public void CancelFindMatch()
    {
        findMatchBtn.SetActive(true);
        cancelFindMatchBtn.SetActive(false);
        PhotonNetwork.LeaveRoom();
        Debug.Log("Stop finding match, back to menu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            PhotonNetwork.LoadLevel(1);
    }
}