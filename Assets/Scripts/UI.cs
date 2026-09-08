using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public enum UIType {Coin, MaxBeting, MGameCnt}
    public UIType type;

    TextMeshProUGUI myText;

    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case UIType.Coin:
                myText.text = string.Format("{0:n0}",Player.instance.coin);
                break;
            case UIType.MaxBeting:
                int Max = Player.instance.coin / 2;
                myText.text = string.Format($"{Max:n0}");
                break;
            case UIType.MGameCnt:
                int GameCnt = Player.instance.nearObject.GetComponent<GamblingManger>().GameCnt;
                myText.text = string.Format($"({GameCnt:n0}/2)");
                break;
        }
    }
}
