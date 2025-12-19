using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool isJump;
    bool isDodge;

    Vector3 move;
    Vector3 dodge;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
    public void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interation();
    }

    public void GetInput() // 키보드 입력
    {
        moneX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        Run = Input.GetButton("Run");
        jump = Input.GetButton("Jump");
        interation = Input.GetButton("Interation");
    }

    public void Move() //움직이는 함수
    {
        move = new Vector3(moneX, 0, moveZ).normalized;

        if (isDodge)
        {
            move = dodge;

        }

        transform.position += move * speed * (!Run ? 0.6f : 1f) * Time.deltaTime;

        anim.SetBool("isWalk", move != Vector3.zero);
        anim.SetBool("isRun", Run);

        
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

        if (jump && !Run && !isJump && !isDodge) //제자리에서 점프
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    public void Dodge() //회피
    {

        if (jump && move != Vector3.zero && Run && !isJump && !isDodge) //움직이며 점프
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

    void Interation() //상호작용
    {
        if(interation && nearObject != null && !isJump)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //아이템 value값을 무기 인덱스로 저장
                hasWeapons[weaponIndex] = true; //무기 보유중인지 여부

                Destroy(nearObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
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
}
