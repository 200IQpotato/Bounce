using UnityEngine;

[CreateAssetMenu(fileName = "Strength", menuName = "Effects/Strength")]
public class Strength : EffectObject
{
    public override void OnApply(IBattleEntity entity, Effect effect)
    {
        entity.stats.attackPercent += 10 * effect.stackCount;
    }

    public override void OnExpire(IBattleEntity entity, Effect effect)
    {
        entity.stats.attackPercent -= 10 * effect.stackCount;
    }
}
