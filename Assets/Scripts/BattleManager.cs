using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.IO;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //Constant
    const int EventHealAmount = 20;

    const int CommonRewardChance = 70;
    const int RareRewardChance = 95;

    const int AttackCardPrice = 30;
    const int StrongAttackCardPrice = 50;
    const int DefenseCardPrice = 25;
    const int HealCardPrice = 25;

    //UI Text
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

    //Button
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

    public Button removeCardButton;
    public Button removeSelectButton1;
    public Button removeSelectButton2;
    public Button removeSelectButton3;

    public Button eventHealButton;
    public Button eventUpgradeButton;
    public Button eventRemoveButton;

    public Button shopAttackCardButton;
    public Button shopDefenseCardButton;
    public Button shopStrongAttackButton;
    public Button shopHealCardButton;

    //Cards / Enemies / Shop
    public CardData attackCard;
    public CardData strongAttackCard;
    public CardData healCard;
    public CardData defenseCard;

    public List<CardData> commonCards;
    public List<CardData> rareCards;
    public List<CardData> epicCards;

    public EnemyData[] enemies;

    public GameObject shopPanel;

    [SerializeField] private BattleUIManager battleUIManager;

    //Battle
    public int playerHp = 100;
    public int playerMaxHp = 100;
    public int playerDefense = 0;

    public int enemyHp = 50;
    public int enemyDefense = 0;

    int currentEnemyIndex = 0;
    private int bossTurnCount = 0;
    private int enemyAttackBonus = 0;

    private EnemyActionData nextEnemyAction;
    private bool isNextBossSpecialAttack = false;

    public int gold = 0;

    public int eventChance = 50;
    public int shopChance = 30;

    //Cards State
    List<CardInstance> deck = new List<CardInstance>();
    List<CardInstance> hand = new List<CardInstance>();
    List<CardInstance> discardPile = new List<CardInstance>();

    private readonly List<CardData> rewardCards = new List<CardData>();
    private readonly List<CardInstance> upgradeCards = new List<CardInstance>();
    private readonly List<CardInstance> removeCards = new List<CardInstance>();

    //Rewards / Events / Shop state
    bool isEventStage = false;
    bool isChoosingReward = false;
    bool usedDeckAction = false;

    void Start()
    {
        Debug.Assert(battleUIManager != null, "BattleUIManager 연결 실패");

        InitializeBattleLog();
        InitializeUIState();
        InitializeFirstStage();
    }

    void InitializeBattleLog()
    {
        resultText.text = "";
        battleUIManager.InitializeBattleLog();
    }

    void InitializeUIState()
    {
        HideRewardButtons();
        nextStageButton.gameObject.SetActive(false);
        HideDeckActionButtons();
        HideUpgradeSelectButtons();
        HideRemoveSelectButtons();
        HideEventButtons();
        shopPanel.SetActive(false);
    }

    void InitializeFirstStage()
    {
        enemyHp = enemies[currentEnemyIndex].maxHp;

        MakeDeck();
        ShuffleDeck();
        DrawCards(); 
        
        DecideNextEnemyAction();

        UpdateUI();
    }

    // Battle Setup
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

    void EndPlayerTurnAfterCard()
    {
        DiscardHand();

        bool enemyDead = CheckEnemyDead();

        if (enemyDead)
        {
            UpdateUI();
            return;
        }

        if (currentEnemyIndex < enemies.Length && enemyHp > 0 && playerHp > 0)
        {
            ExecuteEnemyTurn();
        }

        if (currentEnemyIndex < enemies.Length && playerHp > 0)
        {
            DrawCards();
        }

        UpdateUI();
    }

    bool CheckEnemyDead()
    {
        if (enemyHp > 0)
        {
            return false;
        }

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

            HideEnemyText();

            resultText.text = "보스 처치! Game Clear";
        }
        else
        {
            enemyHp = 0;
            gold += rewardGold;
            AddLog(rewardGold + " 골드 획득");

            HideEnemyText();

            resultText.text = "카드 보상을 선택하세요";
            ShowRewardButtons();
        }

        return true;
    }

    public void GoToNextStage()
    {
        isEventStage = false;
        HideEventButtons();

        if (currentEnemyIndex >= enemies.Length)
        {
            return;
        }

        AddLog("Stage " + (currentEnemyIndex + 1) + " 시작" + enemies[currentEnemyIndex].enemyName);

        PrepareStageUI();
        ResetStageBattleState();
        PreparePlayerForNextStage();

        DecideNextEnemyAction();
        UpdateUI();
    }

    void PrepareStageUI()
    {
        nextStageButton.gameObject.SetActive(false);
        HideDeckActionButtons();
        HideRemoveSelectButtons();

        ShowCardButtons();
        ShowEnemyText();
        resultText.gameObject.SetActive(true);
    }

    void ResetStageBattleState()
    {
        enemyHp = enemies[currentEnemyIndex].maxHp;
        playerDefense = 0;
        bossTurnCount = 0;
        enemyDefense = 0;
        enemyAttackBonus = 0;
        nextEnemyAction = null;
        isNextBossSpecialAttack = false;
    }

    void PreparePlayerForNextStage()
    {
        usedDeckAction = false;

        DiscardHand();
        DrawCards();
    }

    // Deck / Hand
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
        for(int i = 0; i < 3; i++)
        {
            bool drawSuccess = TryDrawOneCard();

            if (drawSuccess == false)
            {
                AddLog("더 이상 뽑을 카드가 없습니다");
                break;
            }
        }

        UpdateCardButtonTexts();
    }

    bool TryDrawOneCard()
    {
        if (deck.Count == 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }

        if (deck.Count == 0)
        {
            return false;
        }

        CardInstance card = deck[0];
        deck.RemoveAt(0);
        hand.Add(card);

        return true;
    }

    void DiscardHand()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            discardPile.Add(hand[i]);
        }

        hand.Clear();
    }

    void ShuffleCards(List<CardInstance> cards)
    {
        for(int i=0;i<cards.Count;i++)
        {
            int randomIndex = Random.Range(i, cards.Count);

            CardInstance temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    List<CardInstance> GetAllPlayerCards()
    {
        List<CardInstance> allCards = new List<CardInstance>();

        allCards.AddRange(deck);
        allCards.AddRange(hand);
        allCards.AddRange(discardPile);

        return allCards;
    }

    List<CardInstance> GetUpgradeableCards()
    {
        List<CardInstance> allCards = GetAllPlayerCards();
        List<CardInstance> upgradeableCards = new List<CardInstance>();

        for (int i = 0; i < allCards.Count; i++)
        {
            if (!allCards[i].isUpgraded)
            {
                upgradeableCards.Add(allCards[i]);
            }
        }

        return upgradeableCards;
    }

    //Player Card Action
    public void UseCard1()
    {
        StartCoroutine(PlayCardAnimation(cardButton1, () => UseCard(0)));
    }

    public void UseCard2()
    {
        StartCoroutine(PlayCardAnimation(cardButton2, () => UseCard(1)));
    }

    public void UseCard3()
    {
        StartCoroutine(PlayCardAnimation(cardButton3, () => UseCard(2)));
    }

    void UseCard(int handIndex)
    {
        if (CanUseCard(handIndex) == false)
        {
            return;
        }

        CardInstance card = hand[handIndex];

        AddLog(card.GetCardName() + " 사용");

        ApplyCardEffects(card);

        EndPlayerTurnAfterCard();
    }

    void ApplyCardEffects(CardInstance card)
    {
        ApplyCardAttack(card);
        ApplyCardHeal(card);
        ApplyCardDefense(card);
        ApplyCardSelfDamage(card);
    }

    bool CanUseCard(int handIndex)
    {
        return playerHp > 0
            && currentEnemyIndex < enemies.Length
            && isChoosingReward == false
            && enemyHp > 0
            && handIndex >= 0
            && handIndex < hand.Count;
    }

    void ApplyCardAttack(CardInstance card)
    {
        if (card.cardData.cardType != CardType.Attack)
        {
            return;
        }

        int totalDamage = CalculateCardDamage(card);
        
        AddLog(totalDamage + " 데미지 시도");
        DealDamageToEnemy(totalDamage);

    }

    int CalculateCardDamage(CardInstance card)
    {
        if (card.cardData.multiHit == false)
        {
            return card.GetDamage();
        }

        int totalDamage = 0;

        for(int i = 0; i < card.cardData.hitcount; i++)
        {
            totalDamage += card.GetDamage();
        }

        return totalDamage;
    }

    void ApplyCardHeal(CardInstance card)
    {
        int healAmount = card.GetHeal();

        if (healAmount <= 0)
        {
            return;
        }

        int beforeHp = playerHp;

        playerHp += healAmount;
        ClampPlayerHp();

        int actualHeal = playerHp - beforeHp;

        AddLog(actualHeal + " Hp 회복");
    }

    void ApplyCardDefense(CardInstance card)
    {
        int defenseAmount = card.GetDefense();

        if (defenseAmount <= 0)
        {
            return;
        }

        playerDefense += defenseAmount;
        AddLog(card.GetDefense() + "방어도 획득");
    }

    void ApplyCardSelfDamage(CardInstance card)
    {
        int selfDamage = card.cardData.selfDamage;

        if (selfDamage <= 0)
        {
            return;
        }

        playerHp -= selfDamage;
        ClampPlayerHp();

        AddLog("자신에게" + card.cardData.selfDamage + " 피해");
    }

    void ClampPlayerHp()
    {
        playerHp = Mathf.Clamp(playerHp, 0, playerMaxHp);
    }

    void DealDamageToEnemy(int damage)
    {
        DamageResult damageResult = CalculateDamageResult(damage, enemyDefense);

        enemyDefense = damageResult.remainingDefense;
        enemyHp -= damageResult.hpDamage;

        if (damageResult.blockDamage > 0 && damageResult.hpDamage <= 0)
        {
            AddLog("적 방어도가 피해를 막음");
        }

        if (damageResult.blockDamage > 0 && damageResult.hpDamage > 0)
        {
            AddLog("적 방어도 파괴");
        }

        if (damageResult.hpDamage > 0)
        {
            AddLog(damageResult.hpDamage + " 피해");
        }
    }

    

    //Enemy Action
    void ExecuteEnemyTurn()
    {
        EnemyData currentEnemy = enemies[currentEnemyIndex];

        if (currentEnemy.isBoss)
        {
            bossTurnCount++;
        }

        if (isNextBossSpecialAttack)
        {
            ExecuteBossSpecialAttack(currentEnemy);
            DecideNextEnemyAction();
            UpdateUI();

            return;
        }

        if (nextEnemyAction != null)
        {
            ExecuteEnemyAction(nextEnemyAction);
        }

        DecideNextEnemyAction();

        UpdateUI();
    }

    void ExecuteBossSpecialAttack(EnemyData currentEnemy)
    {
        int specialDamage = Random.Range(currentEnemy.specialAttackDamage - 5, currentEnemy.specialAttackDamage + 5);

        specialDamage += enemyAttackBonus;

        AddLog(currentEnemy.enemyName + "의 특수 공격" + specialDamage + " 데미지");
        resultText.text = currentEnemy.enemyName + "의 특수 공격";

        DealDamageToPlayer(specialDamage);
    }

    EnemyActionData GetRandomEnemyAction(EnemyData enemy)
    {
        int totalChance = 0;

        for (int i = 0; i < enemy.actions.Count; i++)
        {
            totalChance += enemy.actions[i].chance;
        }

        int randomValue = Random.Range(0, totalChance);

        int currentChance = 0;

        for (int i = 0; i < enemy.actions.Count; i++)
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
        switch (action.actionType)
        {
            case EnemyActionType.Attack:
                ExecuteEnemyAttack(action);
                break;

            case EnemyActionType.Defense:
                ExecuteEnemyDefense(action);
                break;

            case EnemyActionType.MultiAttack:
                ExecuteEnemyMultiAttack(action);
                break;

            case EnemyActionType.IgnoreDefenseAttack:
                ExecuteEnemyIgnoreDefenseAttack(action);
                break;

            case EnemyActionType.AttackBuff:
                ExecuteEnemyAttackBuff(action);
                break;
        }
    }

    void DealDamageToPlayer(int enemyDamage)
    {
        DamageResult damageResult = CalculateDamageResult(enemyDamage, playerDefense);

        playerDefense = damageResult.remainingDefense;
        playerHp -= damageResult.hpDamage;

        if (damageResult.blockDamage > 0)
        {
            AddLog("방어도가 " + damageResult.blockDamage + " 피해를 막음");
        }

        if (damageResult.hpDamage > 0)
        {
            AddLog("플레이어가 " + damageResult.hpDamage + " 피해를 받음");
        }

        if (playerHp <= 0)
        {
            playerHp = 0;

            resultText.text = "패배...";
            AddLog("플레이어 패배...");
        }
    }

    void ExecuteEnemyAttack(EnemyActionData action)
    {
        int damage = action.damage + enemyAttackBonus;

        AddLog(action.actionName);
        DealDamageToPlayer(damage);
    }

    void ExecuteEnemyDefense(EnemyActionData action)
    {
        enemyDefense += action.defense;

        AddLog(action.actionName);
        AddLog("적 방어도 " + action.defense + " 증가");
    }

    void ExecuteEnemyMultiAttack(EnemyActionData action)
    {
        int totalDamage = 0;

        for (int i = 0; i < action.hitCount; i++)
        {
            totalDamage += action.damage + enemyAttackBonus;
        }

        AddLog(action.actionName);
        DealDamageToPlayer(totalDamage);
    }

    void ExecuteEnemyIgnoreDefenseAttack(EnemyActionData action)
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

        DealDamageToPlayer(damage);
    }

    void ExecuteEnemyAttackBuff(EnemyActionData action)
    {
        enemyAttackBonus += 3;

        AddLog(action.actionName);
        AddLog("적 공격력 3 증가");
    }

    void DecideNextEnemyAction()
    {
        if (currentEnemyIndex >= enemies.Length)
        {
            return;
        }

        EnemyData currentEnemy = enemies[currentEnemyIndex];

        ClearNextEnemyAction();

        if (ShouldPrepareBossSpecialAttack(currentEnemy))
        {
            PrepareBossSpecialAttack(currentEnemy);
            return;
        }

        PrepareRandomEnemyAction(currentEnemy);
    }

    void ClearNextEnemyAction()
    {
        isNextBossSpecialAttack = false;
        nextEnemyAction = null;
    }

    bool ShouldPrepareBossSpecialAttack(EnemyData currentEnemy)
    {
        if (currentEnemy.isBoss == false)
        {
            return false;
        }

        int nextBossTurn = bossTurnCount + 1;

        return currentEnemy.specialAttackTurn > 0 && nextBossTurn % currentEnemy.specialAttackTurn == 0;
    }

    void PrepareBossSpecialAttack(EnemyData currentEnemy)
    {
        isNextBossSpecialAttack = true;
        resultText.text = "적 행동 : " + currentEnemy.enemyName + " 특수 공격(" + currentEnemy.specialAttackDamage + ")";
    }

    void PrepareRandomEnemyAction(EnemyData currentEnemy)
    {
        nextEnemyAction = GetRandomEnemyAction(currentEnemy);
        resultText.text = "다음 행동 : " + nextEnemyAction.actionName;
    }

    // Reward
    void HideRewardButtons()
    {
        isChoosingReward = false;

        SetButtonsActive(GetRewardButtons(), false);
    }

    void ShowRewardButtons()
    {
        isChoosingReward = true;
        Button[] rewardButtons = GetRewardButtons();

        rewardCards.Clear();

        for (int i = 0; i < rewardButtons.Length; i++)
        {
            rewardCards.Add(GetRandomRewardCard());
        }

        SetButtonsActive(rewardButtons, true);

        for(int i = 0;i < rewardButtons.Length;i++)
        {
            SetRewardButtonText(rewardButtons[i], rewardCards[i]);
        }
    }

    CardData GetRandomRewardCard()
    {
        int randomNumber = Random.Range(0, 100);

        if (randomNumber < CommonRewardChance)
        {
            return commonCards[Random.Range(0, commonCards.Count)];
        }
        else if (randomNumber < RareRewardChance)
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
        SelectRewardByIndex(0);
    }

    public void SelectReward2()
    {
        SelectRewardByIndex(1);
    }

    public void SelectReward3()
    {
        SelectRewardByIndex(2);
    }

    void SelectRewardByIndex(int index)
    {
        if (index < 0 || index >= rewardCards.Count)
        {
            return;
        }

        Button[] rewardButtons = GetRewardButtons();

        if (index >= rewardButtons.Length)
        {
            return;
        }

        StartCoroutine(PlayCardAnimation(rewardButtons[index], () => SelectReward(rewardCards[index])));
    }

    void SelectReward(CardData selectCard)
    {
        AddRewardCard(selectCard);
        FinishRewardSelection(selectCard);
        DecideShopOrContinueAfterReward();
        UpdateUI();
    }

    void AddRewardCard(CardData selectCard)
    {
        discardPile.Add(new CardInstance(selectCard));
    }

    void FinishRewardSelection(CardData selectCard)
    {
        HideRewardButtons();

        usedDeckAction = false;

        resultText.text = selectCard.cardName + " 획득";
        AddLog(selectCard.cardName + " 획득");

        nextStageButton.gameObject.SetActive(false);
        HideDeckActionButtons();
    }

    void DecideShopOrContinueAfterReward()
    {
        int randomValue = Random.Range(0, 100);

        if (randomValue < shopChance)
        {
            ShowShop();
        }
        else
        {
            DecideEventOrNextStageAfterReward();
        }
    }

    void DecideEventOrNextStageAfterReward()
    {
        int randomValue = Random.Range(0, 100);

        if (randomValue < eventChance)
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

    

    //Deck Action - Upgrade / Remove
    public void ShowUpgradeChoices()
    {
        if (usedDeckAction)
        {
            AddLog("이미 강화 또는 제거를 진행했습니다.");
            return;
        }

        List<CardInstance> upgradeableCards = GetUpgradeableCards();

        Button[] upgradeButtons = GetUpgradeSelectButtons();

        if(upgradeableCards.Count < upgradeButtons.Length)
        {
            Debug.Log("카드가 3장 미만이 불가");
            return;
        }

        ShuffleCards(upgradeableCards);

        upgradeCards.Clear();

        for(int i = 0; i < upgradeButtons.Length; i++)
        {
            upgradeCards.Add(upgradeableCards[i]);
            SetCardButtonText(upgradeButtons[i], upgradeCards[i]);
        }

        ShowUpgradeSelectButtons();
    }

    public void SelectUpgrade1()
    {
        SelectUpgradeByIndex(0);
    }

    public void SelectUpgrade2()
    {
        SelectUpgradeByIndex(1);
    }

    public void SelectUpgrade3()
    {
        SelectUpgradeByIndex(2);
    }

    void SelectUpgradeByIndex(int index)
    {
        if (index < 0 || index >= upgradeCards.Count)
        {
            return;
        }

        Button[] upgradeButtons = GetUpgradeSelectButtons();

        if (index >= upgradeButtons.Length)
        {
            return;
        }

        StartCoroutine(PlayCardAnimation(upgradeButtons[index], () => UpgradeCard(upgradeCards[index])));
    }

    void UpgradeCard(CardInstance card)
    {
        card.Upgrade();

        usedDeckAction = true;

        AddLog(card.GetCardName() + " 강화");

        HideUpgradeSelectButtons();

        HideDeckActionButtons();

        if(isEventStage)
        {
            EndEventStage();
        }

        UpdateUI();
    }

    bool CanShowUpgradeChoices()
    {
        if (usedDeckAction)
        {
            return false;
        }

        List<CardInstance> upgradeableCards=GetUpgradeableCards();

        return upgradeableCards.Count >= GetUpgradeSelectButtons().Length;
    }

    

    public void ShowRemoveChoices()
    {
        if (usedDeckAction)
        {
            AddLog("이미 강화 또는 삭제를 진행했습니다");
            return;
        }

        List<CardInstance> allCards = GetAllPlayerCards();

        Button[] removeButtons = GetRemoveSelectButtons();

        if (allCards.Count < removeButtons.Length)
        {
            Debug.Log("카드 부족해서 실패");
            return;
        }

        ShuffleCards(allCards);

        removeCards.Clear();

        for(int i = 0; i < removeButtons.Length; i++)
        {
            removeCards.Add(allCards[i]);
            SetCardButtonText(removeButtons[i], removeCards[i]);
        }

        ShowRemoveSelectButtons();
    }

    public void SelectRemove1()
    {
        SelectRemoveByIndex(0);
    }

    public void SelectRemove2()
    {
        SelectRemoveByIndex(1);
    }

    public void SelectRemove3()
    {
        SelectRemoveByIndex(2);
    }

    void SelectRemoveByIndex(int index)
    {
        if (index < 0 || index >= removeCards.Count)
        {
            return;
        }

        Button[] removeButtons = GetRemoveSelectButtons();

        if(index >= removeButtons.Length)
        {
            return;
        }

        StartCoroutine(PlayCardAnimation(removeButtons[index], () => RemoveCard(removeCards[index])));
    }

    void RemoveCard(CardInstance card)
    {
        deck.Remove(card);
        hand.Remove(card);
        discardPile.Remove(card);

        usedDeckAction = true;

        AddLog(card.GetCardName() + " 제거!");

        HideRemoveSelectButtons();

        HideDeckActionButtons();

        if (isEventStage)
        {
            EndEventStage();
        }

        UpdateUI();
    }

    bool CanShowRemoveChoices()
    {
        if (usedDeckAction)
        {
            return false;
        }

        List<CardInstance> allCards = GetAllPlayerCards();

        return allCards.Count >= GetRemoveSelectButtons().Length;
    }
    


    //Event
    void HideEventButtons()
    {
        SetButtonsActive(GetEventButtons(), false);
    }

    void ShowEventButtons()
    {
        isEventStage = true;

        SetButtonsActive(GetEventButtons(), true);

        HideCardButtons();

        resultText.text = "이벤트를 선택하세요";
        AddLog("이벤트 스테이지 진입");
    }

    public void EventHeal()
    {
        playerHp += EventHealAmount;
        ClampPlayerHp();

        AddLog("이벤트 : HP 20 회복");
        EndEventStage();
    }

    public void EventUpgrade()
    {
        if (CanShowUpgradeChoices() == false)
        {
            AddLog("강화 가능한 카드가 부족합니다");
            return;
        }

        AddLog("이벤트 : 카드 강화 선택");
        HideEventButtons();
        ShowUpgradeChoices();
    }

    public void EventRemove()
    {
        if (CanShowRemoveChoices() == false)
        {
            AddLog("삭제할 카드가 부족합니다");
            return;
        }

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

    //Shop
    void ShowShop()
    {
        shopPanel.SetActive(true);

        HideCardButtons();
        HideEventButtons();
        resultText.gameObject.SetActive(false);

        nextStageButton.gameObject.SetActive(false);
        HideDeckActionButtons();

        resultText.text = "상점에 입장했습니다";
        AddLog("상점 등장");
    }

    public void ExitShop()
    {
        shopPanel.SetActive(false);
        resultText.gameObject.SetActive(true);

        resultText.text = "상점을 나왔습니다.";
        AddLog("상점 종료");

        DecideEventOrNextStageAfterReward();
    }

    void BuyCard(CardData cardData, int price)
    {
        if (CanBuyCard(price) == false)
        {
            AddLog("골드가 부족합니다");
            return;
        }

        PayGold(price);
        AddPurchasedCard(cardData);
        AddPurchaseLog(cardData, price);

        UpdateUI();
    }

    bool CanBuyCard(int price)
    {
        return gold >= price;
    }

    void PayGold(int price)
    {
        gold -= price;
    }

    void AddPurchasedCard(CardData cardData)
    {
        discardPile.Add(new CardInstance(cardData));
    }

    void AddPurchaseLog(CardData cardData, int price)
    {
        AddLog(cardData.cardName + " 구매");
        AddLog(price + " 골드 사용");
    }

    public void BuyAttackCard()
    {
        StartCoroutine(PlayCardAnimation(shopAttackCardButton, () => BuyCard(attackCard, AttackCardPrice)));
    }

    public void BuyStrongAttackCard()
    {
        StartCoroutine(PlayCardAnimation(shopStrongAttackButton, () => BuyCard(strongAttackCard, StrongAttackCardPrice)));
    }

    public void BuyDefenseCard()
    {
        StartCoroutine(PlayCardAnimation(shopDefenseCardButton, () => BuyCard(defenseCard, DefenseCardPrice)));
    }

    public void BuyHealCard()
    {
        StartCoroutine(PlayCardAnimation(shopHealCardButton, () => BuyCard(healCard, HealCardPrice)));
    }

    //Save / Load
    public void SaveGame()
    {
        SaveData saveData = CreateSaveData();

        string json = JsonUtility.ToJson(saveData, true);
        string path = GetSavePath();

        Debug.Log(json);
        Debug.Log("저장 경로: " + path);

        File.WriteAllText(path, json);

        AddLog("게임 저장 완료");
    }

    SaveData CreateSaveData()
    {
        SaveData saveData = new SaveData();

        saveData.gold = gold;
        saveData.playerHp = playerHp;
        saveData.currentStage = currentEnemyIndex + 1;

        AddCardToSaveData(deck, saveData);
        AddCardToSaveData(hand, saveData);
        AddCardToSaveData(discardPile, saveData);

        return saveData;
    }

    string GetSavePath()
    {
        return Application.persistentDataPath + "/save.json";
    }

    void AddCardToSaveData(List<CardInstance> cards, SaveData saveData)
    {
        foreach (CardInstance card in cards)
        {
            CardSaveData cardSaveData = new CardSaveData();

            cardSaveData.cardName = card.cardData.cardName;
            cardSaveData.isUpgraded = card.isUpgraded;

            saveData.cards.Add(cardSaveData);
        }
    }

    public void LoadGame()
    {
        string path = GetSavePath();

        if (File.Exists(path) == false)
        {
            AddLog("저장 파일이 없습니다");
            return;
        }

        SaveData saveData = LoadSaveDataFromFile(path);

        ApplyLoadedGame(saveData);

        AddLog("게임 불러오기 완료");

        UpdateUI();
    }

    SaveData LoadSaveDataFromFile(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    void ApplyLoadedGame(SaveData saveData)
    {
        ApplySaveData(saveData);
        RestoreCardsFromSaveData(saveData);
        ResetBattleStateAfterLoad();

        isChoosingReward = false;
        isEventStage = false;
        usedDeckAction = false;
        HideAllChoiceButtons();
        ShuffleDeck();
        DrawCards();
        DecideNextEnemyAction();
    }

    void ApplySaveData(SaveData saveData)
    {
        gold = saveData.gold;
        playerHp = saveData.playerHp;
        currentEnemyIndex = saveData.currentStage - 1;
    }

    void RestoreCardsFromSaveData(SaveData saveData)
    {
        deck.Clear();
        hand.Clear();
        discardPile.Clear();

        for (int i = 0; i < saveData.cards.Count; i++)
        {
            CardData cardData = FindCardData(saveData.cards[i].cardName);

            if (cardData == null)
            {
                Debug.LogWarning(saveData.cards[i].cardName + " 카드를 찾을 수 없습니다");
                continue;
            }

            CardInstance cardInstance = new CardInstance(cardData);
            cardInstance.isUpgraded = saveData.cards[i].isUpgraded;

            deck.Add(cardInstance);
        }
    }

    void ResetBattleStateAfterLoad()
    {
        ResetStageBattleState();
    }

    CardData FindCardData(string cardName)
    {
        if (attackCard.cardName == cardName)
        {
            return attackCard;
        }

        if (strongAttackCard.cardName == cardName)
        {
            return strongAttackCard;
        }

        if (defenseCard.cardName == cardName)
        {
            return defenseCard;
        }

        if (healCard.cardName == cardName)
        {
            return healCard;
        }

        foreach (CardData card in commonCards)
        {
            if (card.cardName == cardName)
            {
                return card;
            }
        }

        foreach (CardData card in rareCards)
        {
            if (card.cardName == cardName)
            {
                return card;
            }
        }

        foreach (CardData card in epicCards)
        {
            if (card.cardName == cardName)
            {
                return card;
            }
        }

        return null;
    }


    //UI - Text
    void AddLog(string message)
    {
        battleUIManager.AddLog(message);
    }

    void UpdateUI()
    {
        UpdateStageUI();
        UpdatePlayerUI();
        UpdateEnemyUI();
        UpdateCardCountUI();
        UpdateGoldUI();
    }

    void UpdateStageUI()
    {
        battleUIManager.UpdateStageUI(currentEnemyIndex, enemies.Length);
    }

    void UpdatePlayerUI()
    {
        battleUIManager.UpdatePlayerUI(playerHp, playerDefense);
    }

    void UpdateEnemyUI()
    {
        bool hasEnemy = currentEnemyIndex < enemies.Length;

        if (hasEnemy)
        {
            EnemyData currentEnemy = enemies[currentEnemyIndex];

            battleUIManager.UpdateEnemyUI(true, currentEnemy.enemyName, enemyHp, enemyDefense, currentEnemy.attackDamage);
        }
        else
        {
            battleUIManager.UpdateEnemyUI(false, "", 0, 0, 0);
        }
    }

    void UpdateCardCountUI()
    {
        battleUIManager.UpdateCardCountUI(deck.Count, hand.Count, discardPile.Count);
    }

    void UpdateGoldUI()
    {
        battleUIManager.UpdateGoldUI(gold);
    }

    //UI - Button
    string FormatCardButtonText(CardInstance card)
    {
        return card.GetRarityText() + "\n" + card.GetCardName() + "\n" + card.GetDescription();
    }
    void SetCardButtonText(Button button, CardInstance card)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        buttonText.text = FormatCardButtonText(card);
    }

    void SetRewardButtonText(Button button, CardData card)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        buttonText.text = "[" + card.rarity.ToString() + "]" + "\n" + card.cardName + "\n" + card.GetDescription();
    }

    void ShowCardButtons()
    {
        SetButtonsActive(GetCardButtons(), true);
    }

    void HideCardButtons()
    {
        SetButtonsActive(GetCardButtons(), false);
    }

    void HideEnemyText()
    {
        enemyHpText.gameObject.SetActive(false);
        enemyAttackText.gameObject.SetActive(false);
    }

    void ShowEnemyText()
    {
        enemyHpText.gameObject.SetActive(true);
        enemyAttackText.gameObject.SetActive(true);
    }

    void HideUpgradeSelectButtons()
    {
        SetButtonsActive(new Button[] { upgradeSelectButton1, upgradeSelectButton2, upgradeSelectButton3 }, false);
    }

    void ShowUpgradeSelectButtons()
    {
        SetButtonsActive(new Button[] { upgradeSelectButton1, upgradeSelectButton2, upgradeSelectButton3 }, true);

    }

    void HideRemoveSelectButtons()
    {
        SetButtonsActive(new Button[] { removeSelectButton1, removeSelectButton2, removeSelectButton3 }, false);
    }

    void ShowRemoveSelectButtons()
    {
        SetButtonsActive(new Button[] { removeSelectButton1, removeSelectButton2, removeSelectButton3 }, true);
    }

    void HideDeckActionButtons()
    {
        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);
    }

    void SetButtonsActive(Button[] buttons, bool active)
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(active);
        }
    }

    void UpdateCardButtonTexts()
    {
        Button[] cardButtons = GetCardButtons();

        for(int i = 0; i < cardButtons.Length; i++)
        {
            if(i<hand.Count)
            {
                cardButtons[i].gameObject.SetActive(true);
                SetCardButtonText(cardButtons[i], hand[i]);
            }
            else
            {
                cardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void HideAllChoiceButtons()
    {
        HideRewardButtons();
        HideEventButtons();

        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject.SetActive(false);

        HideUpgradeSelectButtons();
        HideRemoveSelectButtons();

        nextStageButton.gameObject.SetActive(false);

        enemyHpText.gameObject.SetActive(true);
        enemyAttackText.gameObject.SetActive(true);
    }

    Button[] GetCardButtons()
    {
        return new Button[] { cardButton1, cardButton2, cardButton3 };
    }

    Button[] GetUpgradeSelectButtons()
    {
        return new Button[] { upgradeSelectButton1, upgradeSelectButton2, upgradeSelectButton3 };
    }

    Button[] GetRewardButtons()
    {
        return new Button[] { rewardButton1, rewardButton2, rewardButton3 };
    }

    Button[] GetRemoveSelectButtons()
    {
        return new Button[] { removeSelectButton1, removeSelectButton2, removeSelectButton3 };
    }

    Button[] GetEventButtons()
    {
        return new Button[] { eventHealButton, eventRemoveButton, eventUpgradeButton };
    }

    IEnumerator PlayCardAnimation(Button button, System.Action action)
    {
        Vector3 originalScale = button.transform.localScale;

        button.transform.localScale = originalScale * 1.2f;

        yield return new WaitForSeconds(0.15f);

        button.transform.localScale = originalScale;

        action?.Invoke();
    }

    struct DamageResult
    {
        public int blockDamage;
        public int hpDamage;
        public int remainingDefense;
    }

    DamageResult CalculateDamageResult(int damage, int defense)
    {
        DamageResult result = new DamageResult();

        if (defense <= 0)
        {
            result.blockDamage = 0;
            result.hpDamage = damage;
            result.remainingDefense = 0;
            return result;
        }

        if (defense >= damage)
        {
            result.blockDamage = damage;
            result.hpDamage = 0;
            result.remainingDefense = defense - damage;
            return result;
        }

        result.blockDamage = defense;
        result.hpDamage = damage - defense;
        result.remainingDefense = 0;
        return result;
    }
}
