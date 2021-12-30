using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Input Axis ���� ���� �������� ����
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    public float speed;

    bool isJump;
    bool isDodge;
  

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //Animator ������ GetComponentInChildren()���� �ʱ�ȭ
        anim = GetComponentInChildren<Animator>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput()
    {
        //Axis ���� ������ ��ȯ�ϴ� �Լ�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        //Shift�� ���� ���� �۵��ϵ��� GetButton() �Լ� ���
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButton("Jump");

    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (wDown)
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        else
            transform.position += moveVec * speed * Time.deltaTime;

        /*���� �����ڸ� �������
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        */

        transform.position += moveVec * speed * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

    }

    void Turn()
    {
        //LookAt() : ������ ���͸� ���ؼ� ȸ�������ִ� �Լ�
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            // AddForce() �Լ��� �������� �� ���ϱ�
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump",true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            // AddForce() �Լ��� �������� �� ���ϱ�
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // Invoke() �Լ��� �ð��� �Լ� ȣ��
            Invoke("DodgeOut", 0.5f); 
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

        //OnCollisionEneter() �� ���� ����
        void OnCollisionEnter(Collision collision)
    {
        // �±׸� Ȱ���Ͽ� �ٴڿ� ���ؼ��� �۵�
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
            
        }

    }
}
