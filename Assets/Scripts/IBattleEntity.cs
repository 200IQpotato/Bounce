using UnityEngine;

public interface IBattleEntity
{
    void OnTurnStart();
    void OnTurnEnd();
    void TakeDamage(int damage, DamageType damageType);
    void Heal(int healAmount);
    Stats stats { get; }
    bool isExpired{ get; }
}
