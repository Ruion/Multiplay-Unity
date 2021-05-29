using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CustomMatchmakingLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject joinLobbyButton;

    [SerializeField]
    private GameObject lobbyPanel;

    [SerializeField]
    private GameObject mainPanel;

    public InputField playerNameInput;

    private string roomName;
    private int roomSize;

    private List<RoomInfo> roomListings;

    [SerializeField]
    public Transform roomsContainer;

    [SerializeField]
    public Transform roomListingPrefab;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        joinLobbyButton.SetActive(true);
        roomListings = new List<RoomInfo>();

        // check player name save to playerprefs
        if (PlayerPrefs.HasKey("NickName"))
        {
            if (PlayerPrefs.GetString("NickName") == "")
                PhotonNetwork.NickName = "Player" + Random.Range(0, 1000);
            else
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
        }
        else
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000);

        playerNameInput.text = PhotonNetwork.NickName;
    }

    public void PlayerNameUpdate(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("NickName", nameInput);
    }

    public void JoinLobbyOnClick()
    {
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int tempIndex;
        foreach (RoomInfo room in roomList)
        {
            if (roomListings != null)
                tempIndex = roomListings.FindIndex(ByName(room.Name));
            else tempIndex = -1;

            // if not false
            if (tempIndex != -1) // remove listing because it has been closed or already existed
            {
                roomListings.RemoveAt(tempIndex);
                Destroy(roomsContainer.GetChild(tempIndex).gameObject);
            }

            if (room.PlayerCount > 0) // add room listing because it is new
            {
                roomListings.Add(room);
                ListRoom(room);
            }
        }
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }

    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    // predicate function to search through room
    private static System.Predicate<RoomInfo> ByName(string name)
    {
        // return number or -1 , number or false
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    public void CreateRoom()
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, must be room name existed");
    }

    private void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer).gameObject;
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    // paired to cancel button. Used to go back to main menu
    public void MatchmakingCancel()
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();
    }
}