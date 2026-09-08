using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GamblingManger : MonoBehaviour
{
    [Header("게임 활성화 전 UI")]
    public GameObject TextUI;
    public GameObject GameUI;
    [Header("게임 활성화 후 UI")]
    public GameObject CardImage;
    public TextMeshProUGUI PriceOverUI;
    public int GameCnt = 0;
    public int BetPrice = 0;
    public int MaxBetPrice;
    public TMP_InputField Input;
    [Header("카드 UI")]
    public bool[] Joker;
    public RectTransform[] Card;
    public GameObject[] Front;
    public GameObject[] Back;
    public Image[] CardFront;
    public Sprite[] FrontImage;
    public Sprite JokerCard;
    [Header("결과 UI")]
    public GameObject SuccesText;
    public GameObject FailText;

    bool isPlaying;
    bool NotClick = true;

    public Player player;

    private void Update()
    {
        if (player == null)
            return;
        MaxBetPrice = player.coin / 2;
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

    void GameStart()
    {
        player.coin -= BetPrice;
        CardImage.SetActive(false);

        int num = Random.Range(0, Joker.Length);
        Joker[num] = true;
    }
    public void OnClickBettingBtn()
    {
        if (!isPlaying && BetPrice > 0)
        {
            isPlaying = true;
            GameCnt++;
            GameStart();
        }
    }
    public void OnClickClose()
    {
        if(!isPlaying)
            GameUI.SetActive(false);
    }
    public void OnEndEditPrice(string str)
    {
        if (!isPlaying)
        {
            int.TryParse(str, out BetPrice);
            if (BetPrice > MaxBetPrice || BetPrice == 0)
            {
                PriceOverUI.color = Color.yellow;
                BetPrice = 0;
                PriceOverUI.DOFade(0f, 5f);
            }
        }
    }

    public void OnClickCard(int i)
    {
        if (isPlaying && NotClick)
        {
            NotClick = false;
            Card[i].DORotate(new Vector3(0, 90, 0), 1f);
            Back[i].SetActive(false);
            Front[i].SetActive(true);
            if (Joker[i])
            {
                CardFront[i].sprite = JokerCard;
                player.coin += BetPrice * 2;
                StartCoroutine("Success");
            }
            else
            {
                int num = Random.Range(0, FrontImage.Length);
                CardFront[i].sprite = FrontImage[num];
                StartCoroutine("Fail");
            }

            Card[i].DORotate(new Vector3(0, 180, 0), 1f);

            Invoke("GameClose", 10f);
        }
    }
    IEnumerator Success()
    {
        yield return new WaitForSeconds(3f);
        SuccesText.SetActive(true);
        yield return new WaitForSeconds(5f);
        SuccesText.SetActive(false);
    }
    IEnumerator Fail()
    {
        yield return new WaitForSeconds(3f);
        FailText.SetActive(true);
        yield return new WaitForSeconds(5f);
        FailText.SetActive(false);
    }
    void GameClose()
    {
        GameUI.SetActive(false);
        isPlaying = false;
        NotClick = true;
        CardImage.SetActive(true);
        for(int i = 0; i<3; i++)
        {
            Card[i].localRotation = Quaternion.identity; //회전 값 초기화
            Back[i].SetActive(true);
            Front[i].SetActive(false);
            Joker[i] = false;
        }
        Input.text = "";
    }
}
