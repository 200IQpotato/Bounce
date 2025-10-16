using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "Effects/Poison")]
public class Poison : EffectObject
{
    public int damagePerTurn;

    public override void OnTurnEnd(IBattleEntity entity)
    {
        entity.TakeDamage(damagePerTurn);
        Debug.Log($"{entity} takes {damagePerTurn} poison damage.");
    }
}
