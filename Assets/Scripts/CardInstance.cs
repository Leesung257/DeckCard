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
        string description = "";
        if (GetDamage() > 0)
        {
            if (cardData.multiHit)
            {
                description += GetDamage() + " 데미지 x " + cardData.hitcount;
            }
            else
            {
                description += GetDamage() + " 데미지";
            }
        }
        if(GetHeal()>0)
        {
            if (description != "")
            {
                description += "\n";
            }

            description += "HP " + GetHeal() + " 회복";
        }
        if (GetDefense() > 0)
        {
            if (description != "")
            {
                description += "\n";
            }

            description += "방어도 " + GetDefense() + " 획득";
        }

        if(cardData.selfDamage > 0)
        {
            if (description != "")
            {
                description += "\n";
            }

            description += "자신에게 " + cardData.selfDamage + " 피해";
        }

        return description;
    }

    public void Upgrade()
    {
        if (cardData.canUpgrade)
        {
            isUpgraded = true;
        }
    }

    public string GetRarityText()
    {
        return "[" + cardData.rarity.ToString() + "]";
    }
}