using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
<<<<<<< HEAD
        if (collision.gameObject.tag == "Floor")
=======
        if (other.gameObject.tag == "Wall" && gameObject.tag != "DarkMagic")
>>>>>>> parent of 83208dd (데쉬 하는 적 구현(수정 필요))
        {
            Destroy(gameObject, 3);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
