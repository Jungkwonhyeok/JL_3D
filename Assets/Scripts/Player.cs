using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class HeroChange // 캐릭터 교체에 사용되는 클래스
{
    int num;
    public GameObject[] Prefab; // 각 캐릭터 프리팹 배열
}

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public GameObject[] weapons; // 현재 캐릭터가 보유한 무기 오브젝트들
    public bool[] hasWeapons; // 무기(=캐릭터) 보유 여부

    public int ammo; //화살 개수
    public int coin; //코인 개수
    public int health; //체력
    public int maxAmmo; //최대 화살 수
    public int maxCoin; //최대 코인 수
    public int maxHealth; //최대 체력

    float moneX;
    float moveZ;

    bool Run;
    bool jump;
    bool fire1;
    bool interation;
    bool swap1;
    bool swap2;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;

    Vector3 move;
    Vector3 dodge;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    Weapon equipWeapon;
    Renderer render;
    int equipWeaponIndex = -1;
    float fireDelay;

    // 기사 = 0 , 도적 = 1, 궁수 = 2, 바바리안 = 3, 마법사 = 4
    public HeroChange HeroChanges;
    public int SaveitemValue;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
    public void Update()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Dodge();
        Swap();
        Interation();
    }
    public void FindWeapons() // 캐릭터 교체 후 weapons 배열이 null 되는 것을 방지
    {
        List<GameObject> weaponList = new List<GameObject>();

        foreach (Transform child in GetComponentsInChildren<Transform>()) // 자신 + 자식 + 자식의 자식 중 Tag가 Weapon인 오브젝트를 찾음
        {
            if (child.CompareTag("Weapon")|| child.CompareTag("Melee"))
            {
                weaponList.Add(child.gameObject);
            }
        }

        weapons = weaponList.ToArray(); // 찾은 무기들을 배열로 저장
    }

    public void GetInput() // 키보드 입력
    {
        moneX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        Run = Input.GetButton("Run");
        jump = Input.GetButtonDown("Jump");
        fire1 = Input.GetButtonDown("Fire1");
        interation = Input.GetButtonDown("Interation");
        swap1 = Input.GetButtonDown("Swap1");
        swap2 = Input.GetButtonDown("Swap2");
    }

    public void Move() //움직이는 함수
    {
        move = new Vector3(moneX, 0, moveZ).normalized;

        if (isDodge)
        {
            move = dodge;

        }

        if (isSwap || !isFireReady) //움직이며 점프, 무기 전환, 공격 중 일시 행동 불가
            move = Vector3.zero;

        transform.position += move * speed * (!Run ? 0.6f : 1f) * Time.deltaTime;

        if (anim != null)
        {
            anim.SetBool("isWalk", move != Vector3.zero);
            anim.SetBool("isRun", Run);
        }


    }

    public void Turn() //회전
    {
        float rotationSpeed = 15f; //플레이어 방향 회전 속도

        if (move != Vector3.zero) //플레이어가 방향 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void Jump() //점프
    {

        if (jump && !Run && !isJump && !isDodge && !isSwap && isFireReady) //제자리에서 점프, 움직이며 점프, 무기 전환 시 행동 불가
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack() //공격 하는 함수
    {
        if (equipWeapon == null) //무기를 들고 있지 않으면 return
            return;

        fireDelay += Time.deltaTime; //공격 키를 느르고 얼마나 지났는지 값을 입력
        isFireReady = equipWeapon.rate < fireDelay; //공격 딜레이 < 공격 후 지난 시간 으로 공격 가능 여부 확인

        if (fire1 && isFireReady && !isDodge && !isSwap) //공격이 가능 하고, 회피, 무기 교체 상태가 아닐 시 공격
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doCross");
            fireDelay = 0;
        }
    }

    public void Dodge() //회피
    {

        if (jump && move != Vector3.zero && Run && !isJump && !isDodge && !isSwap) //움직이며 점프, 무기 전환 시 행동 불가
        {
            dodge = move;
            speed *= 2f;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.6f); //0.6초 뒤 원상복귀
        }
    }

    void DodgeOut() //회피에서 원 상태로
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap() //무기 교체 시켜주는 함수
    {
        if (swap1 && equipWeaponIndex == 0) //이미 들고 있는 무기 들려하면 return (아래도 같음)
            return;
        if (swap2 && equipWeaponIndex == 1)
            return;

        int weaponIndex = -1; //배열은 0부터 시작하므로 -1 할당

        switch (SaveitemValue)
        {
            case 0: //기사
                if (swap1 && !isJump && !isDodge)
                {
                    weaponIndex = 0;
                    equipWeaponIndex = 0;
                }
                if (swap2 && !isJump && !isDodge)
                {
                    weaponIndex = 3;
                    equipWeaponIndex = 1;
                }
                break;
            case 1: //도적
                if (swap1 && !isJump && !isDodge)
                {
                    weaponIndex = 0;
                    equipWeaponIndex = 0;
                }
                if (swap2 && !isJump && !isDodge)
                {
                    weaponIndex = 2;
                    equipWeaponIndex = 1;
                }
                break;
            case 2: //궁수
                if (swap1 && !isJump && !isDodge)
                {
                    weaponIndex = 0;
                    equipWeaponIndex = 0;
                }
                if (swap2 && !isJump && !isDodge)
                {
                    return;
                }
                break;
            case 3: //바바리안
                if (swap1 && !isJump && !isDodge)
                {
                    weaponIndex = 0;
                    equipWeaponIndex = 0;
                }
                if (swap2 && !isJump && !isDodge)
                {
                    weaponIndex = 2;
                    equipWeaponIndex = 1;
                }
                break;
            case 4: //마법사
                if (swap1 && !isJump && !isDodge)
                {
                    weaponIndex = 0;
                    equipWeaponIndex = 0;
                }
                if (swap2 && !isJump && !isDodge)
                {
                    return;
                }
                break;
        }




        if ((swap1 || swap2) && !isJump && !isDodge) //점프, 회피 중에는 무기 교체 불가
        {
            if (equipWeapon != null) //무기를 들고 있지 않으면 활성화만
            {
                render = equipWeapon.GetComponentInChildren<Renderer>();
                render.GetComponentInChildren<Renderer>().enabled = false; //전에 들고 있던 무기 비활성화
            }

            equipWeapon = weapons[weaponIndex].GetComponentInChildren<Weapon>(); //현재 들고있는 무기Index값 저장

            render = equipWeapon.GetComponentInChildren<Renderer>();
            render.GetComponentInChildren<Renderer>().enabled = true; //Swap에 따른 무기 활성화

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.5f); //원상태로 복귀
        }
    }

    void SwapOut() //원상태로 복귀 시켜준는 함수
    {
        isSwap = false;
    }

    void Interation() //상호작용(아이템 먹음)
    {
        if (interation && nearObject != null && !isJump)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //아이템 value값을 무기 인덱스로 저장

                for (int i = 0; i < hasWeapons.Length; i++) //먹은 아이템 외 비활성화
                {
                    hasWeapons[i] = false;
                }

                hasWeapons[weaponIndex] = true; //먹은 아이템 활성화

                SaveitemValue = weaponIndex; //먹은 아이템 Value값 저장

                Destroy(nearObject); //아이템 obj 삭제

                Change(); //캐릭터 바꿈

                Invoke("FindWeapons", 0.1f);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") //땅에 다이면 다시 점프 가능
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>(); //가까이 가면 tag가 Item인 아이템 먹음
            switch (item.type)
            {
                case Item.Type.Ammo: //타입이 Ammo이면 화살 개수에 아이템(ammo) 값 더해줌, MAX 값이면 그대로 유지
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin: //타입이 Coin이면 코인 개수에 아이템(coin) 값 더해줌, MAX 값이면 그대로 유지
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart: //타입이 Heart이면 체력에 아이템(health) 값 더해줌, MAX 값이면 그대로 유지
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
            }
            Destroy(other.gameObject); //먹은 obj는 삭제
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject; //가까이 가면 nearobj에 아이템 obj를  저장
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null; //멀어지면 nearobj를 비움

    }

    public void HeroChange(int num) //바꿀 캐릭터 저장해주는 함수
    {
        foreach (Transform child in transform) //부모obj 아래 자식obj를 모두 삭제함
        {
            Destroy(child.gameObject);
        }

        Instantiate(HeroChanges.Prefab[num], transform.position, transform.rotation, transform); //현obj에 캐릭터 오브제를 추가함

        HeroReset();
    }

    void HeroReset() //캐릭터 교체 시 값들 초기화 시키는 함수
    {
        equipWeaponIndex = -1;
    }

    public void Change() //캐릭터 바꾸는 함수
    {
        if (hasWeapons[0] == true)
        {
            HeroChange(0); //기사
        }
        else if (hasWeapons[1] == true)
        {
            HeroChange(1); //도적
        }
        else if (hasWeapons[2] == true)
        {
            HeroChange(2); //궁수
        }
        else if (hasWeapons[3] == true)
        {
            HeroChange(3); //바바리안
        }
        else if (hasWeapons[4] == true)
        {
            HeroChange(4); //마법사
        }
    }

}