using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;
using Photon.Realtime;

public class PlayerCar : MonoBehaviourPun,IPunObservable
{
    private Transform tr;
    private Vector3 curpos = Vector3.zero; //���Ϳ� ���ʹϾ�����  ������ ������ ���Ź޴´�.
    private Quaternion currot = Quaternion.identity;
    // ������ ������, �Ĺ̵�, �극��ũ�� �迭
    public Light[] P_Lights; // ������
    public Light[] BF_lights; // �Ĺ̵�
    public Light[] B_lights; // �극��ũ��
    public Light[] S_Lights; // �����

    [Header("WhellCollider")]
    // ������ ������ ���õ� WheelCollider ����
    public WheelCollider frontLeft_col; // ���� �չ���
    public WheelCollider frontRight_col; // ������ �չ���
    public WheelCollider backLeft_col; // ���� �޹���
    public WheelCollider backRight_col; // ������ �޹���

    [Header("Model")]
    // ���� ���� ������ ���õ� Transform ����
    public Transform frontLeft_M; // ���� �չ��� ��
    public Transform frontRight_M; // ������ �չ��� ��
    public Transform backLeft_M; // ���� �޹��� ��
    public Transform backRight_M; // ������ �޹��� ��

    [Header("Mass Blance")] // �����߽�
    private Vector3 CentOffMass = new Vector3(0, -0.5f, 0); // ������ �����߽� ����
    public Rigidbody rb; // Rigidbody ������Ʈ

    [Header("�չ��� �ִ� ȸ����")]
    public float maxSteer = 35f; // �ִ� ��Ƽ� ����

    [Header("�ִ� ������")]
    public float maxToque = 3500f; // �ִ� ��ũ

    [Header("�ִ� �극��ũ")]
    public float maxBrake = 5000f; // �ִ� �극��ũ

    [Header("���� ���ǵ�")]
    public float currentSpeed = 0f; // ���� �ӵ�
    private float Steer = 0f; // ���� ����
    private float forward = 0f; // ���� �Է�
    private float back = 0f; // ���� �Է�
    bool isrevers = false; // ���� ����
    private float motor = 0f; // ���� ��ũ
    private float brake = 0f; // �극��ũ ��ũ
    private GetintheCar getinthe; // ���� ž�� ��ũ��Ʈ
    private bool isHighBeamActive = false; // ����� Ȱ��ȭ ����

   
    void Awake()
    {
        tr = transform;
        photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
        photonView.ObservedComponents[0] = this;
        rb = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ �ʱ�ȭ
        
      
        rb.centerOfMass = CentOffMass; // �����߽� ����

        // ������, �Ĺ̵�, �극��ũ�� �ʱ�ȭ
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

        getinthe = transform.GetChild(5).GetComponent<GetintheCar>(); // ���� ž�� ��ũ��Ʈ ��������
    }

    private void Update()
    {
        if (photonView.IsMine)
  
            // HŰ�� ������ �� ������ ���
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
            Carmove(); // ���� ������ �Լ� ȣ��

            // �����̽��ٸ� ������ �� �극��ũ
            if (Input.GetKey(KeyCode.Space))
            {
                CarBreak();
            }
            else
            {
                Carmove(); // ���� ������ �Լ� ȣ��
            }
        }
        // ������ ž�� ���� ��
        //if (getinthe.isGetin)
        //{
           
        //}

        // ���� ���� ��
        if (isrevers)
        {
            motor = -1 * back; // ������ �� ���� ��ũ ����
            brake = forward; // ������ �� �극��ũ ����
                             // ������ �ѱ�
            for (int i = 0; i < BF_lights.Length; i++)
            {
                BF_lights[i].enabled = true;
            }
        }
        else
        {
            motor = forward; // ������ �� ���� ��ũ ����
            brake = back; // ������ �� �극��ũ ����
            // ������ ����
            for (int i = 0; i < BF_lights.Length; i++)
            {
                BF_lights[i].enabled = false;
            }
        }

        // �޹��� ��ũ ȸ���� ����
        backLeft_col.motorTorque = motor * maxToque;
        backRight_col.motorTorque = motor * maxToque;
        backLeft_col.brakeTorque = brake * maxBrake;
        backRight_col.brakeTorque = brake * maxBrake;

        // �չ��� ��Ƽ� ����
        frontLeft_col.steerAngle = maxSteer * Steer;
        frontRight_col.steerAngle = maxSteer * Steer;

        // ���� �� ȸ�� ����
        frontLeft_M.localEulerAngles = new Vector3(frontLeft_M.localEulerAngles.x, Steer * maxSteer, frontLeft_M.localEulerAngles.z);
        frontRight_M.localEulerAngles = new Vector3(frontRight_M.localEulerAngles.x, Steer * maxSteer, frontRight_M.localEulerAngles.z);

        // ���� ���� ���� ȸ�� �ӵ��� ���� ȸ��
        frontLeft_M.Rotate(frontLeft_col.rpm * Time.deltaTime, 0f, 0f);
        frontRight_M.Rotate(frontRight_col.rpm * Time.deltaTime, 0f, 0f);
        backLeft_M.Rotate(backLeft_col.rpm * Time.deltaTime, 0f, 0f);
        backRight_M.Rotate(backRight_col.rpm * Time.deltaTime, 0f, 0f);
    }

    private void Carmove()
    {
        // �Ĺ̵� ����
        for (int j = 0; j < B_lights.Length; j++)
        {
            B_lights[j].enabled = false;
        }

        // �극��ũ ��ũ �ʱ�ȭ
        frontLeft_col.brakeTorque = 0f;
        frontRight_col.brakeTorque = 0f;
        currentSpeed = rb.velocity.sqrMagnitude; // ���� �ӵ� ���

        // �Է°��� ���� ���� ����
        Steer = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
        forward = Mathf.Clamp(Input.GetAxis("Vertical"), 0f, 1f); // ���� �Է�
        back = -1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f); // ���� �Է�

        // WŰ�� ������ �� ����
        if (Input.GetKey(KeyCode.W))
        {
            StartCoroutine(ForWardCar());
        }

        // SŰ�� ������ �� ����
        if (Input.GetKey(KeyCode.S))
        {
            StartCoroutine(BackWardCar());
           
        }
        
    }

    IEnumerator ForWardCar()
    {
        yield return new WaitForSeconds(0.1f); // ��� ���
        currentSpeed = 0f; // ���� �ӵ� �ʱ�ȭ
        if (back > 0f)
            isrevers = true; // ���� ���� ����
        if (forward > 0f)
            isrevers = false; // ���� ���� ����
    }

    IEnumerator BackWardCar()
    {
        yield return new WaitForSeconds(0.1f); // ��� ���
        currentSpeed = 0.1f; // ���� �ӵ� �ʱ�ȭ
        if (back > 0f)
            isrevers = true; // ���� ���� ����
        
        if (forward > 0f)
            isrevers = false; // ���� ���� ����
       
    }

    void CarBreak()
    {
        // ���� ��ũ�� 0���� �����Ͽ� �극��ũ
        frontRight_col.motorTorque = 0f;
        frontLeft_col.motorTorque = 0f;
        frontLeft_col.brakeTorque = 50000f; // �극��ũ ��ũ ����
        frontRight_col.brakeTorque = 50000f; // �극��ũ ��ũ ����

        // �극��ũ�� �ѱ�
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
