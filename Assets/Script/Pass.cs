using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass : MonoBehaviour
{
    [SerializeField]
    Color lineColor;
    [SerializeField]
    List<Transform> Nodes = new List<Transform>();
    private void OnDrawGizmos() // 좌표에 색상 선과 아이콘을 그릴수있음
    {
        Gizmos.color = lineColor;
        Transform[] pathTransfroms = GetComponentsInChildren<Transform>();
        Nodes = new List<Transform>();
        for(int i = 0; i < pathTransfroms.Length; i++)// 자기 자신을 제외하고 하위 트랜스폼을 담는다.
        {
            if(pathTransfroms[i] != null)
            {
                Nodes.Add(pathTransfroms[i]);
            }
        }
        for(int i = 0; i < Nodes.Count; i++)
        {
            Vector3 currentNode = Nodes[i].position;
            Vector3 previousNode = Vector3.zero; // 이전 노드
            if (i>0)
            {
                previousNode = Nodes[i-1].position;
            }
            else if(i==0 && Nodes.Count >1) // i가 0과 같고 노드 카운터가 1이상이면 
            {
               previousNode = Nodes[Nodes.Count-1].position;
            }
            Gizmos.DrawLine(currentNode, previousNode);
            Gizmos.DrawSphere(currentNode, 1.0f);

        }
    }
}
