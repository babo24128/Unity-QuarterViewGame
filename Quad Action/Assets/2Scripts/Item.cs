using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
    
    
{
    // enum : ������ Ÿ��(Ÿ�� �̸� ���� �ʿ�)
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon};
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }
    // Update is called once per frame
    void Update()
    {
        // Rotate() �Լ��� ��� ȸ�� ȿ��
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }    
    }

}
