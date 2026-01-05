using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleLevelSO", menuName = "Scriptable Objects/BattleLevelSO")]
public class BattleLevelSO : ScriptableObject
{
    public List<Enemy> enemies = new List<Enemy>();
    public List<Vector2> spawnPoints = new List<Vector2>();
    public int minMoneyReward;
    public int maxMoneyReward;
    public int relicCount;
    public List<RelicObject> relicRewards = new List<RelicObject>();
}
