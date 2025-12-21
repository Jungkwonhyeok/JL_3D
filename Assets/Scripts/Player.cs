using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class HeroChange //캐릭터 바꾸는 class
{
    int num;
    public GameObject[] Prefab;
}

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float moneX;
    float moveZ;

    bool Run;
    bool jump;
    bool interation;
    bool swap1;
    bool swap2;

    bool isJump;
    bool isDodge;
    bool isSwap;

    Vector3 move;
    Vector3 dodge;

    public Rigidbody rigid;
    public Animator anim;

    GameObject nearObject;
    GameObject equipWeapon;
    Renderer render;
    int equipWeaponIndex = -1;

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
        Dodge();
        Swap();
        Interation();
    }
    public void FindWeapons() //캐릭터 교체 시 weapons null 방지용 함수
    {
        List<GameObject> weaponList = new List<GameObject>();

        foreach (Transform child in GetComponentsInChildren<Transform>()) //부모obj + 자식obj + 자식의 자식obj 중 Teg가 Weapon인 obj를 새로운 리스트에 저장
        {
            if (child.CompareTag("Weapon"))
            {
                weaponList.Add(child.gameObject);
            }
        }

        weapons = weaponList.ToArray(); //저장한 obj를 weapons리스트에 저장
    }

    public void GetInput() // 키보드 입력
    {
        moneX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        Run = Input.GetButton("Run");
        jump = Input.GetButtonDown("Jump");
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

        if (isSwap) //움직이며 점프, 무기 전환 시 행동 불가
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

        if (jump && !Run && !isJump && !isDodge && !isSwap) //제자리에서 점프, 움직이며 점프, 무기 전환 시 행동 불가
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
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

        if(SaveitemValue == 0) //기사 무기 설정 값
        {
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
        }
        else if (SaveitemValue == 2 || SaveitemValue == 4) //궁수, 마법사 무기 설정 값
        {
            if (swap1 && !isJump && !isDodge)
            {
                weaponIndex = 0;
                equipWeaponIndex = 0;
            }
            if (swap2 && !isJump && !isDodge)
            {
                return;
            }
        }
        else //도적, 바바리안 무기 설정 값
        {
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
        }
        

        if ((swap1 || swap2) && !isJump && !isDodge) //점프, 회피 중에는 무기 교체 불가
        {
            if(equipWeapon != null) //무기를 들고 있지 않으면 활성화만
            {
                render = equipWeapon.GetComponentInChildren<Renderer>(); 
                render.GetComponentInChildren<Renderer>().enabled = false; //전에 들고 있던 무기 비활성화
            }

            equipWeapon = weapons[weaponIndex]; //현재 들고있는 무기Index값 저장

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
        if(interation && nearObject != null && !isJump)
        {
            if(nearObject.tag == "Weapon")
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
        if(collision.gameObject.tag == "Floor") //땅에 다이면 다시 점프 가능
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
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
