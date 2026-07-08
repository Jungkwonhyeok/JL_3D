using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject[] Items;
    public Transform[] ItemPos;


    private void Awake()
    {
        ItemSpawn();
    }

    void ItemSpawn()
    {
        for(int i = 0; i < 3; i++)
        {
            int num = -1;
            int ran = Random.Range(0, 100);
            if (ran <= 10)
                num = 0;
            else if (ran <= 20)
                num = 1;
            else if (ran <= 30)
                num = 2;
            else if (ran <= 40)
                num = 3;
            else if (ran <= 50)
                num = 4;
            else if (ran <= 75)
                num = 5;
            else if (ran <= 100)
                num = 6;
            Instantiate(Items[num], ItemPos[i]);
        }
    }
}
