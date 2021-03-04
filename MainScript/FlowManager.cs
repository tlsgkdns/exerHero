using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Flow
{
    GameOver = -1,Prologue, ExplainJobSelect, SelectJob, Travel, Exercising,
    EncounterEnemy, BattleEnemy, Ending
}

public enum BattleFlow
{
    NonBattle, PrepareBattle, SelectSkill, CheckSkill, EnemyAttack, DefeatEnemy
}

public class FlowManager : MonoBehaviour
{
    public GameObject TextGenerator;
    private Flow curFlow;
    private BattleFlow battleFlow;

    private bool isEnemyNull;
    private bool isPlayerNull;
    private bool isExerciseDone;
    private bool isSkillSelected;
    private bool enemyTurn;
    private bool isGameEnd;

    // Start is called before the first frame update
    void Start()
    {
        curFlow = Flow.Prologue;
        battleFlow = BattleFlow.NonBattle;
        isEnemyNull = true;
        isPlayerNull = true;
        isExerciseDone = false;
        isSkillSelected = false;
        enemyTurn = false;
        isGameEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (curFlow)
        {
            case Flow.Prologue:
            case Flow.ExplainJobSelect:
            case Flow.Travel:
            case Flow.EncounterEnemy:
                if (TextGenerator.GetComponent<TextManager>().checkTextEnded())
                {
                    curFlow++;
                    TextGenerator.GetComponent<TextManager>().resetFileReadFlag();
                }
                break;
            case Flow.SelectJob:
                checkFlagAndGoToNextFlow(!isPlayerNull);
                break;
            case Flow.Exercising:
                checkFlagAndGoToNextFlow(isExerciseDone);
                break;
            case Flow.BattleEnemy:
                if (battleFlow == BattleFlow.NonBattle)
                    battleFlow = BattleFlow.PrepareBattle;
                cycleBattleFlow();
                break;
            case Flow.GameOver:
            case Flow.Ending:
                break;
        }
    }

    private void checkFlagAndGoToNextFlow(bool flag)
    {
        if (flag)
        {
            curFlow++;
        }
    }

    private void checkFlagAndGoToNextBattleFlow(bool flag)
    {
        if (flag)
        {
            battleFlow++;
        }
    }
    private void cycleBattleFlow()
    {
        switch (battleFlow)
        {
            case BattleFlow.PrepareBattle:
                checkFlagAndGoToNextBattleFlow(!isEnemyNull);
                break;
            case BattleFlow.SelectSkill:
                checkFlagAndGoToNextBattleFlow(isSkillSelected);
                break;
            case BattleFlow.CheckSkill:
                isSkillSelected = false;
                if (isEnemyNull)
                    battleFlow = BattleFlow.DefeatEnemy;
                else
                    checkFlagAndGoToNextBattleFlow(enemyTurn);
                break;
            case BattleFlow.EnemyAttack:
                if (!enemyTurn)
                    battleFlow = BattleFlow.SelectSkill;
                break;
            case BattleFlow.DefeatEnemy:
                if (isGameEnd)
                {
                    curFlow = Flow.Ending;
                    battleFlow = BattleFlow.NonBattle;
                }
                else if (TextGenerator.GetComponent<TextManager>().checkTextEnded())
                    battleOver();
                break;
            default:
                break;
        }
    }
    public Flow getCurFlow()
    {
        return curFlow;
    }

    public void exerciseDone()
    {
        isExerciseDone = true;
    }

    public void notifyEnemyisNull(bool isNull)
    {
        isEnemyNull = isNull;
    }

    public void notifyPlayerisNotNull()
    {
        isPlayerNull = false;
    }
    public void notifySkillSelected(bool selected)
    {
        isSkillSelected = selected;
    }

    public BattleFlow GetBattleFlow()
    {
        return battleFlow;
    }

    public void backToSkillSelect()
    {
        battleFlow = BattleFlow.SelectSkill;
        isSkillSelected = false;
    }

    public void goToEnemyTurn()
    {
        enemyTurn = true;
    }

    public void goToPlayerTurn()
    {
        enemyTurn = false;
    }

    private void battleOver()
    {
        battleFlow = BattleFlow.NonBattle;
        curFlow = Flow.Travel;
        isExerciseDone = false;
        enemyTurn = false;
    }

    public void goToGameOverFlow()
    {
        battleFlow = BattleFlow.NonBattle;
        curFlow = Flow.GameOver;
    }

    public void gameIsEnd()
    {
        isGameEnd = true;
    }
}
