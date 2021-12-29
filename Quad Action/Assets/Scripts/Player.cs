using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Input Axis 값을 받을 전역변수 선언
    float hAxis;
    float vAxis;

    bool wDown;
    public float speed;

    Vector3 moveVec;

    Animator anim;

    private void Awake()
    {
        //Animator 변수를 GetComponentInChildren()으로 초기화
        anim = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Axis 값을 정수로 반환하는 함수
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        //Shift는 누를 때만 작동하도록 GetButton() 함수 사용
        wDown = Input.GetButton("Walk");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (wDown)
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        else
            transform.position += moveVec * speed * Time.deltaTime;

        /*삼항 연산자를 썼을경우
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        */

        transform.position += moveVec * speed * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        //LookAt() : 지정된 벡터를 향해서 회전시켜주는 함수
        transform.LookAt(transform.position + moveVec);
    }

}
