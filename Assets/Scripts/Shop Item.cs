using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public enum Type { Knight, Ranger, Rogue, Barbarian, Mage, Ammo, Heart};

    public Type type;
    public int Price;
    public GameObject ItemUI;
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime); //아이템 obj 회전
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            ItemUI.SetActive(true);
            Player player = other.GetComponent<Player>();
            if(player.nearSItem == null)
                player.nearSItem = gameObject;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ItemUI.SetActive(false);
            Player player = other.GetComponent<Player>();
            player.nearSItem = null;
        }

    }
}
