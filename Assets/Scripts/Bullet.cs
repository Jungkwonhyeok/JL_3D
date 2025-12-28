using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Wall") //벽에 충돌 시 obj 삭제
        {
            Destroy(gameObject);
        }
    }
}
