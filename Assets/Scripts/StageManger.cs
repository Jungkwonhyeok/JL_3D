using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManger : MonoBehaviour
{
    public GameObject[] curStageEnemys;
    public GameObject[] exitGates;

    public int curEnemyCount;
    public bool isPlayerInStage = false;
    public bool isCleared = false;

    private void Awake()
    {
        curEnemyCount = curStageEnemys.Length;
    }

    public void startStage()
    {
        foreach (GameObject Gates in exitGates)
        {
            Gates.SetActive(true);
            isPlayerInStage = true;
        }
        
        foreach(GameObject Enemys in curStageEnemys)
        {
            Enemys.GetComponent<Enemy>().ChaseStart();
        }
    }

    public void OnEnemyDie()
    {
        curEnemyCount--;

        if(curEnemyCount == 0)
        {
            foreach (GameObject Gates in exitGates)
            {
                Gates.SetActive(false);
            }
            isCleared = true;
        }
    }
}
