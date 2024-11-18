using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;
using Photon.Realtime;

public class PlayerCar : MonoBehaviourPun,IPunObservable
{
    private Transform tr;
    private Vector3 curpos = Vector3.zero; //벡터와 쿼터니언으로  동작을 변수로 수신받는다.
    private Quaternion currot = Quaternion.identity;
    // 차량의 전조등, 후미등, 브레이크등 배열
    public Light[] P_Lights; // 전조등
    public Light[] BF_lights; // 후미등
    public Light[] B_lights; // 브레이크등
    public Light[] S_Lights; // 상향등

    [Header("WhellCollider")]
    // 차량의 바퀴와 관련된 WheelCollider 변수
    public WheelCollider frontLeft_col; // 왼쪽 앞바퀴
    public WheelCollider frontRight_col; // 오른쪽 앞바퀴
    public WheelCollider backLeft_col; // 왼쪽 뒷바퀴
    public WheelCollider backRight_col; // 오른쪽 뒷바퀴

    [Header("Model")]
    // 차량 모델의 바퀴와 관련된 Transform 변수
    public Transform frontLeft_M; // 왼쪽 앞바퀴 모델
    public Transform frontRight_M; // 오른쪽 앞바퀴 모델
    public Transform backLeft_M; // 왼쪽 뒷바퀴 모델
    public Transform backRight_M; // 오른쪽 뒷바퀴 모델

    [Header("Mass Blance")] // 무게중심
    private Vector3 CentOffMass = new Vector3(0, -0.5f, 0); // 차량의 무게중심 설정
    public Rigidbody rb; // Rigidbody 컴포넌트

    [Header("앞바퀴 최대 회전각")]
    public float maxSteer = 35f; // 최대 스티어링 각도

    [Header("최대 마찰력")]
    public float maxToque = 3500f; // 최대 토크

    [Header("최대 브레이크")]
    public float maxBrake = 5000f; // 최대 브레이크

    [Header("현재 스피드")]
    public float currentSpeed = 0f; // 현재 속도
    private float Steer = 0f; // 방향 조정
    private float forward = 0f; // 전진 입력
    private float back = 0f; // 후진 입력
    bool isrevers = false; // 후진 여부
    private float motor = 0f; // 모터 토크
    private float brake = 0f; // 브레이크 토크
    private GetintheCar getinthe; // 차량 탑승 스크립트
    private bool isHighBeamActive = false; // 상향등 활성화 상태

   
    void Awake()
    {
        tr = transform;
        photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
        photonView.ObservedComponents[0] = this;
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 초기화
        
      
        rb.centerOfMass = CentOffMass; // 무게중심 설정

        // 전조등, 후미등, 브레이크등 초기화
        for (int i = 0; i < P_Lights.Length; i++)
        {
            P_Lights[i].enabled = false;
        }
        for (int j = 0; j < B_lights.Length; j++)
        {
            B_lights[j].enabled = false;
        }
        for (int i = 0; i < BF_lights.Length; i++)
        {
            BF_lights[i].enabled = false;
        }

        getinthe = transform.GetChild(5).GetComponent<GetintheCar>(); // 차량 탑승 스크립트 가져오기
    }

    private void Update()
    {
        if (photonView.IsMine)
  
            // H키를 눌렀을 때 전조등 토글
            if (Input.GetKeyDown(KeyCode.H))
            {

                HeadLight();

            }
        
       
    }

    private void HeadLight()
    {
        for (int i = 0; i < P_Lights.Length; i++)
        {
            P_Lights[i].enabled = !P_Lights[i].enabled;

        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Carmove(); // 차량 움직임 함수 호출

            // 스페이스바를 눌렀을 때 브레이크
            if (Input.GetKey(KeyCode.Space))
            {
                CarBreak();
            }
            else
            {
                Carmove(); // 차량 움직임 함수 호출
            }
        }
        // 차량에 탑승 중일 때
        //if (getinthe.isGetin)
        //{
           
        //}

        // 후진 중일 때
        if (isrevers)
        {
            motor = -1 * back; // 후진할 때 모터 토크 설정
            brake = forward; // 전진할 때 브레이크 설정
                             // 후진등 켜기
            for (int i = 0; i < BF_lights.Length; i++)
            {
                BF_lights[i].enabled = true;
            }
        }
        else
        {
            motor = forward; // 전진할 때 모터 토크 설정
            brake = back; // 후진할 때 브레이크 설정
            // 후진등 끄기
            for (int i = 0; i < BF_lights.Length; i++)
            {
                BF_lights[i].enabled = false;
            }
        }

        // 뒷바퀴 토크 회전력 설정
        backLeft_col.motorTorque = motor * maxToque;
        backRight_col.motorTorque = motor * maxToque;
        backLeft_col.brakeTorque = brake * maxBrake;
        backRight_col.brakeTorque = brake * maxBrake;

        // 앞바퀴 스티어링 설정
        frontLeft_col.steerAngle = maxSteer * Steer;
        frontRight_col.steerAngle = maxSteer * Steer;

        // 바퀴 모델 회전 설정
        frontLeft_M.localEulerAngles = new Vector3(frontLeft_M.localEulerAngles.x, Steer * maxSteer, frontLeft_M.localEulerAngles.z);
        frontRight_M.localEulerAngles = new Vector3(frontRight_M.localEulerAngles.x, Steer * maxSteer, frontRight_M.localEulerAngles.z);

        // 바퀴 모델을 실제 회전 속도에 맞춰 회전
        frontLeft_M.Rotate(frontLeft_col.rpm * Time.deltaTime, 0f, 0f);
        frontRight_M.Rotate(frontRight_col.rpm * Time.deltaTime, 0f, 0f);
        backLeft_M.Rotate(backLeft_col.rpm * Time.deltaTime, 0f, 0f);
        backRight_M.Rotate(backRight_col.rpm * Time.deltaTime, 0f, 0f);
    }

    private void Carmove()
    {
        // 후미등 끄기
        for (int j = 0; j < B_lights.Length; j++)
        {
            B_lights[j].enabled = false;
        }

        // 브레이크 토크 초기화
        frontLeft_col.brakeTorque = 0f;
        frontRight_col.brakeTorque = 0f;
        currentSpeed = rb.velocity.sqrMagnitude; // 현재 속도 계산

        // 입력값에 따라 방향 조정
        Steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
        forward = Mathf.Clamp(Input.GetAxis("Vertical"), 0f, 1f); // 전진 입력
        back = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f); // 후진 입력

        // W키를 눌렀을 때 전진
        if (Input.GetKey(KeyCode.W))
        {
            StartCoroutine(ForWardCar());
        }

        // S키를 눌렀을 때 후진
        if (Input.GetKey(KeyCode.S))
        {
            StartCoroutine(BackWardCar());
           
        }
        
    }

    IEnumerator ForWardCar()
    {
        yield return new WaitForSeconds(0.1f); // 잠시 대기
        currentSpeed = 0f; // 현재 속도 초기화
        if (back > 0f)
            isrevers = true; // 후진 상태 설정
        if (forward > 0f)
            isrevers = false; // 전진 상태 설정
    }

    IEnumerator BackWardCar()
    {
        yield return new WaitForSeconds(0.1f); // 잠시 대기
        currentSpeed = 0.1f; // 현재 속도 초기화
        if (back > 0f)
            isrevers = true; // 후진 상태 설정
        
        if (forward > 0f)
            isrevers = false; // 전진 상태 해제
       
    }

    void CarBreak()
    {
        // 모터 토크를 0으로 설정하여 브레이크
        frontRight_col.motorTorque = 0f;
        frontLeft_col.motorTorque = 0f;
        frontLeft_col.brakeTorque = 50000f; // 브레이크 토크 설정
        frontRight_col.brakeTorque = 50000f; // 브레이크 토크 설정

        // 브레이크등 켜기
        for (int j = 0; j < B_lights.Length; j++)
        {
            B_lights[j].enabled = true;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else
        {
            curpos = (Vector3)stream.ReceiveNext();
            currot = (Quaternion)stream.ReceiveNext();
        }
    }

}
