using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class FileName
{
    public const string skillInfo = "skillInfo";
    public const string prologue = "prologue";
    public const string jobInfo = "jobinfo";
    public const string travel = "travel";
    public const string encounter = "encounter";
    public const string enemyInfo = "enemyInfo";
    public const string gameOver = "gameover";
    public const string ending = "ending";
}
public class GameDirector : MonoBehaviour
{
    public GameObject selectionBtns;
    public GameObject TextGenerator;
    public GameObject FlowManager;
    public GameObject StatusManager;
    public GameObject StageManager;
    public GameObject ExerState;
    public GameObject EnemyManager;
    public GameObject RetryBtn;

    public AudioSource beep;

    private bool checkSkillOnce;

    // Start is called before the first frame update
    void Start()
    {
        setAllGameObjectInActive();
        checkSkillOnce = false;
        TextGenerator.GetComponent<TextManager>().addSentenceFromFile(FileName.prologue, true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (FlowManager.GetComponent<FlowManager>().getCurFlow())
        {
            case Flow.Prologue:
                break;
            case Flow.ExplainJobSelect:
                TextGenerator.GetComponent<TextManager>().addSentenceFromFile(FileName.jobInfo, false);
                break;
            case Flow.SelectJob:
                selectionBtns.SetActive(true);
                if (selectionBtns.GetComponent<ButtonsManager>().isBtnClicked())
                {
                    allocateJob();
                    TextGenerator.GetComponent<TextManager>().notifyToReadList();
                    FlowManager.GetComponent<FlowManager>().notifyPlayerisNotNull();
                    selectionBtns.GetComponent<ButtonsManager>().setBtnInActive();
                    StatusManager.SetActive(true);
                }
                selectionBtns.GetComponent<ButtonsManager>().initBtnSelection();
                break;
            case Flow.Travel:
                TextGenerator.GetComponent<TextManager>().addSentenceFromFileByStage(FileName.travel, 
                                                                            StageManager.GetComponent<StageManager>().returnCurStage());
                break;
            case Flow.Exercising:
                ExerState.SetActive(true);
                if (ExerState.GetComponent<ExerManager>().notifyExerForGameDirector())
                    beep.Play();

                if (ExerState.GetComponent<ExerManager>().isSetOver())
                {
                    StatusManager.GetComponent<StatusManager>().letPlayerGetMana();
                    TextGenerator.GetComponent<TextManager>().notifyToReadList();
                }
                    
                if (ExerState.GetComponent<ExerManager>().isExerciseDone())
                {
                    ExerState.SetActive(false);
                    FlowManager.GetComponent<FlowManager>().exerciseDone();
                }
                break;
            case Flow.EncounterEnemy:
                TextGenerator.GetComponent<TextManager>().addSentenceFromFileByStage(FileName.encounter,
                                                                            StageManager.GetComponent<StageManager>().returnCurStage());
                break;
            case Flow.BattleEnemy:
                battleWithEnemy(FlowManager.GetComponent<FlowManager>(), StatusManager.GetComponent<StatusManager>(),
                                        EnemyManager.GetComponent<EnemyManager>());
                break;
            case Flow.GameOver:
                if (TextGenerator.GetComponent<TextManager>().checkTextEnded())
                    RetryBtn.SetActive(true);
                break;
            case Flow.Ending:
                ExerState.GetComponent<ExerManager>().resetExer();
                TextGenerator.GetComponent<TextManager>().addSentenceFromFile(FileName.ending, true);
                if (TextGenerator.GetComponent<TextManager>().checkTextEnded())
                    RetryBtn.SetActive(true);
                break;
        }
    }

    private void setAllGameObjectInActive()
    {
        selectionBtns.SetActive(false);
        StatusManager.SetActive(false);
        ExerState.SetActive(false);
        EnemyManager.SetActive(false);
        RetryBtn.SetActive(false);
    }
    private void allocateJob()
    {
        int selection = selectionBtns.GetComponent<ButtonsManager>().getBtnSelection();
        StatusManager.GetComponent<StatusManager>().allocJob(selection);
    }

    public void battleWithEnemy(FlowManager battleFlowManager, StatusManager playerManager, EnemyManager enemyManager)
    {
        switch(battleFlowManager.GetBattleFlow())
        {
            case BattleFlow.PrepareBattle:
                prepareBattle(enemyManager);
                battleFlowManager.notifyEnemyisNull(false);
                break;
            case BattleFlow.SelectSkill:
                if (playerManager.HasPlayerNoMana())
                {
                    gameOver();
                    break;
                }
                showSkillInfo(playerManager);  
                if (selectionBtns.GetComponent<ButtonsManager>().isBtnClicked())
                {
                    prepareCheckFlow(playerManager);
                    if (playerManager.HasPlayerNoMana())
                    {
                        gameOver();
                        break;
                    }
                    battleFlowManager.notifySkillSelected(true);
                }
                break;
            case BattleFlow.CheckSkill:
                bool didSkillChecked = (TextGenerator.GetComponent<TextManager>().checkTextEnded() && checkSkillOnce);
                if (didSkillChecked) // 스킬 체크 완료
                {
                    if (enemyManager.isEnemyNoHp())
                        defeatedEnemy(battleFlowManager, enemyManager);
                    prepareEnemyTurn(playerManager, enemyManager);
                    battleFlowManager.goToEnemyTurn();
                    break;
                }
                if (checkSkillOnce)
                    break;
                checkSkillOnce = true;
                useSkill(battleFlowManager, playerManager, enemyManager);
                break;
            case BattleFlow.EnemyAttack:
                bool attackedPlayer = (TextGenerator.GetComponent<TextManager>().checkTextEnded() && checkSkillOnce);
                if (attackedPlayer)
                {
                    preparePlayerTurn(playerManager);
                    battleFlowManager.goToPlayerTurn();
                    break;
                }
                if (checkSkillOnce)
                    break;
                checkSkillOnce = true;
                addEnemyAttackTxt(playerManager, enemyManager);
                if (playerManager.isPlayerHpZero())
                {
                    gameOver();
                }
                break;
        }
    }
    private void gameOver()
    {
        StatusManager.SetActive(false);
        ExerState.GetComponent<ExerManager>().resetExer();
        FlowManager.GetComponent<FlowManager>().goToGameOverFlow();
        TextGenerator.GetComponent<TextManager>().addSentenceFromFile(FileName.gameOver, false);
        
    }

    private void prepareBattle(EnemyManager enemyManager)
    {
        EnemyManager.SetActive(true);
        enemyManager.allocEnemy(StageManager.GetComponent<StageManager>().returnCurStage());
    }
    private void defeatedEnemy(FlowManager battleFlowManager, EnemyManager enemyManager)
    {
        

        checkSkillOnce = false;
        EnemyManager.SetActive(false);
        enemyManager.EnemyDead();
        battleFlowManager.notifyEnemyisNull(true);
        if (WasItLast(battleFlowManager))
            return;
        StageManager.GetComponent<StageManager>().goToNextStage();
    }

    private bool WasItLast(FlowManager flowManager)
    {
        if (StageManager.GetComponent<StageManager>().isLastStage())
        {
            flowManager.gameIsEnd();
            return true;
        }
            
        else
            return false;
    }
    private void backToSkillSelect(FlowManager battleFlowManager, StatusManager playerManager)
    {
        battleFlowManager.backToSkillSelect();
        playerManager.resetAddOnceFlag();
        checkSkillOnce = false;
    }

    private void showSkillInfo(StatusManager playerManager)
    {
        string skillShow = playerManager.showPlayerSkill();
        if (skillShow != "")
        {
            TextGenerator.GetComponent<TextManager>().addSentence(skillShow);
            TextGenerator.GetComponent<TextManager>().notifyToReadList();
        }
        if (TextGenerator.GetComponent<TextManager>().checkTextEnded())
            selectionBtns.SetActive(true);
    }

    private void prepareCheckFlow(StatusManager playerManager)
    {
        playerManager.resetSkillAddFlag();
        playerManager.refreshPlayerInfo();
        selectionBtns.GetComponent<ButtonsManager>().setBtnInActive();
    }

    private void prepareEnemyTurn(StatusManager playerManager, EnemyManager enemyManager)
    {
        selectionBtns.GetComponent<ButtonsManager>().initBtnSelection();
        checkSkillOnce = false;
        enemyManager.refreshInfo();
        playerManager.refreshPlayerInfo();
    }

    private void preparePlayerTurn(StatusManager playerManager)
    {
        checkSkillOnce = false;
        playerManager.refreshPlayerInfo();

    }

    private void addEnemyAttackTxt(StatusManager playerManager, EnemyManager enemyManager)
    {
        int atk = enemyManager.getEnemyAtk();
        TextGenerator.GetComponent<TextManager>().addSentence(enemyManager.getAtkSentence());
        if (atk == 0)
        {
            TextGenerator.GetComponent<TextManager>().addSentence(enemyManager.getFailSentence());
        }
        else
        {
            TextGenerator.GetComponent<TextManager>().addSentence(enemyManager.getSuccSentence());
        }

        playerManager.getPlayerDamage(atk);
    }

    private bool useSkill(FlowManager battleFlowManager, StatusManager playerManager, EnemyManager enemyManager)
    {
        int skillSelect = selectionBtns.GetComponent<ButtonsManager>().getBtnSelection();
        int skillDamage = playerManager.playerUseSkill(skillSelect);
        TextGenerator.GetComponent<TextManager>().notifyToReadList();
        if (skillDamage == -1)
        {
            backToSkillSelect(battleFlowManager, playerManager);
            return false;
        }
        else if (skillDamage > 0)
        {
            enemyManager.getEnemyDamage(skillDamage);
        }

        return true;
    }
}


public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair(T first, U second)
    {
        this.Full = first;
        this.Remain = second;
    }

    public T Full { get; set; }
    public U Remain { get; set; }
};

