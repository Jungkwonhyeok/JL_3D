using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; // 무기 타입 (근접 / 원거리)
    public Type type;
    public int damage;     // 무기 공격력
    public float rate;     // 공격 속도(공격 간 딜레이)
    public BoxCollider meleeArea;      // 근접 공격 판정 범위
    public TrailRenderer trailEffect;  // 근접 공격 이펙트

    public void Use() // 플레이어가 공격 입력 시 무기를 사용하는 함수
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");   // 이전 공격 코루틴 중단
            StartCoroutine("Swing");  // 근접 공격 코루틴 실행
        }
    }

    IEnumerator Swing() // 근접 공격 시 판정과 이펙트를 제어하는 코루틴
    {
        // 일정 시간 후 공격 판정과 이펙트 활성화
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        // 공격 판정 유지 시간 종료
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        // 공격 이펙트 종료
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
}