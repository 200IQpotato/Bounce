using UnityEngine;

public interface IBattleEntity
{
    void OnTurnStart();
    void OnTurnEnd();
    void TakeDamage(int damage);
    bool isExpired{ get; }
}
