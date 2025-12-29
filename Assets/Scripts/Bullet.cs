using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public GameObject Explosion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            if (gameObject.tag == "FireBall")
            {
                GameObject explosion = Instantiate(Explosion,transform.position,transform.rotation);

                Destroy(explosion, 2f);
            }

            Destroy(gameObject);
        }
        else if(other.gameObject.layer == 10 && gameObject.tag == "FireBall")
        {
            GameObject explosion = Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(explosion, 2f);
        }
    }
}
