using UnityEngine;

public interface IBattleEntity
{
    void OnTurnStart();
    void OnTakeTurn();
    void OnTurnEnd();
    void TakeDamage(IBattleEntity attacker, int damage, DamageType damageType);
    void Heal(int rawHealAmount);
    void Summon(GameObject prefab, SummonData rawData, Transform spawnPoint);
    Stats stats { get; }
    bool isExpired{ get; }
}
