using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//1. �����߽� 2.pathTransfrom 3. ���ݶ��̴� ������Ʈ �𵨸� ��ġ
public class AiCar : MonoBehaviour
{
    [Header("CenterMass")]
    [SerializeField] Rigidbody rb;
    public Vector3 CenterMass = new Vector3(0f, -0.5f, 0f);
    [Header("Path")]
    [SerializeField] Transform path;
    [SerializeField] Transform[] pathTransforms;
    [SerializeField] List<Transform> pathList;
    [Header("�ݶ��̴�")]
    [SerializeField]
    WheelCollider FL;
    [SerializeField]
    WheelCollider FR;
    [SerializeField]
    WheelCollider BL;
    [SerializeField]
    WheelCollider BR;
    [SerializeField]
    Transform FLTr;
    [SerializeField]
    Transform FRTr;
    [SerializeField]
    Transform BLTr;
    [SerializeField]
    Transform BRTr;
    [Header("Obstacle Auoid")]
    [SerializeField] private float sensorLegth = 40f; // ���� ����
    [SerializeField] Vector3 frontSenesorPos = new Vector3(0f,0.2f,0f); // ���漾��
    [SerializeField] public Transform Sensorstartpos; //  ���� ��ġ�� ���ȿ� ������Ʈ�� ����
    [SerializeField] float frontsideSensor = 0.2f; //  ���漾�� ���̵� ������
    [SerializeField] float frontSensorAngle = 30f; //  ���漾�� ���̵� ������
    bool avoiding = false;// ��ֹ��� ���Ұ����� �Ǵ�
    private float targetSteerAngle = 0f;
    // ���� ���ǵ�
    public float currentspeed = 5f; // ���� ���ǵ� 
    private int currentNode = 0; // ���� ���
    private float maxspeed = 200f; //  150 �̻� ���޸��� ����
    public float maxMotorTorque = 1000f; // �� �ݶ��̴��� ȸ���ϴ� �ִ� ��
    public float maxSteer = 30f; // �չ��� ȸ�� ��

   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = CenterMass;
        path = GameObject.Find("PantTransform").transform;
        pathTransforms = path.GetComponentsInChildren<Transform>();
        for(int i = 0; i < pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != path)
            {
                pathList.Add(pathTransforms[i]);
            } 
        }
    }


     void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWayPointDistance();
        Carsensor();
        LerpToSteerAngle();
    }
    void ApplySteer()// �չ��� �� �� �ݶ��̴� �� path�� ���� ȸ�� �ϴ� �޼��� 
    {
         Vector3 relativeVector = transform.InverseTransformPoint(pathList[currentNode].position);
        // �������� ����  = ���� ��ǥ�� ���ӻ��� ��ǥ�� ������ǥ�� ��ȯ �Ѵ�.
                          // �н�Ʈ������ x�� / �н�Ʈ���� ��üũ�� *30
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteer;

        //FL.steerAngle = newSteer;
        //FR.steerAngle = newSteer;
        targetSteerAngle = newSteer;

    }


    void Drive()
    {
        currentspeed = 2 * Mathf.PI * FL.radius * FL.rpm * 60 / 1000;

        // ��ǥ ������ ������� �� �ӵ��� ���Դϴ�.
        float targetSpeed = 100;
        if (Vector3.Distance(transform.position, pathList[currentNode].position) < 50f) // ��ǥ �������� 10m �̳��� ��
        {
            targetSpeed = Mathf.Lerp(currentspeed, 10f, Time.deltaTime); // �ӵ��� ���������� ���Դϴ�.
        }

        if (currentspeed < targetSpeed)
        {
            BL.motorTorque = maxMotorTorque;
            BR.motorTorque = maxMotorTorque;
        }
        else // ���� ���ǵ尡 �ְ� �ӵ����� ������
        {
            BL.motorTorque = 0;
            BR.motorTorque = 0;
        }
    }

    void Carsensor()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        // ������ ��ŸƮ ������ 
        sensorStartPos += transform.forward * frontSenesorPos.z;
        // ������ ������ ��
        sensorStartPos += transform.up * frontSenesorPos.y; // ���̸� 0.2
        float avoidMutilplier = 0f;
        avoiding = false;
        #region Front Center Sensor(���漾��)
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLegth,1<<6))
        {
            if (!hit.collider.CompareTag("Track"))
            {
                avoiding = true; // ���ض�
                
            }
            Debug.DrawLine(sensorStartPos, hit.point, Color.red);
        }

        #endregion
        #region Front Center Sensor(��������� ���� ȸ�� �� �� )

        sensorStartPos += transform.right * frontsideSensor;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLegth,1<<6))
        {
           
                avoiding = true; // ���ض�

                avoidMutilplier -= 2.0f;
                Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            
          
        }

        #endregion
        #region ������ ���̵� �� ���� ȸ�� �� �� Ray �� ���� ���� �����ȸ��
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) *
            transform.forward, out hit, sensorLegth, 1 << 6))
        {
            Debug.Log("������ ���̵� �밢�� �ٻ���");
            Debug.DrawLine(sensorStartPos, hit.point,Color.magenta);
            avoiding = true;
            avoidMutilplier -= 2.0f;
        }
        #endregion
        #region Front Center Sensor(������� ����)
        sensorStartPos -= transform.right * frontsideSensor*2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLegth,1<<6))
        {
            
                avoiding = true; // ���ض�
                Debug.Log($"avoidLeft: {avoiding}");
                avoidMutilplier += 3.0f;
            
            Debug.DrawLine(sensorStartPos, hit.point, Color.green);
        }

        #endregion
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) *
            transform.forward, out hit, sensorLegth, 1 << 6))
        {
            Debug.Log("���� ���̵� �밢�� �ٻ���");
            Debug.DrawLine(sensorStartPos, hit.point, Color.magenta);
            avoiding = true;
            avoidMutilplier += 3.0f;
        }


        if(avoidMutilplier == 0) 
        {
            if(Physics.Raycast(sensorStartPos,Quaternion.AngleAxis(frontSensorAngle,transform.up) *
                transform.forward,out hit, sensorLegth, 1 << 6))
            {
                avoiding = true;
                if(hit.normal.x < 0) // ����
                {
                    // ���� x 0���� ������
                    avoidMutilplier = -1f;
                }
                else
                {
                    avoidMutilplier = 1f;
                }

            }
        }
        if (avoiding)
        {
            targetSteerAngle = maxSteer * avoidMutilplier;

        }
    }


    void CheckWayPointDistance()
    {  
            // ������ ���������Ÿ��� ���ؼ� 3.5���� ��������
        if (Vector3.Distance(transform.position, pathList[currentNode].position) < 3.5f)
        {
          
            BL.motorTorque = 10;
            BR.motorTorque = 10;
            Debug.Log(BL.motorTorque);
            // ������ ���� ������ �ٽ� 0���� �ʱ�ȭ
            if (currentNode == pathList.Count - 1)
            {
                currentNode = 0;
                
            }
            else
            {
                currentNode++;
            }
        }
    }
    // �ε巴�� ȸ���ϴ� �;��ϸ� 
    void LerpToSteerAngle()
    {
        FL.steerAngle = Mathf.Lerp(FL.steerAngle, targetSteerAngle, Time.deltaTime*10f);
        FR.steerAngle = Mathf.Lerp(FR.steerAngle, targetSteerAngle, Time.deltaTime*10f);
    }
}
