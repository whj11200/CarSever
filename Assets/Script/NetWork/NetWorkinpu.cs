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
            roomid.text = "���� ��" + Random.Range(0, 999).ToString("000");
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
            _roomName = "���� ��" + Random.Range(0, 999).ToString("000");
        }

        PhotonNetwork.NickName = useridfiled.text;
        PlayerPrefs.SetString("�����̸�",useridfiled.text);

        RoomOptions roomopptions = new RoomOptions();
        roomopptions.IsOpen = true;
        roomopptions.IsVisible = true;
        roomopptions.MaxPlayers = 10;

        PhotonNetwork.CreateRoom(_roomName, roomopptions, TypedLobby.Default);
    }
   
    string GetUserId()
    {
        // ����� ID�� �������� �޼���
        string userid = PlayerPrefs.GetString("USER_ID"); // ����� ����� ID ��������
        if (string.IsNullOrEmpty(userid)) // ID�� ����ְų� null�� ���
        {
            // 0���� 998 ������ ���� ���ڸ� �����ϰ�, "USER" ���λ� �ٿ��� ��ȯ
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
        Debug.Log("���Ӽ���");
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
