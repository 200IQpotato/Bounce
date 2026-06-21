using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "Effects/Poison")]
public class Poison : EffectObject, IOnTakeTurn
{
    public int damagePerTurn;

    public override void OnApply(IBattleEntity entity, Effect effect)
    {
        effect.duration = effect.stackCount;
    }

    public override void OnTurnEnd(IBattleEntity entity, Effect effect)
    {
        effect.stackCount = effect.duration;
    }

    public void OnTakeTurn(IBattleEntity entity, Effect effect)
    {
        entity.TakeDamage(damagePerTurn * effect.stackCount, DamageType.Dot);
        Debug.Log($"{entity} takes {damagePerTurn * effect.stackCount} poison damage.");
    }
}
