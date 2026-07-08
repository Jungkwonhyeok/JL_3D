using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryChecker : MonoBehaviour
{
    StageManager stageManger;

    private void Awake()
    {
        stageManger = GetComponentInParent<StageManager>();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && stageManger.isCleared == false)
        {
            stageManger.startStage();
        }
    }
}
