using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public int maxHealth; //최대 체력
    public int curHealth; //현재 체력
    public int Eexp;
    public Transform Target; // 추적 할 타겟
    public GameObject bullet; //총알 obj(Enemy C 관련)
    public GameObject DarkSpell;
    public Transform bulletPos; //총알 발사 위치(Enemy C 관련)
    public Transform DarkSpellPos;
    public GameObject Necromancer;
    public GameObject BossUI;
    int AttackCnt = 0;

    bool isChase; //추적하고 있는지 여부
    bool isAttack; //공격중인지
    bool isDead;
    bool isDeathProcessed; // Death 처리가 됐는지 확인 하는 함수

    Rigidbody rigid;
    Renderer[] renders;
    NavMeshAgent nav;
    Animator anim;
    Player player;
    RoomManager RoomManger;

    float hitCool = 1f; // 마법 영역에서 몇초에 1번씩 데미지가 들어 오는지 정
    float hitDelay = 1f; // 마법 영역에서 데미지가 들어오고 얼마나 지났는지 저장하는 변수

    void Awake()
    {
        Target = GameObject.Find("Player").transform;
        rigid = GetComponent<Rigidbody>();
        renders = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = Target.GetComponent<Player>();
        RoomManger = GetComponentInParent<RoomManager>();
    }

    public void ChaseStart() // 추적 시작하는 함수
    {
        BossUI.SetActive(true);
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        if (Player.instance.isStop)
        {
            nav.isStopped = true;      // 에이전트 이동 정지
            nav.velocity = Vector3.zero; // 남은 관성 제거
            return;
        }
        if (nav.enabled) //nav가 활성화 되어있으면
        {
            nav.SetDestination(Target.position); //추적 할 목표물 위치 정하기
            nav.isStopped = !isChase; //추적 중이지 않으면 멈춘다
        }
    }

    void FixedUpdate()
    {
        if (Player.instance.isStop)
            return;
        Targerting();
        FreezVelocity();
    }

    IEnumerator Attack() // 공격하는 코루틴
    {
        isChase = false; //추적 비활성화
        isAttack = true; //공격 하고 있는지 여부(O)


        anim.SetBool("isAttack", true);

        if (AttackCnt < 2)
        {
            if(curHealth <= maxHealth / 2)
            {
                AttackCnt++;
                Necromancer.SetActive(true);
            }
                
            yield return new WaitForSeconds(0.1f);
            GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); //총알을 발사 위치에 생성해줌
            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>(); //생선 된 총알의 rigid를 저장
            rigidBullet.velocity = transform.forward * 100; //정면으로 35만큼 힘을 줌
        }
        else
        {
            yield return new WaitForSeconds(1f);
            AttackCnt = 0;
            GameObject instantMagic = Instantiate(DarkSpell, DarkSpellPos.position, DarkSpellPos.rotation);
            Destroy(instantMagic, 2f);
        }

        yield return new WaitForSeconds(2f);

        isChase = true; //추적 활성화
        isAttack = false; //공격 하고 있는지 여부(X)
        anim.SetBool("isAttack", false);
    }

    void FreezVelocity() // 물리력이 추적을 방해하지 않도록 하는 함수
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.constraints = RigidbodyConstraints.FreezeAll;
        }
        rigid.angularVelocity = Vector3.zero;
    }

    void Targerting() //타겟팅을 위한 함수
    {
        float targetRadius = 0.5f; //SphereCast의 반지름
        float targetRange = 20f; //SphereCast의 길이

        RaycastHit[] raycastHits =
            Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (raycastHits.Length > 0 && !isAttack && curHealth > 0) //cast 범위에 들어오고 공격하고 읶지 않을때
        {
            StartCoroutine(Attack());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead || !RoomManger.isPlayerInRoom)
            return;

        if (other.tag == "Melee") //근접무기와 충돌하면 무기 데미지 만큼 현재 체력을 깎는다
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position; //현재 위치에 피격 위치를 뺴서 반작용 방향 구하기

            if (curHealth <= 0)
                isDead = true;

            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet" || other.tag == "FireBall" || other.tag == "MagicArrow") //총알과 충돌하면 총알 데미지 만큼 현재 체력을 깎는다
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position; //현재 위치에 피격 위치를 뺴서 반작용 방향 구하기

            if (other.tag != "MagicArrow")
                Destroy(other.gameObject);

            if (curHealth <= 0)
                isDead = true;

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
            if (isDeathProcessed) yield break; // 이미 처리됐으면 무시
            isDeathProcessed = true;

            foreach (Renderer r in renders)
            {
                r.material.color = Color.gray; //죽으면 색을 그레이로 바꿈
            }

            player.exp += Eexp;
            player.Killcnt++;
            player.isBossClear = true;

            gameObject.layer = 11; // 레이어를 EnemyDead로 바꿈
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.constraints = RigidbodyConstraints.FreezeRotation;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //넉백 시킴

            RoomManger.OnBossDie();

            Destroy(gameObject, 4f); //4초 후 obj 삭제
        }
    }
}
