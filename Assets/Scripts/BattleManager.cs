using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int playerHp = 100;
    public int enemyHp = 50;

    private int bossTurnCount = 0;

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

    public Button cardButton1;
    public Button cardButton2;
    public Button cardButton3;

    public Button rewardButton1;
    public Button rewardButton2;
    public Button rewardButton3;

    public Button nextStageButton;

    private CardData rewardCard1;
    private CardData rewardCard2;
    private CardData rewardCard3;

    public CardData attackCard;
    public CardData strongAttackCard;
    public CardData healCard;
    public CardData defenseCard;

    List<CardData> deck = new List<CardData>();
    List<CardData> hand = new List<CardData>();
    List<CardData> discardPile = new List<CardData>();


    //string[] enemyNames = { "슬라임", "고블린", "늑대", "보스" };
    //int[] enemyMaxHps = { 50, 70, 90, 150 };
    public EnemyData[] enemies;
    int currentEnemyIndex = 0;

    public int playerDefense = 0;

    bool isChoosingReward = false;

    void Start()
    { 
        resultText.text = "";

        HideRewardButtons();
        nextStageButton.gameObject.SetActive(false);

        //enemyHp = enemyMaxHps[currentEnemyIndex];
        enemyHp = enemies[currentEnemyIndex].maxHp;

        MakeDeck();
        ShuffleDeck();
        DrawCards();

        UpdateUI();
    }
    void MakeDeck()
    {
        deck.Clear();

        deck.Add(attackCard);
        deck.Add(attackCard);
        deck.Add(attackCard);
        deck.Add(attackCard);
        deck.Add(strongAttackCard);
        deck.Add(healCard);
        deck.Add(defenseCard);
        deck.Add(defenseCard);
    }

    void HideRewardButtons()
    {
        isChoosingReward = false;

        rewardButton1.gameObject.SetActive(false);
        rewardButton2.gameObject.SetActive(false);
        rewardButton3.gameObject.SetActive(false);
    }

    void ShowRewardButtons()
    {
        isChoosingReward = true;

        rewardCard1 = GetRandomRewardCard();
        rewardCard2 = GetRandomRewardCard();
        rewardCard3 = GetRandomRewardCard();

        rewardButton1.gameObject.SetActive(true);
        rewardButton2.gameObject.SetActive(true);
        rewardButton3.gameObject.SetActive(true);

        rewardButton1.GetComponentInChildren<TMP_Text>().text = rewardCard1.cardName;
        rewardButton2.GetComponentInChildren<TMP_Text>().text = rewardCard2.cardName;
        rewardButton3.GetComponentInChildren<TMP_Text>().text = rewardCard3.cardName;
    }

    CardData GetRandomRewardCard()
    {
        int randomNumber = Random.Range(0, 4);

        if (randomNumber == 0)
        {
            return attackCard;
        }
        else if (randomNumber == 1)
        {
            return strongAttackCard;
        }
        else if (randomNumber == 2)
        {
            return healCard;
        }
        else
        {
            return defenseCard;
        }
    }

    public void SelectReward1()
    {
        SelectReward(rewardCard1);
    }

    public void SelectReward2()
    {
        SelectReward(rewardCard2);
    }
    public void SelectReward3()
    {
        SelectReward(rewardCard3);
    }

    void SelectReward(CardData selectCard)
    {
        discardPile.Add(selectCard);
        
        HideRewardButtons();

        resultText.text = selectCard.cardName + " 획득";

        nextStageButton.gameObject.SetActive(true);

        //DiscardHand();
        //DrawCards();
        UpdateUI();
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);

            CardData temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void DrawCards()
    {
        //hand.Clear();

        DrawOneCard();
        DrawOneCard();
        DrawOneCard();

        cardButton1.GetComponentInChildren<TMP_Text>().text = hand[0].cardName;
        cardButton2.GetComponentInChildren<TMP_Text>().text = hand[1].cardName;
        cardButton3.GetComponentInChildren<TMP_Text>().text = hand[2].cardName;
    }

    void DrawOneCard()
    {
        if (deck.Count == 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }

        CardData card = deck[0];
        deck.RemoveAt(0);
        hand.Add(card);
    }


    public void UseCard1()
    {
        UseCard(0);
    }

    public void UseCard2()
    {
        UseCard(1);
    }

    public void UseCard3()
    {
        UseCard(2);
    }

    void UseCard(int handIndex)
    {
        if (playerHp <= 0 || currentEnemyIndex >= enemies.Length || isChoosingReward || enemyHp<=0 )
        {
            return;
        }

        CardData card = hand[handIndex];

        if(card.cardType==CardType.Attack)
        {
            enemyHp -= card.damage;
        }
        else if(card.cardType==CardType.Heal)
        {
            playerHp += card.heal;

            if (playerHp > 100)
            {
                playerHp = 100;
            }
        }
        else if (card.cardType == CardType.Defense)
        {
            playerDefense += card.defense;
        }

            DiscardHand();

        CheckEnemyDead();

        if (currentEnemyIndex<enemies.Length &&enemyHp > 0)
        {
            EnemyAttack();
        }

        if (currentEnemyIndex < enemies.Length)
        {
            DrawCards();

        }

        UpdateUI();
    }

    void DiscardHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            discardPile.Add(hand[i]);
        }

        hand.Clear();
    }

    void EnemyAttack()
    {
        EnemyData currentEnemy = enemies[currentEnemyIndex];

        int enemyDamage = enemies[currentEnemyIndex].attackDamage;

        if (currentEnemy.isBoss)
        {
            bossTurnCount++;

            if (currentEnemy.specialAttackTurn > 0 && bossTurnCount % currentEnemy.specialAttackTurn == 0)
            {
                enemyDamage = Random.Range(currentEnemy.specialAttackDamage - 5,
                    currentEnemy.specialAttackDamage + 5);
                resultText.text = currentEnemy.enemyName +
                    "의 특수 공격(" + enemyDamage + " 데미지)";
            }
        }

        if (playerDefense > 0)
        {
            playerDefense -= enemyDamage;

            if (playerDefense < 0)
            {
                playerHp += playerDefense;
                playerDefense = 0;
            }
        }
        else
        {
            playerHp -= enemyDamage;
        }

        if (playerHp <= 0)
        {
            playerHp = 0;
            resultText.text = "패배...";
        }
    }

    void CheckEnemyDead()
    {
        if(enemyHp<=0)
        {
            currentEnemyIndex++;

            if (currentEnemyIndex >= enemies.Length)
            {
                enemyHp = 0;
                resultText.text = "보스 처치! Game Clear";
            }
            else
            {
                enemyHp = 0;
                resultText.text = "카드 보상을 선택하세요";
                ShowRewardButtons();
                //resultText.text = enemyNames[currentEnemyIndex] + " 등장";
            }
        }
    }

    public void GoToNextStage()
    {
        if(currentEnemyIndex>=enemies.Length)
        {
            return;
        }

        nextStageButton.gameObject.SetActive(false);

        //enemyHp = enemyMaxHps[currentEnemyIndex];
        enemyHp = enemies[currentEnemyIndex].maxHp;
        playerDefense = 0;
        bossTurnCount = 0;

        //resultText.text = enemyNames[currentEnemyIndex] + " 등장";
        resultText.text = enemies[currentEnemyIndex].enemyName + " 등장";

        DiscardHand();
        DrawCards();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (currentEnemyIndex < enemies.Length)
        {
            stageText.text = "Stage" + (currentEnemyIndex + 1);
        }
        else
        {
            stageText.text = "Clear";
        }

        playerHpText.text = "플레이어 HP : " + playerHp;
        playerDefenseText.text = "방어도 : " + playerDefense;

        if (currentEnemyIndex < enemies.Length)
        {
            enemyHpText.text = enemies[currentEnemyIndex].enemyName + " HP : " + enemyHp;
            enemyAttackText.text = "적 공격력 : " + enemies[currentEnemyIndex].attackDamage;
        }
        else
        {
            enemyHpText.text = "적 전멸";
            enemyAttackText.text = "적 공격력 : 0";
        }

        deckCountText.text = "덱 : " + deck.Count;
        handCountText.text = "손패 : " + hand.Count;
        discardCountText.text = "묘지 : " + discardPile.Count;

        int totalCardCount = deck.Count + hand.Count + discardPile.Count;
        totalCardCountText.text = "전체 카드 : " + totalCardCount;
    }
}
