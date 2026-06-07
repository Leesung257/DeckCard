using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.IO;
using UnityEngine.Experimental.AI;

public class BattleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int playerHp = 100;
    public int enemyHp = 50;

    private int bossTurnCount = 0;

    public int eventChance = 50;

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

    public TMP_Text goldText;

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

    public Button eventHealButton;
    public Button eventUpgradeButton;
    public Button eventRemoveButton;

    bool isEventStage = false;

    public CardData attackCard;
    public CardData strongAttackCard;
    public CardData healCard;
    public CardData defenseCard;

    List<CardInstance> deck = new List<CardInstance>();
    List<CardInstance> hand = new List<CardInstance>();
    List<CardInstance> discardPile = new List<CardInstance>();

    public List<CardData> commonCards;
    public List<CardData> rareCards;
    public List<CardData> epicCards;

    public Slider playerHpSlider;
    public Slider enemyHpSlider;

    public int playerMaxHp = 100;

    public EnemyData[] enemies;
    int currentEnemyIndex = 0;

    public int playerDefense = 0;
    public int enemyDefense = 0;

    bool isChoosingReward = false;

    CardData selectUpgradeCard;

    bool usedDeckAction = false;

    private int enemyAttackBonus = 0;

    private EnemyActionData nextEnemyAction;

    private bool isNextBossSpecialAttack = false;

    public int gold = 0;

    public GameObject shopPanel;
    public int shopChance = 30;


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
        HideEventButtons();
        shopPanel.SetActive(false);

        enemyHp = enemies[currentEnemyIndex].maxHp;

        playerHpSlider.maxValue = playerMaxHp;
        playerHpSlider.value = playerHp;

        enemyHpSlider.maxValue=enemies[currentEnemyIndex].maxHp;
        enemyHpSlider.value = enemyHp;

        MakeDeck();
        ShuffleDeck();
        DrawCards();

        DecideNextEnemyAction();

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
        int randomNumber = Random.Range(0, 100);

        if (randomNumber < 70)
        {
            return commonCards[Random.Range(0, commonCards.Count)];
        }
        else if (randomNumber < 95)
        {
            return rareCards[Random.Range(0, rareCards.Count)];
        }
        else
        {
            return epicCards[Random.Range(0,epicCards.Count)];
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

        nextStageButton.gameObject.SetActive(false);
        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);

        DecideShopAfterReward();

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

        int totalDamage = 0;

        AddLog(card.GetCardName() + " 사용");

        if (card.cardData.cardType == CardType.Attack)
        {
            if (card.cardData.multiHit)
            {
                for (int i = 0; i < card.cardData.hitcount; i++)
                {
                    totalDamage += card.GetDamage();
                }
            }
            else
            {
                totalDamage = card.GetDamage();
            }

            DealDamageToEnemy(totalDamage);

            AddLog(totalDamage + " 데미지");
        }
        if (card.GetHeal() > 0)
        {
            int beforeHp = playerHp;

            playerHp += card.GetHeal();

            if (playerHp > playerMaxHp)
            {
                playerHp = playerMaxHp;
            }

            int actualHeal = playerHp - beforeHp;

            AddLog(actualHeal + " Hp 회복");
        }
        if (card.GetDefense() > 0)
        {
            playerDefense += card.GetDefense();
            AddLog(card.GetDefense() + "방어도 획득");
        }

        if(card.cardData.selfDamage > 0)
        {
            playerHp -= card.cardData.selfDamage;

            AddLog("자신에게" + card.cardData.selfDamage + " 피해");

            if (playerHp < 0)
            {
                playerHp = 0;
            }
        }

        DiscardHand();

        CheckEnemyDead();

        if (currentEnemyIndex < enemies.Length && enemyHp > 0 && playerHp > 0)
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

    void DealDamageToEnemy(int damage)
    {
        if (enemyDefense > 0)
        {
            enemyDefense -= damage;

            if(enemyDefense < 0)
            {
                int remainDamage = -enemyDefense;

                enemyDefense = 0;

                enemyHp -= remainDamage;

                AddLog("적 방어도 파괴");
                AddLog(remainDamage + " 피해");
            }
            else
            {
                AddLog("적 방어로 피해를 막음");
            }
        }
        else
        {
            enemyHp -= damage;
        }
    }

    void EnemyAttack()
    {
        EnemyData currentEnemy = enemies[currentEnemyIndex];

        if (currentEnemy.isBoss)
        {
            bossTurnCount++;
        }

        if (isNextBossSpecialAttack)
        {
            int specialDamage = Random.Range(currentEnemy.specialAttackDamage - 5, currentEnemy.specialAttackDamage + 5);

            specialDamage += enemyAttackBonus;

            AddLog(currentEnemy.enemyName + "의 특수 공격" + specialDamage + " 데미지");
            resultText.text = currentEnemy.enemyName + "의 특수 공격";

            EnemyAttackDamage(specialDamage);
            DecideNextEnemyAction();
            UpdateUI();

            return;
        }
        //EnemyActionData action = GetRandomEnemyAction(currentEnemy);
        if (nextEnemyAction != null)
        {
            ExecuteEnemyAction(nextEnemyAction);
        }

        DecideNextEnemyAction();

        UpdateUI();

        return;
    }

    void CheckEnemyDead()
    {
        if(enemyHp<=0)
        {
            HideCardButtons();

            enemyDefense = 0;
            enemyAttackBonus = 0;
            nextEnemyAction = null;
            isNextBossSpecialAttack = false;

            int rewardGold = Random.Range(15, 26);

            currentEnemyIndex++;

            if (currentEnemyIndex >= enemies.Length)
            {
                enemyHp = 0;
                resultText.text = "보스 처치! Game Clear";
            }
            else
            {
                enemyHp = 0;
                gold += rewardGold;
                AddLog(rewardGold + " 골드 획득");
                resultText.text = "카드 보상을 선택하세요";
                ShowRewardButtons();
                //resultText.text = enemyNames[currentEnemyIndex] + " 등장";
            }
        }
    }

    public void GoToNextStage()
    {
        isEventStage = false;
        HideEventButtons();

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
        enemyDefense = 0;
        enemyAttackBonus = 0;

        //resultText.text = enemyNames[currentEnemyIndex] + " 등장";
        //resultText.text = enemies[currentEnemyIndex].enemyName + " 등장";

        usedDeckAction = false;
        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);

        DiscardHand();
        DrawCards();

        DecideNextEnemyAction();

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
        buttonText.text = card.GetRarityText() + "\n" + card.GetCardName() + "\n" + card.GetDescription();
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

        if(isEventStage)
        {
            EndEventStage();
        }

        UpdateUI();
    }

    void SetRewardButtonText(Button button, CardData card)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        buttonText.text = "[" + card.rarity.ToString() + "]" + "\n" + card.cardName + "\n" + card.GetDescription();
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

        if (isEventStage)
        {
            EndEventStage();
        }

        UpdateUI();
    }

    void HideEventButtons()
    {
        eventHealButton.gameObject.SetActive(false);
        eventRemoveButton.gameObject.SetActive(false);
        eventUpgradeButton.gameObject.SetActive(false);
    }

    void ShowEventButtons()
    {
        isEventStage = true;

        eventHealButton.gameObject.SetActive(true);
        eventRemoveButton.gameObject.SetActive(true);
        eventUpgradeButton.gameObject.SetActive(true);

        cardButton1.gameObject.SetActive(false);
        cardButton2.gameObject.SetActive(false);
        cardButton3.gameObject.SetActive(false);

        resultText.text = "이벤트를 선택하세요";
        AddLog("이벤트 스테이지 진입");
    }

    public void EventHeal()
    {
        playerHp += 20;

        if (playerHp > playerMaxHp)
        {
            playerHp = playerMaxHp;
        }

        AddLog("이벤트 : HP 20 회복");
        EndEventStage();
    }

    public void EventUpgrade()
    {
        AddLog("이벤트 : 카드 강화 선택");
        HideEventButtons();
        ShowUpgradeChoices();
    }

    public void EventRemove()
    {
        AddLog("이벤트 : 카드 제거 선택");
        HideEventButtons();
        ShowRemoveChoices();
    }

    void EndEventStage()
    {
        isEventStage = false;

        HideEventButtons();

        nextStageButton.gameObject.SetActive(true);

        resultText.text = "이벤트 완료";
        UpdateUI();
    }

    void DecideNextAfterReward()
    {
        int randomValue=Random.Range(0, 100);

        if(randomValue < eventChance)
        {
            ShowEventButtons();
        }
        else
        {
            nextStageButton.gameObject.SetActive(true);
            resultText.text = "다음 스테이지 입장하세요";
            AddLog("이벤트 없이 다음 스테이지 진행");
        }
    }

    EnemyActionData GetRandomEnemyAction(EnemyData enemy)
    {
        int totalChance = 0;

        for(int i = 0; i < enemy.actions.Count; i++)
        {
            totalChance += enemy.actions[i].chance;
        }

        int randomValue = Random.Range(0, totalChance);

        int currentChance = 0;

        for(int i = 0;i < enemy.actions.Count; i++)
        {
            currentChance += enemy.actions[i].chance;

            if (randomValue < currentChance)
            {
                return enemy.actions[i];
            }
        }

        return enemy.actions[0];
    }

    void ExecuteEnemyAction(EnemyActionData action)
    {
        if (action.actionType == EnemyActionType.Attack)
        {
            int damage = action.damage + enemyAttackBonus;

            AddLog(action.actionName);
            EnemyAttackDamage(damage);
        }
        else if (action.actionType == EnemyActionType.Defense)
        {
            enemyDefense += action.defense;

            AddLog(action.actionName);
            AddLog("적 방어도 " + action.defense + " 증가");
        }
        else if (action.actionType == EnemyActionType.MultiAttack)
        {
            int totalDamage = 0;

            for(int i = 0; i < action.hitCount; i++)
            {
                totalDamage += action.damage + enemyAttackBonus;
            }

            AddLog(action.actionName);

            EnemyAttackDamage(totalDamage);
        }
        else if (action.actionType == EnemyActionType.IgnoreDefenseAttack)
        {
            int ignoreAmount = action.ignoreDefense;

            if (ignoreAmount > playerDefense)
            {
                ignoreAmount = playerDefense;
            }

            playerDefense -= ignoreAmount;
            playerHp -= ignoreAmount;

            AddLog(action.actionName);
            AddLog("방어도 " + ignoreAmount + " 무시");

            int damage = action.damage + enemyAttackBonus;

            EnemyAttackDamage(damage);
        }
        else if (action.actionType == EnemyActionType.AttackBuff)
        {
            enemyAttackBonus += 5;

            AddLog(action.actionName);
            AddLog("적 공격력 5 증가");
        }
    }

    void EnemyAttackDamage(int enemyDamage)
    {
        if (playerDefense > 0)
        {
            int blockDamage = enemyDamage;

            playerDefense -= enemyDamage;

            if(playerDefense < 0)
            {
                int remainDamage = -playerDefense;

                blockDamage = enemyDamage - remainDamage;
                playerHp-= remainDamage;
                playerDefense = 0;

                AddLog("방어도로 " + blockDamage + " 피해를 막음");
                AddLog("플레이어가 " + remainDamage + " 피해를 막음");
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

    void DecideNextEnemyAction()
    {
        if (currentEnemyIndex >= enemies.Length)
        {
            return;
        }

        EnemyData currentEnemy = enemies[currentEnemyIndex];

        isNextBossSpecialAttack = false;
        nextEnemyAction = null;

        if (currentEnemy.isBoss)
        {
            int nextBossturn = bossTurnCount + 1;

            if (currentEnemy.specialAttackTurn > 0 && nextBossturn % currentEnemy.specialAttackTurn == 0)
            {
                isNextBossSpecialAttack = true;

                resultText.text = "다음 행동 : " + currentEnemy.enemyName + " 특수 공격(" + currentEnemy.specialAttackDamage + ")";

                return;
            }
        }

        nextEnemyAction = GetRandomEnemyAction(currentEnemy);

        resultText.text = "다음 행동 : " + nextEnemyAction.actionName;
    }

    void ShowShop()
    {
        shopPanel.SetActive(true);

        HideCardButtons();
        HideEventButtons();

        nextStageButton.gameObject.SetActive(false);
        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);

        resultText.text = "상점에 입장했습니다";
        AddLog("상점 등장");
    }

    public void ExitShop()
    {
        shopPanel.SetActive(false);

        resultText.text = "상점을 나왔습니다.";
        AddLog("상점 종료");

        DecideNextAfterReward();
    }

    void DecideShopAfterReward()
    {
        int randomValue = Random.Range(0, 100);

        if(randomValue < shopChance)
        {
            ShowShop();
        }
        else
        {
            DecideNextAfterReward();
        }
    }

    void BuyCard(CardData cardData, int price)
    {
        if (gold < price)
        {
            AddLog("골드가 부족합니다");
            return;
        }

        gold -= price;

        discardPile.Add(new CardInstance(cardData));

        AddLog(cardData.cardName + " 구매");
        AddLog(price + " 골드 사용");

        UpdateUI();
    }

    public void BuyAttackCard()
    {
        BuyCard(attackCard, 30);
    }
    public void BuyStrongAttackCard()
    {
        BuyCard(strongAttackCard, 50);
    }
    public void BuyDefenseCard()
    {
        BuyCard(defenseCard, 25);
    }
    public void BuyHealCard()
    {
        BuyCard(healCard, 25);
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData();

        saveData.gold = gold;
        saveData.playerHp = playerHp;
        saveData.currentStage = currentEnemyIndex+1;

        foreach(CardInstance card in deck)
        {
            CardSaveData cardSaveData = new CardSaveData();

            cardSaveData.cardName = card.cardData.cardName;
            cardSaveData.isUpgraded = card.isUpgraded;

            saveData.cards.Add(cardSaveData);
        }

        foreach(CardInstance card in hand)
        {
            CardSaveData cardSaveData = new CardSaveData();

            cardSaveData.cardName = card.cardData.cardName;
            cardSaveData.isUpgraded = card.isUpgraded;

            saveData.cards.Add(cardSaveData);
        }

        foreach(CardInstance card in discardPile)
        {
            CardSaveData cardSaveData = new CardSaveData();

            cardSaveData.cardName = card.cardData.cardName;
            cardSaveData.isUpgraded = card.isUpgraded;

            saveData.cards.Add(cardSaveData);
        }

        string json = JsonUtility.ToJson(saveData, true);

        string path = Application.persistentDataPath + "/save.json";

        Debug.Log(json);
        Debug.Log("저장 경로: " + path);

        File.WriteAllText(path, json);

        AddLog("게임 저장 완료");
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.json";

        if (File.Exists(path) == false)
        {
            AddLog("저장 파일이 없습니다");
            return;
        }

        string json = File.ReadAllText(path);

        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        gold = saveData.gold;
        playerHp = saveData.playerHp;
        currentEnemyIndex = saveData.currentStage - 1;

        deck.Clear();
        hand.Clear();
        discardPile.Clear();

        for(int i = 0; i < saveData.cards.Count; i++)
        {
            CardData cardData = FindCardData(saveData.cards[i].cardName);

            if (cardData == null)
            {
                Debug.LogWarning(saveData.cards[i].cardName + "카드를 찾을 수 없습니다");
                continue;
            }

            CardInstance cardInstance = new CardInstance(cardData);
            cardInstance.isUpgraded = saveData.cards[i].isUpgraded;

            deck.Add(cardInstance);
        }

        enemyHp = enemies[currentEnemyIndex].maxHp;
        enemyDefense = 0;
        playerDefense = 0;
        bossTurnCount = 0;
        enemyAttackBonus = 0;

        ShuffleDeck();
        DrawCards();
        DecideNextEnemyAction();

        AddLog("게임 불러오기 완료");

        UpdateUI();
    }

    CardData FindCardData(string cardName)
    {
        if(attackCard.cardName== cardName)
        {
            return attackCard;
        }

        if(strongAttackCard.cardName == cardName)
        {
            return strongAttackCard;
        }

        if(defenseCard.cardName == cardName)
        {
            return defenseCard;
        }

        if(healCard.cardName == cardName)
        {
            return healCard;
        }

        foreach(CardData card in commonCards)
        {
            if(card.cardName== cardName)
            {
                return card;
            }
        }

        foreach(CardData card in rareCards)
        {
            if(card.cardName == cardName)
            {
                return card;
            }
        }

        foreach(CardData card in epicCards)
        {
            if(card.cardName== cardName)
            {
                return card;
            }
        }

        return null;
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
            enemyHpText.text = enemies[currentEnemyIndex].enemyName + " HP : " + enemyHp + " / 방어도 : " + enemyDefense;
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

        goldText.text = "Gold : " + gold;

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
