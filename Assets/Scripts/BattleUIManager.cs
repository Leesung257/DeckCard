using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



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

    public GameObject shopPanel;

    public void Initialize(
        TMP_Text playerHpText,
        TMP_Text enemyHpText,
        TMP_Text resultText,
        TMP_Text playerDefenseText,
        TMP_Text deckCountText,
        TMP_Text handCountText,
        TMP_Text discardCountText,
        TMP_Text totalCardCountText,
        TMP_Text stageText,
        TMP_Text enemyAttackText,
        TMP_Text battleLogText,
        TMP_Text goldText,
        Button cardButton1,
        Button cardButton2,
        Button cardButton3,
        Button rewardButton1,
        Button rewardButton2,
        Button rewardButton3,
        Button nextStageButton,
        Button upgradeCardButton,
        Button upgradeSelectButton1,
        Button upgradeSelectButton2,
        Button upgradeSelectButton3,
        Button removeCardButton,
        Button removeSelectButton1,
        Button removeSelectButton2,
        Button removeSelectButton3,
        Button eventHealButton,
        Button eventUpgradeButton,
        Button eventRemoveButton,
        Button shopAttackCardButton,
        Button shopDefenseCardButton,
        Button shopStrongAttackButton,
        Button shopHealCardButton,
        GameObject shopPanel)
    {
        this.playerHpText = playerHpText;
        this.enemyHpText = enemyHpText;
        this.resultText = resultText;
        this.playerDefenseText = playerDefenseText;
        this.deckCountText = deckCountText;
        this.handCountText = handCountText;
        this.discardCountText = discardCountText;
        this.totalCardCountText = totalCardCountText;
        this.stageText = stageText;
        this.enemyAttackText = enemyAttackText;
        this.battleLogText = battleLogText;
        this.goldText = goldText;
        this.cardButton1 = cardButton1;
        this.cardButton2 = cardButton2;
        this.cardButton3 = cardButton3;
        this.rewardButton1 = rewardButton1;
        this.rewardButton2 = rewardButton2;
        this.rewardButton3 = rewardButton3;
        this.nextStageButton = nextStageButton;
        this.upgradeCardButton = upgradeCardButton;
        this.upgradeSelectButton1 = upgradeSelectButton1;
        this.upgradeSelectButton2 = upgradeSelectButton2;
        this.upgradeSelectButton3 = upgradeSelectButton3;
        this.removeCardButton = removeCardButton;
        this.removeSelectButton1 = removeSelectButton1;
        this.removeSelectButton2 = removeSelectButton2;
        this.removeSelectButton3 = removeSelectButton3;
        this.eventHealButton = eventHealButton;
        this.eventUpgradeButton = eventUpgradeButton;
        this.eventRemoveButton = eventRemoveButton;
        this.shopAttackCardButton = shopAttackCardButton;
        this.shopDefenseCardButton = shopDefenseCardButton;
        this.shopStrongAttackButton = shopStrongAttackButton;
        this.shopHealCardButton = shopHealCardButton;
        this.shopPanel = shopPanel;
    }


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

    public void UpdateUI(int currentEnemyIndex, EnemyData[] enemies, int playerHp, int playerDefense, int enemyHp, int enemyDefense, int deckCount, int handCount, int discardCount, int gold)
    {
        UpdateStageUI(currentEnemyIndex, enemies);
        UpdatePlayerUI(playerHp, playerDefense);
        UpdateEnemyUI(currentEnemyIndex, enemies, enemyHp, enemyDefense);
        UpdateCardCountUI(deckCount,handCount,discardCount);
        UpdateGoldUI(gold);
    }

    public void UpdateStageUI(int currentEnemyIndex, EnemyData[] enemies)
    {
        if (currentEnemyIndex < enemies.Length)
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

    public void UpdateEnemyUI(int currentEnemyIndex, EnemyData[] enemies, int enemyHp, int enemyDefense)
    {
        if(currentEnemyIndex<enemies.Length)
        {
            enemyHpText.text = enemies[currentEnemyIndex].enemyName + " HP : " + enemyHp + " / 방어도 : " + enemyDefense;
            enemyAttackText.text = "적 공격력 : " + enemies[currentEnemyIndex].attackDamage;
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

    public void SetResultText(string message)
    {
        resultText.text = message;
    }

    public void SetResultTextActive(bool active)
    {
        resultText.gameObject.SetActive(active);
    }

    public void ShowEnemyIntent(string intentText)
    {
        resultText.text = intentText;
    }

    public void SetButtonsActive(Button[] buttons, bool active)
    {
        for(int i=0;i<buttons.Length;i++)
        {
            buttons[i].gameObject.SetActive(active);
        }
    }

    private Button[] GetCardButtons()
    {
        return new Button[] { cardButton1, cardButton2, cardButton3 };
    }
    private Button[] GetRewardButtons()
    {
        return new Button[] { rewardButton1, rewardButton2, rewardButton3 };
    }

    private Button[] GetUpgradeSelectButtons()
    {
        return new Button[]
        {
            upgradeSelectButton1, upgradeSelectButton2, upgradeSelectButton3
        };
    }

    private Button[] GetRemoveSelectButtons()
    {
        return new Button[]
        {
            removeSelectButton1, removeSelectButton2, removeSelectButton3
        };
    }

    private Button[] GetEventButtons()
    {
        return new Button[]
        {
            eventHealButton, eventRemoveButton, eventUpgradeButton
        };
    }

    public void ShowCardButtons()
    {
        SetButtonsActive(GetCardButtons(), true);
    }

    public void HideCardButtons()
    {
        SetButtonsActive(GetCardButtons(), false);
    }

    public void ShowEnemyText()
    {
        enemyHpText.gameObject.SetActive(true);
        enemyAttackText.gameObject.SetActive(true);
    }

    public void HideEnemyText()
    {
        enemyHpText.gameObject.SetActive(false);
        enemyAttackText.gameObject.SetActive(false);
    }

    public void HideUpgradeSelectButtons()
    {
        SetButtonsActive(GetUpgradeSelectButtons(), false);
    }

    public void ShowUpgradeSelectButtons()
    {
        SetButtonsActive(GetUpgradeSelectButtons(), true);
    }

    public void HideRemoveSelectButtons()
    {
        SetButtonsActive(GetRemoveSelectButtons(), false);
    }

    public void ShowRemoveSelectButtons()
    {
        SetButtonsActive(GetRemoveSelectButtons(), true);
    }
   
    public void HideDeckActionButtons()
    {
        upgradeCardButton.gameObject.SetActive(false);
        removeCardButton.gameObject .SetActive(false);
    }
    public void HideAllChoiceButtons()
    {
        HideRewardButtons();
        HideEventButtons();

        HideDeckActionButtons();

        HideUpgradeSelectButtons();
        HideRemoveSelectButtons();

        SetNextStageButtonActive(false);

        ShowEnemyText();
    }

    public void SetNextStageButtonActive(bool active)
    {
        nextStageButton.gameObject.SetActive(active);
    }

    private string FormatCardButtonText(CardInstance card)
    {
        return card.GetRarityText()
            + "\n" + card.GetCardName()
            + "\n" + card.GetDescription();
    }

    public void SetCardButtonText(Button button,CardInstance card)
    {
        TMP_Text buttonText=button.GetComponentInChildren<TMP_Text>();
        buttonText.text = FormatCardButtonText(card);
    }

    public void SetRewardButtonText(Button button,CardData card)
    {
        TMP_Text buttonText=button.GetComponentInChildren<TMP_Text>();

        buttonText.text = "[" + card.rarity.ToString() + "]"
            + "\n" + card.cardName
            + "\n" + card.GetDescription();
    }

    public void UpdateCardButtonTexts(List<CardInstance> hand)
    {
        Button[] cardButtons = GetCardButtons();

        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (i < hand.Count)
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

    public void ShowRewardButtons(IReadOnlyList<CardData> rewardCards)
    {
        Button[] rewardButtons = GetRewardButtons();
        SetButtonsActive(rewardButtons, true);

        for(int i=0;i<rewardButtons.Length;i++)
        {
            SetRewardButtonText(rewardButtons[i], rewardCards[i]);
        }
    }

    public void HideRewardButtons()
    {
        SetButtonsActive(GetRewardButtons(), false);
    }

    public void ShowEventButtons()
    {
        SetButtonsActive(GetEventButtons(), true);
        HideCardButtons();
    }

    public void HideEventButtons()
    {
        SetButtonsActive(GetEventButtons(), false);
    }

    public void ShowShopPanel()
    {
        shopPanel.SetActive(true);
    }

    public void HideShopPanel()
    {
        shopPanel.SetActive(false);
    }

    public void PlayButtonAnimation(Button button, System.Action action)
    {
        StartCoroutine(PlayButtonAnimationCoroutine(button, action));
    }

    private IEnumerator PlayButtonAnimationCoroutine(
        Button button,
        System.Action action)
    {
        Vector3 originalScale = button.transform.localScale;

        button.transform.localScale = originalScale * 1.2f;

        yield return new WaitForSeconds(0.15f);

        button.transform.localScale = originalScale;

        action?.Invoke();
    }
}
