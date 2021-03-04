using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
    protected const int basicMana = 5;
    protected Pair<int, int> hp;
    protected int mana;
    public readonly int SkillCount;
    protected List<Skill> skillList;
    protected string className;
    protected string DamageString = " 데미지";
    protected Player()
    {
        skillList = new List<Skill>();
        hp = new Pair<int, int>(100, 100);
        mana = 0;
        SkillCount = 3;
    }

    public virtual int addMana()
    {
        mana += basicMana;

        return basicMana;
    }

    public virtual int useMana(Skill skill) // 마나 사용 성공 시, 데미지 반환, 아니면 -1 반환
    {
        if (skill.getManaCost() > mana)
            return -1;

        mana -= skill.getManaCost();

        return skill.getSkillDamage();
    }

    public int getDamage(int damage) // 데미지를 받을 때, HP가 0보다 작으면 -1, 아니면 남은 HP 반환
    {
        hp.Remain -= damage;
        if (hp.Remain < 0)
            hp.Remain = -1;

        return hp.Remain;
    }

    public int recoverHp(int recover) // Hp 회복 함수, Hp는 최대 Hp를 넘지 못한다. 현재 HP를 반환
    {
        hp.Remain += recover;

        if (hp.Remain >= hp.Full)
        {
            hp.Remain = hp.Full;
            return 0;
        }

        return recover;
    }
    public int useSkill(int skillNum) // 스킬을 사용한다. 마나가 부족하면 -1, 확률을 벗어나면 -2, 아니면 데미지를 반환한다.
    {
        Skill usedSkill = skillList[skillNum - 1];
        int rndNum = UnityEngine.Random.Range(1, 101);
        int skillDamage = useMana(usedSkill);
        if (skillDamage == -1)
        {
            return -1;
        }
        if (rndNum > usedSkill.getPossibility())
        {
            return -2;
        }

        return skillDamage;
    }
    public void setSkills(string job) // skillInfo 파일을 통해 skill 정보를 세팅한다.
    {
        string readLine;
        TextAsset asset = Resources.Load(FileName.skillInfo) as TextAsset;
        string str = asset.text;

        TextReader txtReader = new StringReader(str);

        while ((readLine = txtReader.ReadLine()) != null)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(readLine, job))
            {
                for (int i = 0; i < SkillCount; i++)
                {
                    
                    string sInfo = txtReader.ReadLine();
                    skillList.Add(new Skill(sInfo));
                }
                break;
            }
        }
        if (readLine == null)
            Debug.LogError("no Info");
    }

    public string getClassName()
    {
        return className;
    }

    public string getHpInfo()
    {
        return hp.Remain + "/" + hp.Full;
    }

    public string getRemainMana()
    {
        return mana.ToString();
    }
    
    public string showSkillsInfo()
    {
        string retInfo = "";
        for(int i=0; i < SkillCount; i++)
        {
            int num = i+1;
            retInfo += num + ". " + skillList[i].ShowSkillInfo() + "\n";
        }

        return retInfo;
    }

    public string getSkillUseSentence(int skillNum)
    {
        return skillList[skillNum - 1].getUseSentence();
    }

    public string getFailSentence(int skillNum)
    {
        return skillList[skillNum - 1].getFailSentence();
    }

    public string getSuccSentence(int skillNum)
    {
        return skillList[skillNum - 1].getSuccSentence() + " (-" + skillList[skillNum - 1].getSkillDamage() + DamageString + ")";
    }

    public bool isPlayerNoHp()
    {
        bool ret = (hp.Remain <= 0) ? true : false;

        return ret;
    }
}

public class Archer : Player // 아처, 50% 확률로 마나석의 절반을 추가로 얻는다. 단, 타격 확률이 낮다.
{
    const float additionalManaPoss = 50f;

    public Archer()
    {
        className = "Archer";
        setSkills(className);
    }

    public override int addMana()
    {
        int addedMana = basicMana;
        int addRnd = UnityEngine.Random.Range(1, 101);

        if (addRnd < additionalManaPoss)
        {
            int additionalMana = (int) (basicMana * 0.5f);
            addedMana = basicMana + additionalMana;

        }
        mana += addedMana;

        return addedMana;
    }
  
}
public class Warrior : Player // 워리어, 데미지를 타격 할 확률이 높다. 단, 데미지가 낮다.
{ 
    public Warrior()
    {
        className = "Warrior";
        setSkills(className);
    }
}
public class Magician : Player // 매지션, 마나석 소비가 50% 확률로 하나 줄어든다. 단, 50% 확률로 마나석 채집 때, 마나석의 절반만 얻는다.
{
    public Magician()
    {
        className = "Magician";
        setSkills(className);
    }

    public override int useMana(Skill skill)
    {
        int costMana = skill.getManaCost();

        if (skill.getManaCost() > mana)
            return -1;

        if (UnityEngine.Random.value > 0.5f)
        {
            costMana -= 1;
        }
            
        mana -= costMana;

        return skill.getSkillDamage();
    }

    public override int addMana()
    {
        int add = basicMana;

        if (UnityEngine.Random.value > 0.5f)
        {
            add = basicMana / 2;
        }

        mana += add;

        return add;
    }
}

public class Skill
{
    private string skillName;
    private int useMana;
    private int possibility;
    private int damage;
    private string useSentence; 
    private string succSentence;
    private string failSentence;
    

    public Skill(string skillInfo) // skillInfo : {이름/사용마나/확률/데미지/사용 시 문장/성공 시 문장/실패 시 문장} 순.
    { 
        string[] sInfo = skillInfo.Split('/');
        skillName = sInfo[0];
        useSentence = sInfo[4];
        succSentence = sInfo[5];
        failSentence = sInfo[6];
        try
        {
            useMana = Int32.Parse(sInfo[1]);
            possibility = Int32.Parse(sInfo[2]);
            damage = Int32.Parse(sInfo[3]);
        }catch(FormatException e)
        {
            Debug.Log(e.Message);
        }
    }
    public string ShowSkillInfo()
    {
        return skillName + ". " + "데미지:" + damage + " 소모 마나:" + useMana + " 확률:" + possibility + "%";
    }
    public int DamageUp(int up)
    {
        damage += up;

        return damage;
    }

    public int DamageDown(int down)
    {
        damage -= down;

        return damage;
    }

    public int getPossibility()
    {
        return possibility;
    }

    public string getUseSentence()
    {
        return useSentence;
    }
    public string getSuccSentence()
    {
        return succSentence;
    }
    public string getFailSentence()
    {
        return failSentence;
    }

    public int getManaCost()
    {
        return useMana;
    }

    public int getSkillDamage()
    {
        return damage;
    }
}