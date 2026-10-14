using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject characterListPanel;

    [Header("Scene")]
    [SerializeField] private string dungeonSceneName = "Dungeon";

    // 설정 버튼
    public void OpenSetting() => settingPanel.SetActive(true);
    public void CloseSetting() => settingPanel.SetActive(false);

    // 캐릭터 목록 버튼
    public void OpenCharacterList() => characterListPanel.SetActive(true);
    public void CloseCharacterList() => characterListPanel.SetActive(false);

    // 던전 입장 버튼
    public void EnterDungeon()
    {
        SceneManager.LoadScene(3);
    }

    // 시작 화면 돌아가기 버튼
    public void BackStart()
    {
        SceneManager.LoadScene(0);
    }
}
