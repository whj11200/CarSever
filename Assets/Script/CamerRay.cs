using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerRay : MonoBehaviour
{
    
    void Start()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red, 30f);   
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
