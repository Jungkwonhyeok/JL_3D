using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    public RectTransform MiniMapPos;
    public Vector2 curPos;
    public Sprite VisitedRoom;
    public Image curRoomUI;
    public GameObject[] onRoomUI;
    public GameObject[] offRoomUI;

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            MiniMapPos.anchoredPosition = curPos;
            curRoomUI.sprite = VisitedRoom;
            if (offRoomUI != null)
            {
                foreach (GameObject room in onRoomUI)
                {
                    room.SetActive(true);
                }
            }
            if(offRoomUI != null)
            {
                foreach (GameObject room in offRoomUI)
                {
                    room.SetActive(false);
                }
            }
        }
    }
}
