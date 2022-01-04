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

    //Input Axis ���� ���� �������� ���� 
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

    // Ʈ���� �� �������� �����ϱ� ���� ���� ����
    GameObject nearObject;

    // ������ ������ ���⸦ �����ϴ� ������ �����ϰ� Ȱ��
    Weapon equipWeapon;

    int equipWeaponIndex = -1;

    float fireDelay;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //Animator ������ GetComponentInChildren()���� �ʱ�ȭ
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
        //Axis ���� ������ ��ȯ�ϴ� �Լ�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        //Shift�� ���� ���� �۵��ϵ��� GetButton() �Լ� ���
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
        
        // ���Ұ� �����߿� �������� ���ϵ���
        if (isSwap || !isFireReady)
            moveVec = Vector3.zero;
        
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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            // AddForce() �Լ��� �������� �� ���ϱ�
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

    //OnCollisionEneter() �� ���� ����
    void OnCollisionEnter(Collision collision)
    {
        // �±׸� Ȱ���Ͽ� �ٴڿ� ���ؼ��� �۵�
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
                // ������ ������ �����ͼ� �ش� ���� �Լ��� üũ
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
                    // ����ź ������� ����ü�� Ȱ��ȭ �ǵ��� ����
                    if (hasGrenades == maxHasGrenades)
                        return;
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    break;

                }
           

            Destroy(other.gameObject);
        }    
    }

    // Ʈ���� �̺�Ʈ�� OnTriggerStay, Exit ���
    void OnTriggerStay(Collider other)
    {
        // Weapon �±׸� �������� �Ͽ� ���� �ۼ�
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
