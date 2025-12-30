using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range, Magic }; // 무기 타입 (근접 / 원거리 / 마법)
    public Type type;
    public int damage;     // 무기 공격력
    public float rate;     // 공격 속도(공격 간 딜레이)
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;      // 근접 공격 판정 범위
    public TrailRenderer trailEffect;  // 근접 공격 이펙트
    public Transform bulletPos;
    public GameObject bullet;

    public void Use() // 플레이어가 공격 입력 시 무기를 사용하는 함수
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");   // 이전 공격 코루틴 중단
            StartCoroutine("Swing");  // 근접 공격 코루틴 실행
        }
        else if(type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
        else if (type == Type.Magic)
        {
            if(gameObject.name == "staff")
            {
                StartCoroutine("MagicCall");
                return;
            }
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() // 근접 공격 시 판정과 이펙트를 제어하는 코루틴
    {
        // 일정 시간 후 공격 판정과 이펙트 활성화
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        // 공격 판정 유지 시간 종료
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = false;

        // 공격 이펙트 종료
        yield return new WaitForSeconds(0.2f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot() //원거리 공격 시 총알 발사하는 코루틴
    {
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); //특정 위치에 Bullet소환
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //특정위치 기준 앞으로 50만큼 속도를 더해줘라


        yield return null;
    }

    IEnumerator MagicCall()
    {
        RaycastHit rayHit;
        Physics.Raycast(bulletPos.position, bulletPos.forward, out rayHit, 1000);

        Vector3 MagicPos = rayHit.point;
        MagicPos.y = 0.5f;

        yield return new WaitForSeconds(0.1f);
        GameObject intantMagic = Instantiate(bullet, MagicPos, Quaternion.identity);

        yield return new WaitForSeconds(2f);
        Destroy(intantMagic);
    }
    
}