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
        var existingEffect = effects.Find(e => e.effectObject == effect.effectObject);

        if (existingEffect != null)
        {
            if (effect.effectObject.isStackable)
            {
                existingEffect.stackCount++;
                Debug.Log($"Effect {effect.effectObject.name} stacked to {existingEffect.stackCount}.");
            }
            else
            {
                Debug.Log($"Effect {effect.effectObject.name} is not stackable. Refreshing duration.");
            }

            existingEffect.duration = Mathf.Max(existingEffect.duration, effect.duration);
            effect.effectObject.OnApply(this.GetComponent<IBattleEntity>(), existingEffect);
        }
        else
        {
            effects.Add(effect);
            effect.effectObject.OnApply(this.GetComponent<IBattleEntity>(), effect);
            Debug.Log($"Effect {effect.effectObject.name} applied with duration {effect.duration}.");
        }
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
            effect.effectObject.OnTurnEnd(entity, effect);
            Debug.Log($"Effect {effect.effectObject.name} duration decreased to {effect.duration}.");

            if (effect.duration <= 0)
            {
                effect.effectObject.OnExpire(entity, effect);
                Debug.Log($"Effect {effect.effectObject.name} has expired.");
            }
        }
        RemoveExpiredEffects();
    }
}
