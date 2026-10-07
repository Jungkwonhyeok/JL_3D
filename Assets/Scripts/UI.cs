using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public enum UIType {Coin, MGameStart, MGameNormal}
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
            case UIType.MGameStart:
                if (!Player.instance.nearObject.GetComponent<MiniGameManager>().isPlayGame)
                    myText.text = "게임을 하시겠습니까? [F]";
                else
                    myText.text = "";
                break;
            case UIType.MGameNormal:
                if (!Player.instance.nearObject.GetComponent<MiniGameManager>().isPlayGame)
                    myText.text = "자네는 운명을 믿나?";
                else
                    myText.text = "언젠가 다시 만나지...";
                break;
        }
    }
}
