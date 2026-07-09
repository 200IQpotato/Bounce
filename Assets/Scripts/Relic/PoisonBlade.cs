using UnityEngine;

[CreateAssetMenu(fileName = "Poison Blade", menuName = "Relics/Poison Blade")]
public class PoisonBlade : RelicObject
{
    public EffectObject poisonEffect;
    public int poisonStack;
    public override void OnHit(Player player, IBattleEntity entity)
    {
        entity.stats.ApplyEffect(new Effect(poisonEffect, 1, poisonStack));
        OnTrigger();
    }
}
