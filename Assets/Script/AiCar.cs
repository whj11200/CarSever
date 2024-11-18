using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//1. 무게중심 2.pathTransfrom 3. 휠콜라이더 컴포넌트 모델링 배치
public class AiCar : MonoBehaviour
{
    [Header("CenterMass")]
    [SerializeField] Rigidbody rb;
    public Vector3 CenterMass = new Vector3(0f, -0.5f, 0f);
    [Header("Path")]
    [SerializeField] Transform path;
    [SerializeField] Transform[] pathTransforms;
    [SerializeField] List<Transform> pathList;
    [Header("콜라이더")]
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
    [SerializeField] private float sensorLegth = 40f; // 센서 길이
    [SerializeField] Vector3 frontSenesorPos = new Vector3(0f,0.2f,0f); // 전방센서
    [SerializeField] public Transform Sensorstartpos; //  센서 위치를 차안에 오브젝트로 설정
    [SerializeField] float frontsideSensor = 0.2f; //  전방센서 사이드 포지션
    [SerializeField] float frontSensorAngle = 30f; //  전방센서 사이드 포지션
    bool avoiding = false;// 장애물을 피할것인지 판단
    private float targetSteerAngle = 0f;
    // 현재 스피드
    public float currentspeed = 5f; // 현재 스피드 
    private int currentNode = 0; // 현재 노드
    private float maxspeed = 200f; //  150 이상 못달리게 제한
    public float maxMotorTorque = 1000f; // 훨 콜라이더가 회전하는 최대 힘
    public float maxSteer = 30f; // 앞바퀴 회전 각

   
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
    void ApplySteer()// 앞바퀴 가 휠 콜라이더 가 path에 따라서 회전 하는 메서드 
    {
         Vector3 relativeVector = transform.InverseTransformPoint(pathList[currentNode].position);
        // 실제적인 방향  = 월드 좌표를 게임상의 자표를 로컬좌표로 변환 한다.
                          // 패스트랜스폼 x값 / 패스트랜폼 개체크기 *30
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteer;

        //FL.steerAngle = newSteer;
        //FR.steerAngle = newSteer;
        targetSteerAngle = newSteer;

    }


    void Drive()
    {
        currentspeed = 2 * Mathf.PI * FL.radius * FL.rpm * 60 / 1000;

        // 목표 지점에 가까워질 때 속도를 줄입니다.
        float targetSpeed = 100;
        if (Vector3.Distance(transform.position, pathList[currentNode].position) < 50f) // 목표 지점에서 10m 이내일 때
        {
            targetSpeed = Mathf.Lerp(currentspeed, 10f, Time.deltaTime); // 속도를 점진적으로 줄입니다.
        }

        if (currentspeed < targetSpeed)
        {
            BL.motorTorque = maxMotorTorque;
            BR.motorTorque = maxMotorTorque;
        }
        else // 현재 스피드가 최고 속도보다 높으면
        {
            BL.motorTorque = 0;
            BR.motorTorque = 0;
        }
    }

    void Carsensor()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        // 센서의 스타트 포지션 
        sensorStartPos += transform.forward * frontSenesorPos.z;
        // 센서의 포워드 값
        sensorStartPos += transform.up * frontSenesorPos.y; // 높이를 0.2
        float avoidMutilplier = 0f;
        avoiding = false;
        #region Front Center Sensor(전방센서)
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLegth,1<<6))
        {
            if (!hit.collider.CompareTag("Track"))
            {
                avoiding = true; // 피해라
                
            }
            Debug.DrawLine(sensorStartPos, hit.point, Color.red);
        }

        #endregion
        #region Front Center Sensor(전방오른쪽 센서 회전 할 시 )

        sensorStartPos += transform.right * frontsideSensor;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLegth,1<<6))
        {
           
                avoiding = true; // 피해라

                avoidMutilplier -= 2.0f;
                Debug.DrawLine(sensorStartPos, hit.point, Color.blue);
            
          
        }

        #endregion
        #region 오른쪽 사이드 쪽 센서 회전 할 떄 Ray 에 의해 각을 만들어회전
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) *
            transform.forward, out hit, sensorLegth, 1 << 6))
        {
            Debug.Log("오른쪽 사이드 대각선 줄생성");
            Debug.DrawLine(sensorStartPos, hit.point,Color.magenta);
            avoiding = true;
            avoidMutilplier -= 2.0f;
        }
        #endregion
        #region Front Center Sensor(전방왼쪽 센서)
        sensorStartPos -= transform.right * frontsideSensor*2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLegth,1<<6))
        {
            
                avoiding = true; // 피해라
                Debug.Log($"avoidLeft: {avoiding}");
                avoidMutilplier += 3.0f;
            
            Debug.DrawLine(sensorStartPos, hit.point, Color.green);
        }

        #endregion
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) *
            transform.forward, out hit, sensorLegth, 1 << 6))
        {
            Debug.Log("왼쪽 사이드 대각선 줄생성");
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
                if(hit.normal.x < 0) // 방향
                {
                    // 각도 x 0보다 작은면
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
            // 차량과 도착지점거리를 비교해서 3.5보다 낮아지면
        if (Vector3.Distance(transform.position, pathList[currentNode].position) < 3.5f)
        {
          
            BL.motorTorque = 10;
            BR.motorTorque = 10;
            Debug.Log(BL.motorTorque);
            // 마지막 까지 왔을때 다시 0으로 초기화
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
    // 부드럽게 회전하는 것안하면 
    void LerpToSteerAngle()
    {
        FL.steerAngle = Mathf.Lerp(FL.steerAngle, targetSteerAngle, Time.deltaTime*10f);
        FR.steerAngle = Mathf.Lerp(FR.steerAngle, targetSteerAngle, Time.deltaTime*10f);
    }
}
