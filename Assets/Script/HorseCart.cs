using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HorseCart : MonoBehaviour
{
   public Animator animator;
    public float speed = 0f;
    [SerializeField] Transform path;
    [SerializeField] Transform[] pathTransform;
    [SerializeField] List<Transform> pathList;
    public int currentNode = 0; // 현재노드

    void Start()
    {
        animator = GetComponent<Animator>();
        path = GameObject.Find("PantTransform").transform;
        pathTransform = path.GetComponentsInChildren<Transform>();
        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path)
            {
                pathList.Add(pathTransform[i]);
            }
        }
    }


    void FixedUpdate()
    {
        LookatPoint();
        CheackPoint();
    }

    void LookatPoint()
    {
        Quaternion rot = Quaternion.LookRotation(pathList[currentNode].position -transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        transform.Translate(Vector3.forward* Time.deltaTime*10f);
    }
    void CheackPoint()
    {
        animator.SetBool("RUN", true);
        if (Vector3.Distance(transform.position, pathList[currentNode].position)< 5.5)
        {
            if(currentNode == pathList.Count-1)
            {
                currentNode = 0;    
            }
            else
            {
                currentNode++;
            }
        }
    }
}
