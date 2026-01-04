using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public GameObject Explosion; //FireBall 폭발 효과

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" && gameObject.tag != "DarkMagic") //
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
