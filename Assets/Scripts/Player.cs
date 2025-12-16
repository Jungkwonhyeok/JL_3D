using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPower;

    float moneX;
    float moveZ;
    bool Run;
    bool jump;

    bool isJump;

    Vector3 move;

    Rigidbody rigid;
    Animator anim;

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
    }

    public void GetInput() // 키보드 입력
    {
        moneX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        Run = Input.GetButton("Run");
        jump = Input.GetButton("Jump");

    }

    public void Move() //움직이는 함수
    {
        move = new Vector3(moneX, 0, moveZ).normalized;

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

        if (jump && !isJump)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
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
}
