using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public void Use() //플레이어가 공격 할 떄 무기 사용하는 함수
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing() //공격 시 공격 범위, 효과를 나타내주는 함수(코루틴)
    {
        yield return new WaitForSeconds(0.1f); //0.1초 뒤에 공격 범위(BoxCollider), 근접 공격 효과(TrailRenderer) 활성화
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f); //0.3초 뒤 공격 범위(BoxCollider) 비활성화
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f); //0.3초 뒤 근접 공격 효과(TrailRenderer) 비활성화
        trailEffect.enabled = false;
    }
}
