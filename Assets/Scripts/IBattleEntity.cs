using UnityEngine;

public interface IBattleEntity
{
    void OnTurnStart();
    void OnTurnEnd();
    void TakeDamage(int damage);
    void Heal(int healAmount);
    Stats stats { get; }
    bool isExpired{ get; }
}
