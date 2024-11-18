using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetWorkinpu : MonoBehaviourPunCallbacks
{
    
   
    public InputField useridfiled;
    public Text roomid;
    public InputField roomName;
    public string Version = "1.0v";
 

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = Version;
            PhotonNetwork.ConnectUsingSettings();
            roomid.text = "현재 방" + Random.Range(0, 999).ToString("000");
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        useridfiled.text = GetUserId();
    }

    public void OnClickJoinRandomRoom()
    {
        string _roomName = roomName.text;
        if (string.IsNullOrEmpty(roomid.text))
        {
            _roomName = "현재 방" + Random.Range(0, 999).ToString("000");
        }

        PhotonNetwork.NickName = useridfiled.text;
        PlayerPrefs.SetString("유저이름",useridfiled.text);

        RoomOptions roomopptions = new RoomOptions();
        roomopptions.IsOpen = true;
        roomopptions.IsVisible = true;
        roomopptions.MaxPlayers = 10;

        PhotonNetwork.CreateRoom(_roomName, roomopptions, TypedLobby.Default);
    }
   
    string GetUserId()
    {
        // 사용자 ID를 가져오는 메서드
        string userid = PlayerPrefs.GetString("USER_ID"); // 저장된 사용자 ID 가져오기
        if (string.IsNullOrEmpty(userid)) // ID가 비어있거나 null일 경우
        {
            // 0에서 998 사이의 랜덤 숫자를 생성하고, "USER" 접두사 붙여서 반환
            userid = "USER" + Random.Range(0, 999).ToString("000");
        }
        return userid;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 20 });
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("접속성공");
    }
    public void StartButoon()
    {
        StartCoroutine(loadRaceScene());
    }
    IEnumerator loadRaceScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        AsyncOperation ao = SceneManager.LoadSceneAsync("CarBulidScene");
        yield return ao;
    }
}
