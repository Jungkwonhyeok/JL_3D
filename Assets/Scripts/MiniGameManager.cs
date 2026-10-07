using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    [Header("게임 활성화 전 UI")]
    public GameObject TextUI;
    public GameObject GameUI;
    [Header("게임 활성화 후 UI")]
    public GameObject SemiTitle;
    public GameObject HeroChoice;
    public GameObject CardObj;
    public TextMeshProUGUI OKText;
    public Image[] HeroBtn;
    [Header("카드 UI")]
    public bool[] isSuccess;
    public RectTransform[] Card; //카드 회전에 필요함
    public GameObject[] Front;
    public GameObject[] Back;
    public Image[] CardFront;
    public Sprite FailImage;
    public Sprite SuccessImage;
    [Header("직업 선택 UI 색상")]
    public Color Normal;
    public Color[] HeroColor;

    public int Heroindex = -1;

    public bool isPlayGame = false;
    bool NotClick = true;

    public Player player;

    private void Update()
    {
        if (player == null)
            return;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player") return;

        player = other.GetComponent<Player>();
        if (other.tag == "Player" && player.nearObject == null)
        {
            TextUI.SetActive(true);

            if (player.nearObject == null)
                player.nearObject = gameObject;
        }

    }

    void OnTriggerExit(Collider other)
    {
        player = other.GetComponent<Player>();
        if (other.tag == "Player" && player.nearObject != null)
        {
            TextUI.SetActive(false);
            player.nearObject = null;
        }

    }

    public void GameStart()
    {
        int num = Random.Range(0, isSuccess.Length);
        isSuccess[num] = true;
    }
    public void OnClickClose()
    {
        if(!isPlayGame)
            GameUI.SetActive(false);
    }

    public void OnClickCard(int i)
    {
        isPlayGame = true;

        if (isPlayGame && NotClick)
        {
            NotClick = false;
            Card[i].DORotate(new Vector3(0, 90, 0), 1f);
            Back[i].SetActive(false);
            Front[i].SetActive(true);
            if (isSuccess[i])
            {
                CardFront[i].sprite = SuccessImage;
                StartCoroutine("Success");
            }
            else
            {
                CardFront[i].sprite = FailImage;
                StartCoroutine("Fail");
            }

            Card[i].DORotate(new Vector3(0, 180, 0), 1f);
        }
    }
    IEnumerator Success()
    {
        yield return new WaitForSeconds(5f);
        CardObj.SetActive(false);
        SemiTitle.SetActive(false);
        HeroChoice.SetActive(true);
    }
    IEnumerator Fail()
    {
        yield return new WaitForSeconds(5f);
        GameClose();
    }
    void GameClose()
    {
        GameUI.SetActive(false);
    }

    public void OnclickHeroBtn(int num)
    {
        for(int i=0; i < HeroBtn.Length; i++)
        {
            HeroBtn[i].color = Normal;
        }
        HeroBtn[num].color = HeroColor[num];
        OKText.color = HeroColor[num];

        Heroindex = num;
    }

    public void OnClickokBtn()
    {
        for (int i = 0; i < player.hasWeapons.Length; i++)
        {
            player.hasWeapons[i] = false;
        }
        player.hasWeapons[Heroindex] = true;
        player.SaveitemValue = Heroindex;

        player.Change();

        Invoke("FindWeapon", 0.1f);
        GameClose();
    }

    void FindWeapon()
    {
        player.FindWeapons();
    }
}
