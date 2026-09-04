using UnityEngine;

public enum CardType
{
    Attack,
    Heal,
    Defense
}

[CreateAssetMenu(fileName ="NewCardData", menuName ="Card/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;

    public CardRarity rarity;

    public int damage;
    public int heal;
    public int defense;
    public int selfDamage;

    public bool multiHit;
    public int hitcount;

    public bool canUpgrade = true;
    public int upgradeDamage;
    public int upgradeHeal;
    public int upgradeDefense;

    public string GetDescription()
    {
        string description = "";

        if (damage > 0)
        {
            if (multiHit)
            {
                description += damage + " 데미지 x " + hitcount;
            }
            else
            {
                description += damage + " 데미지";
            }
        }
        if (heal > 0)
        {
            if (description != "")
            {
                description += "\n";
            }

            description += "HP " + heal + " 회복";
        }
        if (defense > 0)
        {
            if(description != "")
            {
                description += "\n";
            }

            description += "방어도 " + defense + " 획득";
        }
        if(selfDamage > 0)
        {
            if (description != "")
            {
                description += "\n";
            }

            description += "자신에게 " + selfDamage + " 피해";
        }
        
        return description;
    }
}