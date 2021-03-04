using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    Player player;
    public GameObject TextGenerator;

    private bool refresh;
    private bool addOnce;
    private enum StatusTxt
    { ClassName, Hp, Mana }

    private const string ManaDeficit = "마나가 부족합니다";

    // Start is called before the first frame update
    void Start()
    {
        refresh = true;
        addOnce = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && refresh)
        {
            setChildText(StatusTxt.ClassName, player.getClassName());
            setChildText(StatusTxt.Hp, "체력: " + player.getHpInfo());
            setChildText(StatusTxt.Mana, "소유 마나: " + player.getRemainMana());
            refresh = false;
        }
    }

    private void setChildText(StatusTxt st, string txt)
    {
        this.gameObject.transform.GetChild((int)st).gameObject.GetComponent<Text>().text = txt;
    }
    public void allocJob(int selection)
    {
        switch (selection)
        {
            case 0:
                break;
            case 1:
                player = new Archer();
                TextGenerator.GetComponent<TextManager>().addSentence("백발 백중의 궁수이다.");
                break;
            case 2:
                player = new Warrior();
                TextGenerator.GetComponent<TextManager>().addSentence("저돌적인 전사이다.");
                break;
            case 3:
                player = new Magician();
                TextGenerator.GetComponent<TextManager>().addSentence("원소를 다루는 마법사이다.");
                break;
            default:
                Debug.LogError("selection Error");
                break;
        }
    }

    public void letPlayerGetMana()
    {
        int m = player.addMana();
        int rec = recoverPlayerHp();
        string infoSentence = (rec == 0) ? m + "개의 마나석을 획득하였다" : m + "개의 마나석을 획득하였다, " + rec + " Hp 회복";

        TextGenerator.GetComponent<TextManager>().addSentence(infoSentence);

        refresh = true;
    }

    public string showPlayerSkill() // 스킬 정보를 보여준다. addOnce는 한 번만 보여주도록 하는 플래그이다.
    {
        if (!addOnce)
        {
            addOnce = true;
            return player.showSkillsInfo();
        }
        else
            return "";
    }

    public void resetSkillAddFlag()
    {
        addOnce = false;
    }

    public int playerUseSkill(int skillNum)
    {
        int skillDamage = player.useSkill(skillNum);

        if (skillDamage == -1)
        {
            TextGenerator.GetComponent<TextManager>().addSentence(ManaDeficit);
        }
        else if (skillDamage == -2)
        {
            TextGenerator.GetComponent<TextManager>().addSentence(player.getSkillUseSentence(skillNum));
            TextGenerator.GetComponent<TextManager>().addSentence(player.getFailSentence(skillNum));
            
        }
        else
        {
            TextGenerator.GetComponent<TextManager>().addSentence(player.getSkillUseSentence(skillNum));
            TextGenerator.GetComponent<TextManager>().addSentence(player.getSuccSentence(skillNum));

        }

        return skillDamage;
    }

    public bool HasPlayerNoMana()
    {
        if (player.getRemainMana() == "0")
            return true;
        else
            return false;
    }

    public void getPlayerDamage(int dmg)
    {
        player.getDamage(dmg);
    }

    public void refreshPlayerInfo()
    {
        refresh = true;
    }

    public bool isPlayerHpZero()
    {
        return player.isPlayerNoHp();
    }

    public void resetAddOnceFlag()
    {
        addOnce = false;
    }

    private int recoverPlayerHp()
    {
        int recoverHp = UnityEngine.Random.Range(0, 5);

        recoverHp = player.recoverHp(recoverHp);

        return recoverHp;
    }
}
