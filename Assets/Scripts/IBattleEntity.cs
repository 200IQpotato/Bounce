using UnityEngine;

public interface IBattleEntity
{
    void OnTurnStart();
    void OnTurnEnd();
    void TakeDamage(int damage);
    Stats stats { get; }
    bool isExpired{ get; }
}
