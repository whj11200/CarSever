using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManger : MonoBehaviourPunCallbacks
{

    private GameManger instance;
    int Count;
    int PlayerClamp;


    private void Awake()
    {
        
        PhotonNetwork.IsMessageQueueRunning = true;
        PlayerClamp = PhotonNetwork.CurrentRoom.MaxPlayers;
        Count = PhotonNetwork.CurrentRoom.PlayerCount;
        if(PlayerClamp > 5)
        {
            return;
        }
        CreateCar(Count);
        PhotonNetwork.IsMessageQueueRunning = true;
    }

   
    public void CreateCar(int palyerCount)
    {
        switch (palyerCount)
        {
            case 1:
                Vector3 Spwan1 = new Vector3(60.1f, 0.5f, 0.88f);
                Quaternion quaternion = Quaternion.Euler(0f, -90f, 0f);
                PhotonNetwork.Instantiate("PlayerCar", Spwan1, quaternion);
                break;
            case 2:
                Vector3 Spwan2 = new Vector3(60.1f*1.5f, 0.5f, 0.88f*1.5f);
                Quaternion quaternion2 = Quaternion.Euler(0f, -90f, 0f);
                PhotonNetwork.Instantiate("PlayerCar", Spwan2, quaternion2);
                break;
            case 3:
                Vector3 Spwan3 = new Vector3(60.1f*2, 0.5f, 0.88f*2);
                Quaternion quaternion3 = Quaternion.Euler(0f, -90f, 0f);
                PhotonNetwork.Instantiate("PlayerCar", Spwan3, quaternion3);
                break;
            case 4:
                Vector3 Spwan4 = new Vector3(60.1f*2.5f, 0.5f, 0.88f*2.5f);
                Quaternion quaternion4 = Quaternion.Euler(0f, -90f, 0f);
                PhotonNetwork.Instantiate("PlayerCar", Spwan4, quaternion4);
                break;
           
        }
        //PhotonNetwork.Instantiate("PlayerCar", new Vector3(60.1f + PhotonNetwork.LocalPlayer.ActorNumber * Random.Range(1f,3f), 1f, 2.958026f + PhotonNetwork.LocalPlayer.ActorNumber * Random.Range(1f, 4.5f)), Quaternion.Euler(0,-90f,0));
    }
}
