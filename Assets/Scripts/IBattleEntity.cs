using UnityEngine;

public interface IBattleEntity
{
    void OnTurnStart();
    void OnTakeTurn();
    void OnTurnEnd();
    void TakeDamage(int damage, DamageType damageType);
    void Heal(int healAmount);
    void Summon(GameObject prefab, SummonData rawData, Transform spawnPoint);
    Stats stats { get; }
    bool isExpired{ get; }
}
