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
        if (cardType == CardType.Attack)
        {
            return damage + " µ¥¹ÌÁö";
        }
        else if (cardType == CardType.Heal)
        {
            return "HP " + heal + " È¸º¹";
        }
        else if (cardType == CardType.Defense) 
        {
            return "¹æ¾îµµ " + defense + " È¹µæ";
        }

        return "";
    }
}