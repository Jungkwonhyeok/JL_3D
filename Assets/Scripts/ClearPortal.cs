using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearPortal : MonoBehaviour
{
    public GameObject PortalUI;
    public GameObject ResultUI;

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

    public void OnResult()
    {
        ResultUI.SetActive(true);
        Player.instance.isStop=true;
    }
}