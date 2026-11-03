using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Result : MonoBehaviour
{
    public enum UIType {Kill, KillPoint, Time, TimePoint, Hit, HitPoint, Score, Tire, HeroIcon}
    public UIType Type;

    public GameObject[] Tire;
    public GameObject[] HeroIcon;

    TextMeshProUGUI myText;

    public int score;

    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        int K = Player.instance.Killcnt * 50;

        int T = (1200 - (int)Player.instance.ClearTime) * 3;
        if (1200 < Player.instance.ClearTime)
            T = 0;

        int H = Player.instance.Hitcnt * 3;

        score = K + T - H;

        switch (Type)
        {
            case UIType.Kill:
                int kill = Player.instance.Killcnt;
                myText.text = string.Format("{0:n0}¸¶¸®", kill);
                break;
            case UIType.KillPoint:
                int killpoint = Player.instance.Killcnt * 50;
                myText.text = string.Format("+{0:n0}", killpoint);
                break;
            case UIType.Time:
                int M = (int)(Player.instance.ClearTime/60);
                int S = (int)(Player.instance.ClearTime % 60); ;
                myText.text = string.Format("{0:00}:{1:00}", M,S);
                break;
            case UIType.TimePoint:
                int timepoint = (1200-(int)Player.instance.ClearTime) * 3;
                if(1200 < Player.instance.ClearTime)
                    timepoint = 0;
                myText.text = string.Format("+{0:n0}", timepoint);
                break;
            case UIType.Hit:
                int hit = Player.instance.Hitcnt;
                myText.text = string.Format("{0:n0}È¸", hit);
                break;
            case UIType.HitPoint:
                int hitpoint = Player.instance.Hitcnt * 3;
                myText.text = string.Format("-{0:n0}", hitpoint);
                break;
            case UIType.Score:
                myText.text = string.Format("{0:n0}", score);
                break;
            case UIType.HeroIcon:
                SetHeroIcon();
                break;
            case UIType.Tire:
                SetTire();
                break;
        }
    }

    void SetTire()
    {
        if (score >= 5000)
            Tire[0].SetActive(true);
        else if(score >= 4500)
            Tire[1].SetActive(true);
        else if (score >= 4000)
            Tire[2].SetActive(true);
        else
            Tire[3].SetActive(true);
    }

    void SetHeroIcon()
    {
        for (int num = 0; num < HeroIcon.Length;  num++)
        {
            if (Player.instance.hasWeapons[num])
            {
                HeroIcon[num].SetActive(true);
            }
        }
    }
}
