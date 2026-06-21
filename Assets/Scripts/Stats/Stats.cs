using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public int attack;
    public int attackPercent;
    public float force;
    public int money;
    public List<Effect> effects = new();

    public System.Action OnHealthChanged;
    public System.Action OnMaxHealthChanged;
    public System.Action OnAttackChanged;
    public System.Action OnMoneyChanged;

    public void ModifyMaxHealth(int value)
    {
        maxHealth += value;
        health = Mathf.Min(health, maxHealth);
        OnMaxHealthChanged?.Invoke();
        OnHealthChanged?.Invoke();
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        OnHealthChanged?.Invoke();
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        OnHealthChanged?.Invoke();
    }

    public void ModifyAttack(int value)
    {
        attack += value;
        OnAttackChanged?.Invoke();
    }

    public int GetAttack()
    {
        return (int)(attack * (1 + attackPercent / 100f));
    }

    public bool IsAffordable(int cost)
    {
        if (money >= cost)
            return true;
        return false;
    }

    public void ModifyMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke();
    }

    public void NotifyOnHit(IBattleEntity owner, IBattleEntity target)
    {
        foreach (var effect in effects)
        {
            if (effect.effectObject is IOnHitEffect e)
                e.OnHit(owner, target);
        }
    }

    public void NotifyOnDealDamage(IBattleEntity owner, IBattleEntity target, ref int damage, DamageType damageType)
    {
        foreach (var effect in effects)
        {
            if (effect.effectObject is IOnDealDamageEffect e)
                e.OnDealDamage(owner, target, ref damage, damageType);
        }
    }

    public void NotifyOnTakeDamage(IBattleEntity owner, ref int damage, DamageType damageType)
    {
        foreach (var effect in effects)
        {
            if (effect.effectObject is IOnTakeDamageEffect e)
                e.OnTakeDamage(owner, ref damage, damageType);
        }
    }

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
            effects.Sort((a, b) => a.effectObject.priority.CompareTo(b.effectObject.priority));
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

        foreach (var effect in effects)
            effect.isConsumed = false;

        RemoveExpiredEffects();
    }

    public void CleanEffect()
    {
        effects = new();
        Debug.Log("Effects clear");
    }
}
