using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C }; // 어떤 타입의 Enemy인지 구분 해주는 열거형 변수
    public Type enemyType;
    public int maxHealth; //최대 체력
    public int curHealth; //현재 체력
    public Transform Target; // 추적 할 타겟
    public BoxCollider meleeArea; //근접 공격 범위
    public bool isChase; //추적하고 있는지 여부
    public bool isAttack; //공격중인지

    Rigidbody rigid;
    CapsuleCollider capsuleCollider;
    Renderer[] renders;
    NavMeshAgent nav;
    Animator anim;

    float hitCool = 1f; // 마법 영역에서 몇초에 1번씩 데미지가 들어 오는지 정
    float hitDelay = 1f; // 마법 영역에서 데미지가 들어오고 얼마나 지났는지 저장하는 변수
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        renders = GetComponentsInChildren<Renderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    void ChaseStart() // 추적 시작하는 함수
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        if (nav.enabled) //nav가 활성화 되어있으면
        {
            nav.SetDestination(Target.position); //추적 할 목표물 위치 정하기
            nav.isStopped = !isChase; //추적 중이지 않으면 멈춘다
        }
    }
    void FreezeVelocity() // 물리력이 추적을 방해하지 않도록 하는 함수
    {
        //물리력이 추적을 방해하지 않아야한다고 했으므로 불필요한 거 없이
        //추적하는 에너미의 움직임을 아얘 방해하지 않고 물리 회전 정도를 아예 0으로 만들어버려야 한다고
        //판단하여 남긴 것
        rigid.angularVelocity = Vector3.zero;
    }

    void Targerting() //타겟팅을 위한 함수
    {
        float targetRadius = 0; //SphereCast의 반지름
        float targetRange = 0; //SphereCast의 길이

        switch (enemyType) //타입에 따라 SphereCast의 반지름과 길이를 정해줌
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 10f;
                break;
            case Type.C:

                break;
        }

        RaycastHit[] raycastHits =
            Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (raycastHits.Length > 0 && !isAttack && curHealth > 0) //cast 범위에 들어오고 공격하고 읶지 않을때
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack() // 공격하는 코루틴
    {
        isChase = false; //추적 비활성화
        isAttack = true; //공격 하고 있는지 여부(O)
        anim.SetBool("isAttack", true);

        switch (enemyType) //타입에 따라 어떤 공격을 할 건지 정해줌
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);

                meleeArea.enabled = true; //0.2초 뒤 근접 공격 범위 활성화

                yield return new WaitForSeconds(1f);

                meleeArea.enabled = false; //1초 뒤 근접 공격 범위 비활성화

                yield return new WaitForSeconds(1f); // 1초 동안 멈춤
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                if (curHealth > 0)
                    rigid.AddForce(transform.forward * 20, ForceMode.Impulse); //정면으로 20만큼 힘을 더 해줌 (데쉬)

                meleeArea.enabled = true; // 근접 공격 범위 활성화

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero; // 데쉬 후 정지 시켜줌

                meleeArea.enabled = false; //근접 공격 범위 비활성화


                yield return new WaitForSeconds(2f); // 2초 동안 멈춤
                break;
            case Type.C:

                break;
        }

        isChase = true; //추적 활성화
        isAttack = false; //공격 하고 있는지 여부(X)
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        Targerting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee") //근접무기와 충돌하면 무기 데미지 만큼 현재 체력을 깎는다
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position; //현재 위치에 피격 위치를 뺴서 반작용 방향 구하기

            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet" || other.tag == "FireBall") //총알과 충돌하면 총알 데미지 만큼 현재 체력을 깎는다
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position; //현재 위치에 피격 위치를 뺴서 반작용 방향 구하기
            Destroy(other.gameObject);


            StartCoroutine(OnDamage(reactVec));
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "DarkMagic")
        {
            hitDelay += Time.deltaTime;   // 트리거에 닿아 있는 동안 시간 누적

            if (hitCool <= hitDelay)      // 쿨타임 1초 넘으면
            {
                Bullet bullet = other.GetComponent<Bullet>();
                curHealth -= bullet.damage;   // 데미지 1회 적용
                hitDelay = 0;                 // 타이머 초기화

                Vector3 reactVec = transform.position - other.transform.position;
                StartCoroutine(OnDamage(reactVec));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "DarkMagic")
            hitDelay = 0f;
    }
    IEnumerator OnDamage(Vector3 reactVec)
    {
        foreach (Renderer r in renders)
        {
            r.material.color = Color.red; // 피격 시 0.1동안 빨간 색으로 바꿈
        }
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0) //죽지 않으면 다시 흰색으로 바꿈
        {
            foreach (Renderer r in renders)
            {
                r.material.color = Color.white;
            }
        }
        else
        {
            foreach (Renderer r in renders)
            {
                r.material.color = Color.gray; //죽으면 색을 그레이로 바꿈
            }
            if (meleeArea != null)
                meleeArea.enabled = false; //
            gameObject.layer = 11; // 레이어를 EnemyDead로 바꿈
            isChase = false;
            nav.enabled = false; //근접 공격 범위 비활성화
            anim.SetTrigger("doDie");

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //넉백 시킴

            Destroy(gameObject, 4f); //4초 후 obj 삭제
        }
    }
}
