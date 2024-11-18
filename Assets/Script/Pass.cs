using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass : MonoBehaviour
{
    [SerializeField]
    Color lineColor;
    [SerializeField]
    List<Transform> Nodes = new List<Transform>();
    private void OnDrawGizmos() // ��ǥ�� ���� ���� �������� �׸�������
    {
        Gizmos.color = lineColor;
        Transform[] pathTransfroms = GetComponentsInChildren<Transform>();
        Nodes = new List<Transform>();
        for(int i = 0; i < pathTransfroms.Length; i++)// �ڱ� �ڽ��� �����ϰ� ���� Ʈ�������� ��´�.
        {
            if(pathTransfroms[i] != null)
            {
                Nodes.Add(pathTransfroms[i]);
            }
        }
        for(int i = 0; i < Nodes.Count; i++)
        {
            Vector3 currentNode = Nodes[i].position;
            Vector3 previousNode = Vector3.zero; // ���� ���
            if (i>0)
            {
                previousNode = Nodes[i-1].position;
            }
            else if(i==0 && Nodes.Count >1) // i�� 0�� ���� ��� ī���Ͱ� 1�̻��̸� 
            {
               previousNode = Nodes[Nodes.Count-1].position;
            }
            Gizmos.DrawLine(currentNode, previousNode);
            Gizmos.DrawSphere(currentNode, 1.0f);

        }
    }
}
