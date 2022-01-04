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

    // 코루틴 함수 호출
    IEnumerator Swing()
    {
        
        // 1
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        // 2
         yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
        
    }

    // 기본적인 함수 호출 : Use() 메인투린 -> Swing() 서브루틴 -> Use() 메인루틴
    // 코루틴 함수 : Use() 메인투린 + Swing() 코루틴 [Co-Op]
}
