using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;

    public int ammo;
    public int coin;
    public int health;


    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    //Input Axis 값을 받을 전역변수 선언 
    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool fDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    // 트리거 된 아이템을 저장하기 위한 변수 선언
    GameObject nearObject;

    // 기존에 장착된 무기를 저장하는 변수를 선언하고 활용
    Weapon equipWeapon;

    int equipWeaponIndex = -1;

    float fireDelay;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //Animator 변수를 GetComponentInChildren()으로 초기화
        anim = GetComponentInChildren<Animator>();

    }

    void Start()
    {
        Debug.Log("Game Start");
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Dodge();
        Swap();
        Interation();
    }

    void GetInput()
    {
        //Axis 값을 정수로 반환하는 함수
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        //Shift는 누를 때만 작동하도록 GetButton() 함수 사용
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButton("Jump");
        fDown = Input.GetButton("Fire1");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;
        
        // 스왑과 공격중엔 움직이지 못하도록
        if (isSwap || !isFireReady)
            moveVec = Vector3.zero;
        
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

    }

    void Turn()
    {
        //LookAt() : 지정된 벡터를 향해서 회전시켜주는 함수
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            // AddForce() 함수로 물리적인 힘 가하기
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump",true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger("doSwing");
            fireDelay = 0;
        }
    }

    //OnCollisionEneter() 로 착지 구현
    void OnCollisionEnter(Collision collision)
    {
        // 태그를 활용하여 바닥에 대해서만 작동
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;

        }

    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = moveVec;
            // AddForce() 함수로 물리적인 힘 가하기
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // Invoke() 함수로 시간차 함수 호출
            Invoke("DodgeOut", 0.5f); 
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }



    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
                 equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                // 아이템 정보를 가져와서 해당 무기 입수를 체크
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
   


            if (other.tag == "Item")
            {
                Item item = other.GetComponent<Item>();
                switch (item.type)
                {
                    case Item.Type.Ammo:
                        ammo += item.value;
                        if (ammo > maxAmmo)
                            ammo = maxAmmo;
                        break;
                    case Item.Type.Coin:
                        coin += item.value;
                        if (coin > maxCoin)
                            coin = maxCoin;
                        break;
                    case Item.Type.Heart:
                        health += item.value;
                        if (health > maxHealth)
                            health = maxHealth;
                        break;
                    case Item.Type.Grenade:
                    // 수류탄 개수대로 공전체가 활성화 되도록 구현
                    if (hasGrenades == maxHasGrenades)
                        return;
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    break;

                }
           

            Destroy(other.gameObject);
        }    
    }

    // 트리거 이벤트인 OnTriggerStay, Exit 사용
    void OnTriggerStay(Collider other)
    {
        // Weapon 태그를 조건으로 하여 로직 작성
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;

            Debug.Log(nearObject.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
