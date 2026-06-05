using UnityEditor.Build.Reporting;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHp;
    public int attackDamage;

    public bool isBoss;
    public int specialAttackDamage;
    public int specialAttackTurn;

    public List<EnemyActionData> actions;
}
