using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public enum UIType { Coin, MGameStart, MGameNormal, P_HP, P_HP_Text, Exp, Exp_Text, Lv, CurAmmo, MaxAmmo, PlayerIcon, Button, Slot1, Slot2, B_HP}
    public UIType type;
    [Header("PlayerIcon용")]
    public GameObject[] PlayerIcon;
    [Header("Bullet(Button)용")]
    public GameObject BulletUI;
    [Header("Slot1, Slot2용")]
    public GameObject[] SlotIcon;
    public GameObject OpenUI;
    public GameObject LockUI;
    [Header("Button용")]
    public GameObject StopUI;
    public GameObject GameOver;
    TextMeshProUGUI myText;
    Slider mySlider;


    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case UIType.Coin:
                myText.text = string.Format("{0:n0}", Player.instance.coin);
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
            case UIType.Lv:
                myText.text = string.Format("{0:n0}", Player.instance.level);
                break;
            case UIType.MaxAmmo:
                myText.text = string.Format("{0:n0}", Player.instance.ammo);
                break;
            case UIType.CurAmmo:
                int ammo = 0;
                if (Player.instance.equipWeapon != null)
                    ammo = Player.instance.equipWeapon.curAmmo;

                    myText.text = string.Format("{0:n0}", ammo);
                break;
            case UIType.P_HP:
                float Curhealth = Player.instance.health;
                float Maxhealth = Player.instance.maxHealth;
                mySlider.value = Curhealth / Maxhealth;
                break;
            case UIType.Exp:
                float CurExp = Player.instance.exp;
                mySlider.value = CurExp / 100f;
                break;
            case UIType.P_HP_Text:
                int cur = Player.instance.health;
                int max = Player.instance.maxHealth;
                myText.text = string.Format("{0:n0}/{1:n0}", cur, max);
                break;
            case UIType.Exp_Text:
                int Exp = Player.instance.exp;
                myText.text = string.Format("{0:n0}/100", Exp);
                break;
            case UIType.Slot1:
                SetSlot1();
                break;
            case UIType.Slot2: 
                SetSlot2();
                break;
            case UIType.B_HP:
                Boss boss = GameObject.Find("Boss").GetComponent<Boss>();
                if (boss == null) 
                    return;
                float B_curhp = boss.curHealth;
                float B_maxhp = boss.maxHealth;
                mySlider.value = B_curhp/B_maxhp;
                break;
        }
    }

    void SetSlot1()
    {
        for(int i=0; i < SlotIcon.Length; i++)
        {
            if (Player.instance.hasWeapons[i])
            {
                SlotIcon[i].SetActive(true);
            }
            else
                SlotIcon[i].SetActive(false);
        }
    }
    void SetSlot2()
    {
        if (Player.instance.hasWeapons[0] && Player.instance.level >= 2)
        {
            OpenUI.SetActive(true);
            SlotIcon[1].SetActive(true);
            LockUI.SetActive(false);
        }
        else if (Player.instance.hasWeapons[1] && Player.instance.level >= 3)
        {
            OpenUI.SetActive(true);
            SlotIcon[0].SetActive(true);
            LockUI.SetActive(false);
        }
        else if (Player.instance.hasWeapons[3] && Player.instance.level >= 3)
        {
            OpenUI.SetActive(true);
            SlotIcon[1].SetActive(true);
            LockUI.SetActive(false);
        }
        else
        {
            OpenUI.SetActive(false);
            LockUI.SetActive(true);
        }
    }
    public void SetPlayerIcon()
    {
        for (int num = 0; num < PlayerIcon.Length; num++)
        {
            if (Player.instance.hasWeapons[num])
            {
                PlayerIcon[num].SetActive(true);
            }
            else
                PlayerIcon[num].SetActive(false);
        }
    }

    public void OnBulletUI()
    {
        BulletUI.SetActive(true);
    }
    public void OffBulletUI()
    {
        BulletUI.SetActive(false);
    }

    public void StopBtn()
    {
        StopUI.SetActive(true);
        Player.instance.isStop = true;
    }
    public void AgainBtn()
    {
        StopUI.SetActive(false);
        Player.instance.isStop = false;
    }
    public void MainBtn()
    {
        LoadingUIManager.Instance.LoadScene("Start");
        GameObject player = GameObject.FindWithTag("Player").gameObject;
        Destroy(player);
    }
    public void ReplayBtn()
    {
        LoadingUIManager.Instance.LoadScene("Lobby");
        GameObject player = GameObject.FindWithTag("Player").gameObject;
        Destroy(player);
    }
}
