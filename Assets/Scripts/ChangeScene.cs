using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void SceneChange()
    {
        LoadingUIManager.Instance.LoadScene("Lobby");
    }

    public void ClickExit()
    {
        Application.Quit();
    }
}
