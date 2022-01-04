using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public enum Type {  Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public void Use()
    {
        if(type == Type.Melee)
        {
           StopCoroutine("Swing");
           StartCoroutine("Swing");
        }
    }

    // �ڷ�ƾ �Լ� ȣ��
    IEnumerator Swing()
    {
        
        // 1
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        // 2
         yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
        
    }

    // �⺻���� �Լ� ȣ�� : Use() �������� -> Swing() �����ƾ -> Use() ���η�ƾ
    // �ڷ�ƾ �Լ� : Use() �������� + Swing() �ڷ�ƾ [Co-Op]
}
