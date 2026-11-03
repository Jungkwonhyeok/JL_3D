using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] Rewards;
    public GameObject[] curRoomEnemys;
    public GameObject[] exitGates;
    public GameObject ClearPortal;

    public Transform RewardPos;
    public int curEnemyCount;
    public bool isPlayerInRoom = false;
    public bool isCleared = false;

    private void Awake()
    {
        curEnemyCount = curRoomEnemys.Length;
    }

    public void startRoom()
    {
        foreach (GameObject Gates in exitGates)
        {
            Gates.SetActive(true);
            isPlayerInRoom = true;
        }
        
        foreach(GameObject Enemys in curRoomEnemys)
        {
            if(Enemys.GetComponent<Enemy>() == null)
            {
                Enemys.GetComponent<Boss>().ChaseStart();
                return;
            }
            Enemys.GetComponent<Enemy>().ChaseStart();
        }
    }

    public void OnEnemyDie()
    {
        if (gameObject.name == "BossRoom")
            return;
        curEnemyCount--;

        if(curEnemyCount == 0)
        {
            foreach (GameObject Gates in exitGates)
            {
                Gates.SetActive(false);
            }
            isCleared = true;

            RewardsSpawn();
        }
    }

    public void OnBossDie()
    {
        curEnemyCount--;

        if (curEnemyCount == 0)
        {
            foreach (GameObject Gates in exitGates)
            {
                Gates.SetActive(false);
            }
            isCleared = true;

            PortalSpawn();
        }
    }

    void PortalSpawn()
    {
        ClearPortal.SetActive(true);
    }

    void RewardsSpawn()
    {
        int ran = Random.Range(0, 100);
        if (ran < 40)
            Instantiate(Rewards[0], RewardPos.position, RewardPos.rotation);
        else if (ran < 55)
            Instantiate(Rewards[1], RewardPos.position, RewardPos.rotation);
        else if (ran < 95)
            Instantiate(Rewards[2], RewardPos.position, RewardPos.rotation);
        else if (ran < 96)
            Instantiate(Rewards[3], RewardPos.position, RewardPos.rotation);
        else if (ran < 97)
            Instantiate(Rewards[4], RewardPos.position, RewardPos.rotation);
        else if (ran < 98)
            Instantiate(Rewards[5], RewardPos.position, RewardPos.rotation);
        else if (ran < 99)
            Instantiate(Rewards[6], RewardPos.position, RewardPos.rotation);
        else if (ran < 100)
            Instantiate(Rewards[7], RewardPos.position, RewardPos.rotation);
    }
}
