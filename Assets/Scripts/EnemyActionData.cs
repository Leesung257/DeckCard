using UnityEngine;

[CreateAssetMenu(fileName ="NewEnemyActionData", menuName ="Enemy/Enemy Action Data")]
public class EnemyActionData : ScriptableObject
{
    public string actionName;
    public EnemyActionType actionType;

    public int damage;
    public int hitCount;
    public int defense;
    public int ignoreDefense;

    public int chance;
}