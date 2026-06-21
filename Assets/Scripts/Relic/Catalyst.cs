using UnityEngine;

[CreateAssetMenu(fileName = "Catalyst", menuName = "Relics/Catalyst")]
public class Catalyst : RelicObject
{
    public int HitCountToExplode = 7;

    public override void OnHit(Player player, IBattleEntity entity)
    {
        if ( RelicManager.Instance.hitCount % HitCountToExplode == 0 && RelicManager.Instance.hitCount != 0 )
        {
            foreach ( Effect effect in entity.stats.effects)
            {
                if ( effect.effectObject.effectType == EffectType.Dot && effect.effectObject is IOnTakeTurn e)
                {
                    e.OnTakeTurn(entity, effect);
                }
            }
            
            Debug.Log($"Catalyst triggered!");
        }
    }

    public override int GetUIValue(int rawValue)
    {
        return rawValue % 7;
    }
}
