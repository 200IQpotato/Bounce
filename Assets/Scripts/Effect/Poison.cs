using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "Effects/Poison")]
public class Poison : EffectObject, IOnTakeTurn
{
    public void OnTakeTurn(IBattleEntity entity, Effect effect)
    {
        entity.TakeDamage(null, effect.stackCount, DamageType.Dot);
        Debug.Log($"{entity} takes {effect.stackCount} poison damage.");
    }
}
