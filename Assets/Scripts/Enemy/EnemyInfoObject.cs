using UnityEngine;

[CreateAssetMenu(fileName = "EnemyInfoObject", menuName = "Scriptable Objects/EnemyInfoObject")]
public class EnemyInfoObject : ScriptableObject
{
    public Sprite enemySprite;
    public string enemyID;
}
