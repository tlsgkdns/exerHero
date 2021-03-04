using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    Enemy enemy;
    private bool refresh;
    private enum StatusTxt
    { ClassName, Hp, Atk }

    // Start is called before the first frame update
    void Start()
    {
        refresh = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemy != null && refresh)
        {
            setChildText(StatusTxt.ClassName, enemy.getEnemyName());
            setChildText(StatusTxt.Hp, "체력: " + enemy.getHpInfo());
            setChildText(StatusTxt.Atk, "공격력: " + enemy.getAtk());
            refresh = false;
        }
    }
    private void setChildText(StatusTxt st, string txt)
    {
        this.gameObject.transform.GetChild((int)st).gameObject.GetComponent<Text>().text = txt;
    }

    public void allocEnemy(Stage curStage)
    {
        switch(curStage)
        {
            case Stage.Cliff:
                enemy = new Bird();
                break;
            case Stage.Cave:
                enemy = new Orc();
                break;
            case Stage.Desert:
                enemy = new Scorpion();
                break;
        }
    }

    public void getEnemyDamage(int damage)
    {
        enemy.enemyGetDamage(damage);
    }

    public bool isEnemyNoHp()
    {
        return enemy.isHpZero();
    }

    public void EnemyDead()
    {
        enemy = null;
        refresh = true;
    }

    public void refreshInfo()
    {
        refresh = true;
    }

    public int getEnemyAtk()
    {
        return enemy.enemyAttack();
    }

    public string getAtkSentence()
    {
        return enemy.getAtkSentence();
    }

    public string getSuccSentence()
    {
        return enemy.getSuccSentence();
    }

    public string getFailSentence()
    {
        return enemy.getFailSentence();
    }
}
