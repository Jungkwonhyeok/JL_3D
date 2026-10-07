using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryChecker : MonoBehaviour
{
    RoomManager RoomManger;

    private void Awake()
    {
        RoomManger = GetComponentInParent<RoomManager>();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && RoomManger.isCleared == false)
        {
            RoomManger.startRoom();
        }
    }
}
