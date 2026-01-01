using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth; //최대 체력
    public int curHealth; //현재 체력
    public Transform Target; // 추적 할 타겟
    public bool isChase; //추적하고 있는지 여부

    Rigidbody rigid;
    CapsuleCollider capsuleCollider;
    Renderer[] renders;
    NavMeshAgent nav;
    Animator anim;

    float hitCool = 1f; // 마법 영역에서 몇초에 1번씩 데미지가 들어 오는지 정하는 변수
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
        if (isChase)
        {
            nav.SetDestination(Target.position);
        }
    }
    void FreezVelocity() // 물리력이 추적을 방해하지 않도록 하는 함수
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        FreezVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee") //근접무기와 충돌하면 무기 데미지 만큼 현재 체력을 깎는다
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position; //현재 위치에 피격 위치를 뺴서 반작용 방향 구하기

            StartCoroutine(OnDamage(reactVec));
        }
        else if(other.tag == "Bullet" || other.tag == "FireBall") //총알과 충돌하면 총알 데미지 만큼 현재 체력을 깎는다
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
            gameObject.layer = 11; // 레이어를 EnemyDead로 바꿈
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //넉백 시킴

            Destroy(gameObject, 4f); //4초 후 obj 삭제
        }
    }
}
