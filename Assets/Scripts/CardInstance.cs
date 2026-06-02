public class CardInstance
{
    public CardData cardData;
    public bool isUpgraded;

    public CardInstance(CardData data)
    {
        cardData = data;
        isUpgraded = false;
    }  

    public string GetCardName()
    {
        if (isUpgraded)
        {
            return cardData.cardName + "+";
        }

        return cardData.cardName;
    }

    public int GetDamage()
    {
        if (isUpgraded)
        {
            return cardData.damage + cardData.upgradeDamage;
        }

        return cardData.damage;
    }

    public int GetHeal()
    {
        if (isUpgraded)
        {
            return cardData.heal + cardData.upgradeHeal;
        }

        return cardData.heal;
    }

    public int GetDefense()
    {
        if (isUpgraded)
        {
            return cardData.defense + cardData.upgradeDefense;
        }

        return cardData.defense;
    }

    public string GetDescription()
    {
        if (cardData.cardType == CardType.Attack)
        {
            return GetDamage() + " µ¥¹ÌÁö";
        }
        else if(cardData.cardType == CardType.Heal)
        {
            return "HP " + GetHeal() + " È¸º¹";
        }
        else if (cardData.cardType == CardType.Defense)
        {
            return "¹æ¾îµµ " + GetDefense() + "È¹µæ";
        }

        return "";
    }

    public void Upgrade()
    {
        if (cardData.canUpgrade)
        {
            isUpgraded = true;
        }
    }
}