using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "Effects/Poison")]
public class Poison : EffectObject
{
    public int damagePerTurn;

    public override void OnTurnEnd(IBattleEntity entity, Effect effect)
    {
        entity.TakeDamage(damagePerTurn * effect.stackCount);
        Debug.Log($"{entity} takes {damagePerTurn * effect.stackCount} poison damage.");
    }
}
