using UnityEngine;

[CreateAssetMenu(fileName = "Poison Blade", menuName = "Relics/Poison Blade")]
public class PoisonBlade : RelicObject
{
    public EffectObject poisonEffect;
    public override void OnHit(Player player, IBattleEntity entity, ref int damage)
    {
        entity.stats.ApplyEffect(new Effect(poisonEffect, 1));
    }
}
