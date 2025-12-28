using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent <MeshRenderer>().material;
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
        else if(other.tag == "Bullet") //총알과 충돌하면 총알 데미지 만큼 현재 체력을 깎는다
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position; //현재 위치에 피격 위치를 뺴서 반작용 방향 구하기
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red; // 피격 시 0.1동안 빨간 색으로 바꿈
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0) //죽지 않으면 다시 흰색으로 바꿈
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray; //죽으면 색을 그레이로 바꿈
            gameObject.layer = 11; // 레이어를 EnemyDead로 바꿈

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //넉백 시킴

            Destroy(gameObject, 4f); //4초 후 obj 삭제
        }
    }
}
