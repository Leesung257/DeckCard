using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


public class BattleUIManager : MonoBehaviour
{
    public TMP_Text battleLogText;

    private const int MaxBattleLogCount = 5;
    private readonly List<string> battleLogs = new List<string>();

    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;
    public TMP_Text resultText;
    public TMP_Text playerDefenseText;

    public TMP_Text deckCountText;
    public TMP_Text handCountText;
    public TMP_Text discardCountText;
    public TMP_Text totalCardCountText;

    public TMP_Text stageText;
    public TMP_Text enemyAttackText;
    public TMP_Text goldText;

    public void InitializeBattleLog()
    {
        battleLogs.Clear();
        battleLogText.text = "";
    }

    public void AddLog(string message)
    {
        battleLogs.Add(message);

        if(battleLogs.Count > MaxBattleLogCount)
        {
            battleLogs.RemoveAt(0);
        }

        battleLogText.text = "";

        for(int i = 0;i< battleLogs.Count; i++)
        {
            battleLogText.text += battleLogs[i] + "\n";
        }
    }

    public void UpdateStageUI(int currentEnemyIndex, int enemyCount)
    {
        if (currentEnemyIndex < enemyCount)
        {
            stageText.text = "Stage" + (currentEnemyIndex + 1);
        }
        else
        {
            stageText.text = "Clear";
        }
    }

    public void UpdatePlayerUI(int playerHp, int playerDefense)
    {
        playerHpText.text = "플레이어 HP : " + playerHp;
        playerDefenseText.text = "방어도 : " + playerDefense;
    }

    public void UpdateEnemyUI(
        bool hasEnemy,
        string enemyName,
        int enemyHp,
        int enemyDefense,
        int enemyAttackDamage)
    {
        if(hasEnemy)
        {
            enemyHpText.text = enemyName + " HP : " + enemyHp + " / 방어도 : " + enemyDefense;
            enemyAttackText.text = "적 공격력 : " + enemyAttackDamage;
        }
        else
        {
            enemyHpText.text = "적 전멸";
            enemyAttackText.text = "적 공격력 : 0";
        }
    }

    public void UpdateCardCountUI(
        int deckCount,
        int handCount,
        int discardCount)
    {
        deckCountText.text = "덱 : " + deckCount;
        handCountText.text = "손패 : " + handCount;
        discardCountText.text = "묘지 : " + discardCount;

        int totalCardCount = deckCount + handCount + discardCount;
        totalCardCountText.text = "전체 카드 : " + totalCardCount;
    }

    public void UpdateGoldUI(int gold)
    {
        goldText.text = "Gold : " + gold;
    }
}
