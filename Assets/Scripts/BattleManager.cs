using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int playerHp = 100;
    public int enemyHp = 50;

    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;
    public TMP_Text resultText;

    public TMP_Text deckCountText;
    public TMP_Text handCountText;
    public TMP_Text discardCountText;

    public Button cardButton1;
    public Button cardButton2;
    public Button cardButton3;

    List<string> deck = new List<string>();
    List<string> hand = new List<string>();
    List<string> discardPile = new List<string>();

    string card1;
    string card2;
    string card3;

    void Start()
    {
        resultText.text = "";

        MakeDeck();
        ShuffleDeck();
        DrawCards();

        UpdateUI();
    }
    void MakeDeck()
    {
        deck.Clear();

        deck.Add("공격 카드");
        deck.Add("공격 카드");
        deck.Add("공격 카드");
        deck.Add("공격 카드");
        deck.Add("강공격 카드");
        deck.Add("회복 카드");
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);

            string temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void DrawCards()
    {
        hand.Clear();

        DrawOneCard();
        DrawOneCard();
        DrawOneCard();

        cardButton1.GetComponentInChildren<TMP_Text>().text = hand[0];
        cardButton2.GetComponentInChildren<TMP_Text>().text = hand[1];
        cardButton3.GetComponentInChildren<TMP_Text>().text = hand[2];
    }

    void DrawOneCard()
    {
        if (deck.Count == 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }

        string card = deck[0];
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
        if (enemyHp <= 0 || playerHp <= 0)
        {
            return;
        }

        string cardName = hand[handIndex];

        if(cardName=="공격 카드")
        {
            enemyHp -= 10;
        }
        else if(cardName=="강공격 카드")
        {
            enemyHp -= 20;
        }
        else if(cardName=="회복 카드")
        {
            playerHp += 15;

            if (playerHp > 100)
            {
                playerHp = 100;
            }
        }

        DiscardHand();

        CheckEnemyDead();

        if (enemyHp > 0)
        {
            EnemyAttack();
        }

        DrawCards();
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

        deckCountText.text = "덱 : " + deck.Count;
        handCountText.text = "손패 : " + hand.Count;
        discardCountText.text = "버림더미 : " + discardPile.Count;
    }
}
