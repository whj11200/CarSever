using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Camervial : MonoBehaviourPun
{
    
    void Start()
    {
        if (photonView.IsMine)
        {
            CinemachineVirtualCamera virtualCamera = FindObjectOfType(typeof(CinemachineVirtualCamera)) as CinemachineVirtualCamera;
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

    }
   

}
