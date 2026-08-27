using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class Portal : MonoBehaviour
{
    public GameObject PortalUI;

    void OnTriggerStay(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (other.tag == "Player" && player.nearObject == null)
        {
            PortalUI.SetActive(true);
            
            if (player.nearObject == null)
                player.nearObject = gameObject;
        }

    }

    void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (other.tag == "Player" && player.nearObject != null)
        {
            PortalUI.SetActive(false);
            player.nearObject = null;
        }

    }

    public void NextStage(string stageName)
    {
        SceneManager.LoadScene(stageName);
    }
}
