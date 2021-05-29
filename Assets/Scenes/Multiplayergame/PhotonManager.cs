using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private TextMeshProUGUI p1Score;
    [SerializeField] private TextMeshProUGUI p2Score;

    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
            SetScoreTexts();
        }
    }

    private void SpawnPlayer()
    {
        int player = 0;
        if (!PhotonNetwork.IsMasterClient)
        {
            player = 1;
        }
        GameObject playerSpawn = PhotonNetwork.Instantiate("Player", spawnPoints[player].position, Quaternion.identity);
        FindObjectOfType<CameraFollow>().target = playerSpawn.transform;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) => SetScoreTexts();

    private void SetScoreTexts()
    {
        p1Score.text = "P1 Score : " + PhotonNetwork.CurrentRoom.CustomProperties["P1SCORE"].ToString();
        p2Score.text = "P2 Score : " + PhotonNetwork.CurrentRoom.CustomProperties["P2SCORE"].ToString();
    }
}