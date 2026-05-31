using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int playerHp = 100;
    public int enemyHp = 50;

    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;
    public TMP_Text resultText;


    void Start()
    {
        resultText.text = "";
        UpdateUI();
    }

    // Update is called once per frame
    public void UseAttackCard()
    {
        enemyHp -= 10;
        CheckEnemyDead();

        if (enemyHp > 0)
        {
            EnemyAttack();
        }

        UpdateUI();
    }

    public void UseStrongAttackCard()
    {
        enemyHp -= 20;
        CheckEnemyDead();

        if (enemyHp > 0)
        {
            EnemyAttack();
        }

        UpdateUI();
    }

    public void UseHealCard()
    {
        playerHp += 15;

        if (playerHp > 100)
        {
            playerHp = 100;
        }

        EnemyAttack();
        UpdateUI();
    }

    void EnemyAttack()
    {
        playerHp -= 5;

        if(playerHp<=0)
        {
            playerHp = 0;
            resultText.text = "패배...";
        }
    }

    void CheckEnemyDead()
    {
        if(enemyHp<=0)
        {
            enemyHp = 0;
            resultText.text = "승리!";
        }
    }
    void UpdateUI()
    {
        playerHpText.text = "플레이어 HP : " + playerHp;
        enemyHpText.text = "슬라임 HP : " + enemyHp;
    }
}
