using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class Enemy
{
    protected string name;
    protected Pair<int, int> hp;
    protected int atk;
    protected float poss;
    protected string attackSentence;
    protected string attackSucc;
    protected string attackFail;

    public Enemy(string name)
    {
        string info = retEnemyInfoByFile(name);
        setEnemyInfo(info);

        attackSentence = this.name + "의 공격";
        attackSucc = "당신은 " + atk + "의 데미지를 받았다";
        attackFail = "당신은 무사히 공격을 피했다.";
        
    }

    protected string retEnemyInfoByFile(string name)
    {
        string readLine;
        TextAsset asset = Resources.Load(FileName.enemyInfo) as TextAsset;
        string str = asset.text;

        TextReader txtReader = new StringReader(str);

        while ((readLine = txtReader.ReadLine()) != null)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(readLine, name))
            {
                return txtReader.ReadLine();
            }
        }

        return "";
    }

    protected void setEnemyInfo(string enemyInfo)
    {
        string[] eInfo = enemyInfo.Split('/');

        this.name = eInfo[0];
        try
        {
            hp = new Pair<int, int>(Int32.Parse(eInfo[1]), Int32.Parse(eInfo[1]));
            atk = Int32.Parse(eInfo[2]);
            poss = float.Parse(eInfo[3]);
        }
        catch (FormatException e)
        {
            Debug.Log(e.Message);
        }
    }

    public string getEnemyName()
    {
        return name;
    }

    public string getHpInfo()
    {
        return hp.Remain + "/" + hp.Full;
    }

    public int getAtk()
    {
        return atk;
    }

    public void enemyGetDamage(int dmg)
    {
        hp.Remain -= dmg;
    }

    public int enemyAttack()
    {
        int atkRnd = UnityEngine.Random.Range(1, 101);
        if (atkRnd > poss)
            return 0;
        else
            return atk;
    }

    public bool isHpZero()
    {
        bool ret = (hp.Remain <= 0) ? true : false;

        return ret;
    }

    public string getAtkSentence()
    {
        return attackSentence;
    }

    public string getSuccSentence()
    {
        return attackSucc;
    }

    public string getFailSentence()
    {
        return attackFail;
    }
}


public class Bird : Enemy
{
    public Bird() : base("Bird")
    {

    }
    
}

public class Orc : Enemy
{
    public Orc() : base ("Orc")
    {

    }
}

public class Scorpion : Enemy
{
    public Scorpion() : base("Scorpion")
    {

    }
}
