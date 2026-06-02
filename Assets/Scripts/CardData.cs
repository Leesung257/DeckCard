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
    public int damage;
    public int heal;
    public int defense;
}