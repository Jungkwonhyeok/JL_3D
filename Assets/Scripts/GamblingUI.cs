using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamblingUI : MonoBehaviour
{
    public GameObject TextUI;
    public GameObject GameUI;
    public int GameCnt = 0;

    void OnTriggerStay(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (other.tag == "Player" && player.nearObject == null)
        {
            TextUI.SetActive(true);

            if (player.nearObject == null)
                player.nearObject = gameObject;
        }

    }

    void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (other.tag == "Player" && player.nearObject != null)
        {
            TextUI.SetActive(false);
            player.nearObject = null;
        }

    }
}
