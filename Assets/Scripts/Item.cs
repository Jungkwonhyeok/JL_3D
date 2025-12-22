using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {Weapon, Ammo, Coin, Heart};

    public Type type;
    public int value;

    void Update()
    {
        transform.Rotate(Vector3.up * 20 *  Time.deltaTime); //아이템 obj 회전
    }
}
