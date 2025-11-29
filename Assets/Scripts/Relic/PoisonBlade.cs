using UnityEngine;

[CreateAssetMenu(fileName = "Poison Blade", menuName = "Relics/Poison Blade")]
public class PoisonBlade : RelicObject
{
    public EffectObject poisonEffect;
    public override void OnHit(Player player, IBattleEntity entity)
    {
        entity.stats.ApplyEffect(new Effect(poisonEffect, 2));
    }
}
