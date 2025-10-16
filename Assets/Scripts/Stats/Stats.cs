using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int health;
    public int attack;
    public float force;
    public List<Effect> effects = new();

    public void ApplyEffect(Effect effect)
    {
        effects.Add(effect);
        effect.effectObject.OnApply(this.GetComponent<IBattleEntity>());
    }

    public void RemoveExpiredEffects()
    {
        effects.RemoveAll(e => e.duration <= 0);
    }

    public void OnTurnEnd(IBattleEntity entity)
    {
        foreach (var effect in effects)
        {
            effect.duration--;
            effect.effectObject.OnTurnEnd(entity);
            Debug.Log($"Effect {effect.effectObject.name} duration decreased to {effect.duration}.");

            if (effect.duration <= 0)
            {
                effect.effectObject.OnExpire(entity);
                Debug.Log($"Effect {effect.effectObject.name} has expired.");
            }
        }
        RemoveExpiredEffects();
    }
}
