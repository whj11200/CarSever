using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetintheCar : MonoBehaviour
{
    private string playertag = "Player";
    public bool isGetin = false;
    [SerializeField]
    private GameObject fpsplayer;
    public Camera maincam;
    [SerializeField]
    private AudioListener listener;
    [SerializeField]
    private Camera fpscamer;
    RaycastHit hit;
    Ray ray;

    void Start()
    {
        fpsplayer = GameObject.FindWithTag(playertag);
        listener = fpsplayer.GetComponentInChildren<AudioListener>();
        maincam = Camera.main;
        fpscamer = fpsplayer.GetComponentInChildren<Camera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playertag))
        {
            isGetin = true;
        }
    }

    void Update()
    {
        Debug.DrawRay(fpscamer.transform.position, fpscamer.transform.forward, Color.red, 30f);

        ray = new Ray(fpscamer.transform.position, fpscamer.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetKeyDown(KeyCode.E) && isGetin)
            {
                PlayerinCar();
            }
        }

       
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerOut();
        }
    }

    private void PlayerinCar()
    {
        fpsplayer.SetActive(false);
        maincam.depth = 0;
        fpscamer.depth = -1f;
        listener.enabled = false;
    }

    private void PlayerOut()
    {
        fpsplayer.transform.position = transform.position + new Vector3(-Random.Range(5f, 10f), 0, 0);
        isGetin = false;
        fpsplayer.SetActive(true);
        listener.enabled = true;
    }
}
