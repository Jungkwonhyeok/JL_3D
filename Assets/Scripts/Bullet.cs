using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee; //근접 무기인지 원거리 무기인지 여부

    public GameObject Explosion; //FireBall 폭발 효과

    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall" && gameObject.tag != "DarkMagic") //근접 무기가 아니고, 흑마법(마법사 Lv3)이 아니면 벽에 충돌 시 obj삭제
        {
            if (gameObject.tag == "FireBall") // 태그가 FireBall이면 폭발 효과를 남김
            {
                GameObject explosion = Instantiate(Explosion,transform.position,transform.rotation);

                Destroy(explosion, 2f); // 2초 후 폭발 효과 삭제
            }

            Destroy(gameObject);
        }
        else if(other.gameObject.layer == 10 && gameObject.tag == "FireBall") // FireBall 관련 같은 내용인데 적과 충돌 했을때
        {
            GameObject explosion = Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(explosion, 2f);
        }
    }
}
