using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonNetWork : MonoBehaviourPunCallbacks
{
    public InputField userid;
    public InputField roomid;
    string Version = "v1";

    private void Awake()
    {
        PhotonNetwork.GameVersion = Version;
        //PhotonNetwork.IsMessageQueueRunning = true;
        
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            roomid.text = "현재 방" + Random.Range(0, 999).ToString("000");
        }
    }
    
    public override void OnJoinedLobby()
    {
        Debug.Log("접속완료");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤룸 참가 실패");
        PhotonNetwork.CreateRoom("CarRoom", new RoomOptions { MaxPlayers = 5 });
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("방생성 성공");

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("방 참가 성공");
        StartCoroutine(loadRaceScene());
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("오류코드"+ returnCode.ToString());
        Debug.Log("방 생성 실패");
    }
    IEnumerator loadRaceScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        AsyncOperation ao = SceneManager.LoadSceneAsync("CarBulidScene");
        yield return ao;
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public void OnclickJoinRandRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = userid.text;
        PlayerPrefs.SetString("USER_ID",userid.text);

        PhotonNetwork.JoinRandomRoom();
    }
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }


}
