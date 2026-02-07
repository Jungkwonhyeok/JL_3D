using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class HeroChange // 캐릭터 교체에 사용되는 데이터 클래스 (프리팹 묶음용)
{
    public GameObject[] Prefab; // 각 캐릭터 프리팹 배열
}

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public GameObject[] weapons; // 현재 캐릭터가 보유 중인 무기 오브젝트 배열
    public bool[] hasWeapons; // 무기 보유 여부 (캐릭터 선택 판단용)
    public Camera followCamera; // 조준 및 회전에 사용하는 카메라

    public int ammo; // 현재 화살 개수
    public int coin; // 현재 코인 개수
    public int health; // 현재 체력
    public int maxAmmo; // 최대 화살 수
    public int maxCoin; // 최대 코인 수
    public int maxHealth; // 최대 체력
    public int exp; // 경험치
    public int level = 1; // 플레이어 레벨

    float moneX;
    float moveZ;

    bool Run;
    bool jump;
    bool reload;
    bool fire1;
    bool interation;
    bool swap1;
    bool swap2;

    bool isJump; // 점프 상태 여부
    bool isDodge; // 회피 상태 여부
    bool isSwap; // 무기 교체 중 여부
    bool isFireReady = true; // 공격 쿨타임이 끝났는지 여부
    bool isReload; // 재장전 중 여부
    bool isBorder; // 벽에 막혀 있는지 여부
    bool isDamage; //적에게 공격 당하고 있는지 여부

    Vector3 move;
    Vector3 dodge;

    Rigidbody rigid;
    Animator anim;
    Renderer[] renders;

    GameObject nearObject; // 근처에 있는 상호작용 대상
    Weapon equipWeapon; // 현재 장착 중인 무기 스크립트
    Renderer render; // 무기 렌더러 제어용
    int equipWeaponIndex = -1; // 현재 무기 인덱스 (-1 = 장착 없음)
    float fireDelay; // 공격 딜레이 타이머

    // 기사 = 0 , 도적 = 1, 궁수 = 2, 바바리안 = 3, 마법사 = 4
    public HeroChange HeroChanges;
    public int SaveitemValue; // 현재 선택된 캐릭터(무기) 인덱스 저장용

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>(); // 최초 Animator 참조
        renders = GetComponentsInChildren<Renderer>();
    }

    void OnTransformChildrenChanged() //캐릭터가 교체 됐을 떄 null 방지
    {
        renders = GetComponentsInChildren<Renderer>();
        anim = GetComponentInChildren<Animator>();
    }
    public void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Reload();
        Dodge();
        Swap();
        Attack();
        Interation();
        LevelUp();
    }

    public void FindWeapons() // 캐릭터 교체 후 무기 배열을 다시 구성하는 함수

    {
        List<GameObject> weaponList = new List<GameObject>();

        foreach (Transform child in GetComponentsInChildren<Transform>()) // 자신 + 모든 하위 오브젝트 순회
        {
            if (child.CompareTag("Weapon") || child.CompareTag("Melee") || child.CompareTag("Shield")) // 무기, 근접, 쉴드 태그만 수집
            {
                weaponList.Add(child.gameObject);
            }
        }

        weapons = weaponList.ToArray(); // 리스트 → 배열 변환
    }

    public void GetInput() // 키보드 / 마우스 입력 처리
    {
        moneX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        Run = Input.GetButton("Run");
        jump = Input.GetButtonDown("Jump");
        reload = Input.GetButtonDown("Reload");
        fire1 = Input.GetButtonDown("Fire1");
        interation = Input.GetButtonDown("Interation");
        swap1 = Input.GetButtonDown("Swap1");
        swap2 = Input.GetButtonDown("Swap2");
    }

    public void Move() // 이동 처리
    {
        move = new Vector3(moneX, 0, moveZ).normalized;

        if (isDodge) // 회피 중에는 방향 고정
        {
            move = dodge;
        }

        if (isSwap || isReload || !isFireReady || (equipWeapon != null && equipWeapon.isShield)) // 특정 상태 중 이동 제한
            move = Vector3.zero;

        if (!isBorder) // 벽에 막히지 않았을 때만 이동
            transform.position += move * speed * (!Run ? 0.6f : 1f) * Time.deltaTime;

        if (anim != null)
        {
            anim.SetBool("isWalk", move != Vector3.zero);
            anim.SetBool("isRun", Run);
        }
    }

    public void Turn() // 캐릭터 회전 처리
    {
        float rotationSpeed = 15f;

        if (move != Vector3.zero) // 이동 방향 기준 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (fire1 && !isJump && !isReload && !isDodge && (equipWeapon != null && equipWeapon.curAmmo != 0)) // 공격 중 조준 회전
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0f;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    public void Jump() // 점프 처리
    {
        if (jump && !Run && !isJump && !isDodge && !isSwap && isFireReady)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack() // 공격 처리
    {
        // 무기가 없으면 공격 불가
        if (equipWeapon == null) 
        {
            fireDelay = 10;
            return;
        }

        // 공격 쿨타임 계산
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate <= fireDelay;

        if (fire1 && isFireReady && !isDodge && !isSwap && !isReload)
        {
            // 원거리 무기인데 탄약이 없으면 공격 불가
            if (equipWeapon.type == Weapon.Type.Range && equipWeapon.curAmmo <= 0)
                return;

            equipWeapon.Use(); // 무기 사용

            // 무기 타입에 따른 애니메이션
            if (equipWeapon.type != Weapon.Type.Shield)
                anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee || equipWeapon.type == Weapon.Type.Magic ? "doSwing" : "doShot");

            else
            {
                anim.SetBool("doShield", !equipWeapon.isShield);
            }
            fireDelay = 0;
        }
    }

    void Reload() // 재장전 처리
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type != Weapon.Type.Range)
            return;

        if (reload && !isJump && !isSwap && isFireReady && ammo != 0 && equipWeapon.curAmmo != equipWeapon.maxAmmo && !isReload)
        {
            isReload = true;
            anim.SetTrigger("doReload");
            Invoke("ReloadOut", 1f);
        }
    }

    void ReloadOut() // 재장전 완료 처리
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    public void Dodge() // 회피 처리
    {
        if (jump && move != Vector3.zero && Run && !isJump && !isDodge && !isSwap)
        {
            dodge = move;
            speed *= 2f;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.6f);
        }
    }

    void DodgeOut() // 회피 종료 후 상태 복구
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap() // 무기 교체 처리
    {
        if (swap1 && equipWeaponIndex == 0)
            return;
        if (swap2 && equipWeaponIndex == 1)
            return;

        int weaponIndex = -1;

        if (!isJump && !isDodge && !(equipWeapon != null && equipWeapon.isShield))
        {
            switch (SaveitemValue) // 현재 캐릭터 타입에 따른 무기 분기
            {
                case 0: // 기사
                    if (swap1)
                    {
                        weaponIndex = (level == 1 ? 0 : level == 2 ? 1 : level == 3 ? 2 : 2);
                        equipWeaponIndex = 0;
                    }
                    if (swap2)
                    {
                        if (level == 1)
                            return;

                        weaponIndex = (level == 2 ? 3 : level == 3 ? 4 : 4);
                        equipWeaponIndex = 1;
                    }
                    break;
                case 1: // 도적
                    if (swap1)
                    {
                        weaponIndex = (level == 1 ? 0 : level == 2 ? 1 : 1);
                        equipWeaponIndex = 0;
                    }
                    if (swap2)
                    {
                        if (level == 1 || level == 2)
                            return;

                        weaponIndex = 2;
                        equipWeaponIndex = 1;
                    }
                    break;
                case 2: // 궁수
                    if (swap1)
                    {
                        weaponIndex = (level == 1 ? 0 : level == 2 ? 1 : 1);
                        equipWeaponIndex = 0;
                    }
                    if (swap2)
                    {
                        return;
                    }
                    break;
                case 3: // 바바리안
                    if (swap1)
                    {
                        weaponIndex = (level == 1 ? 0 : level == 2 ? 1 : 1);
                        equipWeaponIndex = 0;
                    }
                    if (swap2)
                    {
                        if (level == 1 || level == 2)
                            return;
                        weaponIndex = 2;
                        equipWeaponIndex = 1;
                    }
                    break;
                case 4: // 마법사
                    if (swap1)
                    {
                        weaponIndex = (level == 1 ? 0 : level == 2 ? 1 : level == 3 ? 2 : 2);
                        equipWeaponIndex = 0;
                    }
                    if (swap2)
                    {
                        return;
                    }
                    break;
            }

            if ((swap1 || swap2))
            {
                if (equipWeapon != null)
                {
                    render = equipWeapon.GetComponentInChildren<Renderer>();
                    render.GetComponentInChildren<Renderer>().enabled = false;
                }
                equipWeapon = weapons[weaponIndex].GetComponentInChildren<Weapon>();

                render = equipWeapon.GetComponentInChildren<Renderer>();
                render.GetComponentInChildren<Renderer>().enabled = true;

                anim.SetTrigger("doSwap");

                isSwap = true;

                Invoke("SwapOut", 0.5f);
            }
        }
    }

    void SwapOut() // 무기 교체 종료
    {
        isSwap = false;
    }

    void Interation() // 무기 아이템 획득 처리
    {
        if (interation && nearObject != null && !isJump)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;

                for (int i = 0; i < hasWeapons.Length; i++)
                {
                    hasWeapons[i] = false;
                }

                hasWeapons[weaponIndex] = true;
                SaveitemValue = weaponIndex;

                Destroy(nearObject);

                Change();

                Invoke("FindWeapons", 0.1f);
            }
        }
    }

    void LevelUp()
    {
        if(exp == 100)
        {
            level++;
            exp = 0;
            equipWeaponIndex = -1;
        }
    }

    void FreezRotation() // 물리 회전 방지
    {
        rigid.angularVelocity = Vector3.zero;

        if (equipWeapon != null && equipWeapon.isShield)
        {
            rigid.velocity = Vector3.zero;
        }
    }

    void StopToWall() // 전방 벽 충돌 감지
    {
        isBorder = Physics.Raycast(transform.position, transform.forward, 3, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        FreezRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
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
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if(!isDamage) //공격을 당하지 않았을떄
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                if(other.GetComponent<Rigidbody>() != null) //적의 총알을 맞으면 총알 obj 삭제 시켜줌
                    Destroy(other.gameObject);

                StartCoroutine(OnDamage());
            }
        }
    }

    IEnumerator OnDamage() //공격 받을때 코루틴
    {
        isDamage = true;
        foreach(Renderer r in renders)
        {
            r.material.color = Color.red; //mtr을 빨간색으로 바꿈
        }

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (Renderer r in renders)
        {
            r.material.color = Color.white; //mtr을 흰색으로 바꿈
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }

    public void HeroChange(int num) // 캐릭터 프리팹 교체
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Instantiate(HeroChanges.Prefab[num], transform.position, transform.rotation, transform);

        HeroReset();
    }

    void HeroReset() // 캐릭터 교체 후 상태 초기화
    {
        equipWeaponIndex = -1;
    }

    public void Change() // 보유 무기에 따른 캐릭터 변경
    {
        if (hasWeapons[0] == true)
        {
            HeroChange(0);
        }
        else if (hasWeapons[1] == true)
        {
            HeroChange(1);
            ammo += 50;
            if (ammo >= maxAmmo)
                ammo = maxAmmo;
        }
        else if (hasWeapons[2] == true)
        {
            HeroChange(2);
            ammo += 50;
            if (ammo >= maxAmmo)
                ammo = maxAmmo;
        }
        else if (hasWeapons[3] == true)
        {
            HeroChange(3);
        }
        else if (hasWeapons[4] == true)
        {
            HeroChange(4);
        }
    }
}
