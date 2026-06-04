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

    public TMP_Text battleLogText;
    //string battleLog = "";
    List<string> battleLogs = new List<string>();

    public Button cardButton1;
    public Button cardButton2;
    public Button cardButton3;

    public Button rewardButton1;
    public Button rewardButton2;
    public Button rewardButton3;

    public Button nextStageButton;

    public Button upgradeCardButton;

    public Button upgradeSelectButton1;
    public Button upgradeSelectButton2;
    public Button upgradeSelectButton3;

    CardInstance upgradeCard1;
    CardInstance upgradeCard2;
    CardInstance upgradeCard3;

    public Button removeCardButton;

    public Button removeSelectButton1;
    public Button removeSelectButton2;
    public Button removeSelectButton3;

    CardInstance removeCard1;
    CardInstance removeCard2;
    CardInstance removeCard3;

    private CardData rewardCard1;
    private CardData rewardCard2;
    private CardData rewardCard3;

    public CardData attackCard;
    public CardData strongAttackCard;
    public CardData healCard;
    public CardData defenseCard;

    List<CardInstance> deck = new List<CardInstance>();
    List<CardInstance> hand = new List<CardInstance>();
    List<CardInstance> discardPile = new List<CardInstance>();

    public Slider playerHpSlider;
    public Slider enemyHpSlider;

    public int playerMaxHp = 100;

    public EnemyData[] enemies;
    int currentEnemyIndex = 0;

    public int playerDefense = 0;

    bool isChoosingReward = false;

    CardData selectUpgradeCard;

    bool usedDeckAction = false;

    void Start()
    { 
        resultText.text = "";
        //battleLog = "";
        battleLogs.Clear();
        battleLogText.text = "";

        HideRewardButtons();
        nextStageButton.gameObject.SetActive(false);
        upgradeCardButton.gameObject.SetActive(false);
        upgradeSelectButton1.gameObject.SetActive(false);
        upgradeSelectButton2.gameObject.SetActive(false);
        upgradeSelectButton3.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);
        removeSelectButton1.gameObject.SetActive(false);
        removeSelectButton2.gameObject.SetActive(false);
        removeSelectButton3.gameObject.SetActive(false);

        enemyHp = enemies[currentEnemyIndex].maxHp;

        playerHpSlider.maxValue = playerMaxHp;
        playerHpSlider.value = playerHp;

        enemyHpSlider.maxValue=enemies[currentEnemyIndex].maxHp;
        enemyHpSlider.value = enemyHp;

        MakeDeck();
        ShuffleDeck();
        DrawCards();

        UpdateUI();
    }
    void MakeDeck()
    {
        deck.Clear();

        deck.Add(new CardInstance(attackCard));
        deck.Add(new CardInstance(attackCard));
        deck.Add(new CardInstance(attackCard));
        deck.Add(new CardInstance(attackCard));
        deck.Add(new CardInstance(strongAttackCard));
        deck.Add(new CardInstance(healCard));
        deck.Add(new CardInstance(defenseCard));
        deck.Add(new CardInstance(defenseCard));
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

        //rewardButton1.GetComponentInChildren<TMP_Text>().text = rewardCard1.cardName;
        //rewardButton2.GetComponentInChildren<TMP_Text>().text = rewardCard2.cardName;
        //rewardButton3.GetComponentInChildren<TMP_Text>().text = rewardCard3.cardName;
        SetRewardButtonText(rewardButton1, rewardCard1);
        SetRewardButtonText(rewardButton2, rewardCard2);
        SetRewardButtonText(rewardButton3, rewardCard3);
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
        discardPile.Add(new CardInstance(selectCard));
        
        HideRewardButtons();

        usedDeckAction = false;

        resultText.text = selectCard.cardName + " 획득";
        AddLog(selectCard.cardName + " 획득");

        nextStageButton.gameObject.SetActive(true);
        upgradeCardButton.gameObject.SetActive(true);
        removeCardButton.gameObject.SetActive(true);

        //DiscardHand();
        //DrawCards();
        UpdateUI();
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);

            CardInstance temp = deck[i];
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

        //cardButton1.GetComponentInChildren<TMP_Text>().text = hand[0].cardName;
        //cardButton2.GetComponentInChildren<TMP_Text>().text = hand[1].cardName;
        //cardButton3.GetComponentInChildren<TMP_Text>().text = hand[2].cardName;
        SetcardButtonText(cardButton1, hand[0]);
        SetcardButtonText(cardButton2, hand[1]);
        SetcardButtonText(cardButton3, hand[2]);
    }

    void DrawOneCard()
    {
        if (deck.Count == 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }

        CardInstance card = deck[0];
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

        CardInstance card = hand[handIndex];

        if(card.cardData.cardType==CardType.Attack)
        {
            enemyHp -= card.GetDamage();
            AddLog(card.GetCardName() + " 사용" + card.GetDamage() + " 데미지");
        }
        else if(card.cardData.cardType==CardType.Heal)
        {
            int beforeHp = playerHp;

            playerHp += card.GetHeal();

            if (playerHp > playerMaxHp)
            {
                playerHp = playerMaxHp;
            }

            int actualHeal = playerHp - beforeHp;

            AddLog(card.GetCardName() + " 사용 HP " + actualHeal + " 회복");
        }
        else if (card.cardData.cardType == CardType.Defense)
        {
            playerDefense += card.GetDefense();
            AddLog(card.GetCardName() + " 사용! 방어도 " + card.GetDefense() + " 획득");
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
                AddLog(currentEnemy.enemyName + "의 특수 공격 " + enemyDamage + " 데미지");
                resultText.text = (currentEnemy.enemyName + "의 특수 공격");
            }
            else
            {
                AddLog(currentEnemy.enemyName + "의 일반 공격" + enemyDamage + " 데미지");
            }
        }
        else
        {
            AddLog(currentEnemy.enemyName + "의 공격" + enemyDamage + " 데미지");
        }

        if (playerDefense > 0)
        {
            int blockDamage = enemyDamage;

            playerDefense -= enemyDamage;

            if (playerDefense < 0)
            {
                int remainDamage = -playerDefense;
                blockDamage = enemyDamage - remainDamage;
                playerHp -= remainDamage;
                playerDefense = 0;

                AddLog("방어도로 " + blockDamage + " 피해를 막음");
                AddLog("플레이어가 " + remainDamage + " 피해를 받음");

            }
            else
            {
                AddLog("방어도로 " + blockDamage + " 피해를 막음");
            }
        }
        else
        {
            playerHp -= enemyDamage;
            AddLog("플레이어가 " + enemyDamage + " 피해를 받음");
        }

        if (playerHp <= 0)
        {
            playerHp = 0;
            resultText.text = "패배...";
            AddLog("플레이어 패배...");
        }
    }

    void CheckEnemyDead()
    {
        if(enemyHp<=0)
        {
            HideCardButtons();

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

        AddLog("Stage " + (currentEnemyIndex + 1) + " 시작" + enemies[currentEnemyIndex].enemyName);

        nextStageButton.gameObject.SetActive(false);
        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);
        removeSelectButton1.gameObject.SetActive(false);
        removeSelectButton2.gameObject.SetActive(false);
        removeSelectButton3.gameObject.SetActive(false);

        ShowCardButtons();

        //enemyHp = enemyMaxHps[currentEnemyIndex];
        enemyHp = enemies[currentEnemyIndex].maxHp;
        enemyHpSlider.maxValue=enemies[currentEnemyIndex].maxHp;
        enemyHpSlider.value = enemyHp;
        playerDefense = 0;
        bossTurnCount = 0;

        //resultText.text = enemyNames[currentEnemyIndex] + " 등장";
        //resultText.text = enemies[currentEnemyIndex].enemyName + " 등장";

        usedDeckAction = false;
        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);

        DiscardHand();
        DrawCards();
        UpdateUI();
    }

    void AddLog(string message)
    {
        //battleLog += message + "\n";
        //battleLogText.text = battleLog;
        battleLogs.Add(message);

        if (battleLogs.Count > 5)
        {
            battleLogs.RemoveAt(0);
        }

        battleLogText.text = "";

        for (int i = 0; i < battleLogs.Count; i++)
        {
            battleLogText.text += battleLogs[i] + "\n";
        }
    }

    void SetcardButtonText(Button button, CardInstance card)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        buttonText.text = card.GetCardName() + "\n" + card.GetDescription();
    }

    /*public void UpgradeRandomCard()
    {
        List<CardInstance> allCards = new List<CardInstance>();

        allCards.AddRange(deck);
        allCards.AddRange(hand);
        allCards.AddRange(discardPile);

        List<CardInstance> upgradeableCards = new List<CardInstance>();

        for (int i = 0; i < allCards.Count; i++)
        {
            if (allCards[i].cardData.canUpgrade && allCards[i].isUpgraded == false)
            {
                upgradeableCards.Add(allCards[i]);
            }
        }

        if(upgradeableCards.Count == 0)
        {
            AddLog("강화할 수 있는 카드가 없습니다");
            return;
        }

        CardInstance selectedCard = upgradeableCards[Random.Range(0, upgradeableCards.Count)];

        selectedCard.Upgrade();

        AddLog(selectedCard.GetCardName() + " 강화 완료");
        resultText.text = selectedCard.GetCardName() + " 강화 완료";

        upgradeCardButton.gameObject.SetActive(false);

        UpdateUI();
    }*/

    public void ShowUpgradeChoices()
    {
        if (usedDeckAction)
        {
            AddLog("이미 강화 또는 제거를 진행했습니다.");
            return;
        }

        List<CardInstance> allCards = new List<CardInstance>();

        allCards.AddRange(deck);
        allCards.AddRange(hand);
        allCards.AddRange(discardPile);

        List<CardInstance> upgradeableCards = new List<CardInstance>();

        for(int i = 0; i < allCards.Count; i++)
        {
            if (!allCards[i].isUpgraded)
            {
                upgradeableCards.Add(allCards[i]);
            }
        }

        if(upgradeableCards.Count < 3)
        {
            Debug.Log("카드가 3장 미만이 불가");
            return;
        }

        ShuffleUpgradeableCards(upgradeableCards);

        upgradeCard1 = upgradeableCards[0];
        upgradeCard2 = upgradeableCards[1];
        upgradeCard3 = upgradeableCards[2];

        SetcardButtonText(upgradeSelectButton1, upgradeCard1);
        SetcardButtonText(upgradeSelectButton2, upgradeCard2);
        SetcardButtonText(upgradeSelectButton3, upgradeCard3);

        upgradeSelectButton1.gameObject.SetActive(true);
        upgradeSelectButton2.gameObject.SetActive(true);
        upgradeSelectButton3.gameObject.SetActive(true);
    }

    void ShuffleUpgradeableCards(List<CardInstance> cards)
    {
        for(int i=0;i<cards.Count;i++)
        {
            int randomIndex = Random.Range(i, cards.Count);

            CardInstance temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public void SelectUpgrade1()
    {
        UpgradeCard(upgradeCard1);
    }

    public void SelectUpgrade2()
    {
        UpgradeCard(upgradeCard2);
    }

    public void SelectUpgrade3()
    {
        UpgradeCard(upgradeCard3);
    }

    void UpgradeCard(CardInstance card)
    {
        card.Upgrade();

        usedDeckAction = true;

        AddLog(card.GetCardName() + " 강화");

        upgradeSelectButton1.gameObject.SetActive(false);
        upgradeSelectButton2.gameObject.SetActive(false);
        upgradeSelectButton3.gameObject.SetActive(false);

        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);

        UpdateUI();
    }

    void SetRewardButtonText(Button button, CardData card)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        if (card.cardType == CardType.Attack)
        {
            buttonText.text = card.cardName + "\n" + card.damage + " 데미지";
        }
        else if (card.cardType == CardType.Heal)
        {
            buttonText.text = card.cardName + "\nHp " + card.heal + " 회복";
        }
        else if(card.cardType== CardType.Defense)
        {
            buttonText.text = card.cardName + "\n방어도 " + card.defense + " 획득";
        }
    }

    void ShowCardButtons()
    {
        cardButton1.gameObject.SetActive(true);
        cardButton2.gameObject.SetActive(true);
        cardButton3.gameObject.SetActive(true);
    }

    void HideCardButtons()
    {
        cardButton1.gameObject.SetActive(false);
        cardButton2.gameObject.SetActive(false);
        cardButton3.gameObject.SetActive(false);
    }

    public void ShowRemoveChoices()
    {
        if (usedDeckAction)
        {
            AddLog("이미 강화 또는 삭제를 진행했습니다");
            return;
        }

        Debug.Log("showRemoveshoice 실행");

        List<CardInstance> allCards = new List<CardInstance>();

        allCards.AddRange(deck);
        allCards.AddRange(hand);
        allCards.AddRange(discardPile);

        Debug.Log("전체 카드수 : " + allCards.Count);

        if (allCards.Count < 3)
        {
            Debug.Log("카드 부족해서 실패");
            return;
        }

        ShuffleUpgradeableCards(allCards);

        removeCard1 = allCards[0];
        removeCard2 = allCards[1];
        removeCard3 = allCards[2];

        SetcardButtonText(removeSelectButton1, removeCard1);
        SetcardButtonText(removeSelectButton2, removeCard2);
        SetcardButtonText(removeSelectButton3, removeCard3);

        removeSelectButton1.gameObject.SetActive(true);
        removeSelectButton2.gameObject.SetActive(true);
        removeSelectButton3.gameObject.SetActive(true);

        Debug.Log("버튼1 상태 : " + removeSelectButton1.gameObject.activeSelf);
        Debug.Log("버튼1 위치 : " + removeSelectButton1.transform.position);
    }

    public void SelectRemove1()
    {
        RemoveCard(removeCard1);
    }

    public void SelectRemove2()
    {
        RemoveCard(removeCard2);
    }

    public void SelectRemove3()
    {
        RemoveCard(removeCard3);
    }

    void RemoveCard(CardInstance card)
    {
        deck.Remove(card);
        hand.Remove(card);
        discardPile.Remove(card);

        usedDeckAction = true;

        AddLog(card.GetCardName() + " 제거!");

        removeSelectButton1.gameObject.SetActive(false);
        removeSelectButton2.gameObject.SetActive(false);
        removeSelectButton3.gameObject.SetActive(false);

        removeCardButton.gameObject.SetActive(false);
        upgradeCardButton.gameObject.SetActive(false);

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

        playerHpSlider.value = playerHp;

        if (currentEnemyIndex < enemies.Length)
        {
            enemyHpSlider.maxValue = enemies[currentEnemyIndex].maxHp;
            enemyHpSlider.value = enemyHp;
        }
        else
        {
            enemyHpSlider.value = 0;
        }
    }
}
