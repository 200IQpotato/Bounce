using System.Collections.Generic;
using Unity.VisualScripting;
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
    public System.Action OnEffectChange;

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

    public void ModifyAttackPercent(int value)
    {
        attackPercent += value;
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
                e.OnHit(owner, effect, target);
        }
    }

    public void NotifyOnDealDamage(IBattleEntity owner, IBattleEntity target, ref int damage, DamageType damageType)
    {
        foreach (var effect in effects)
        {
            if (effect.effectObject is IOnDealDamageEffect e)
                e.OnDealDamage(owner, effect, target, ref damage, damageType);
        }
    }

    public void NotifyOnTakeDamage(IBattleEntity owner, IBattleEntity attacker, ref int damage, DamageType damageType)
    {
        foreach (var effect in effects)
        {
            if (effect.effectObject is IOnTakeDamageEffect e)
                e.OnTakeDamage(owner, effect, attacker, ref damage, damageType);
        }
    }

    public void NotifyOnHeal(IBattleEntity owner, ref int healAmount)
    {
        foreach (var effect in effects)
        {
            if (effect.effectObject is IOnHeal e)
                e.OnHeal(owner, effect, ref healAmount);
        }
    }

    public void NotifyOnSummon(IBattleEntity owner, ref SummonData data)
    {
        foreach (var effect in effects)
        {
            if (effect.effectObject is IOnSummon e)
                e.OnSummon(owner, effect, ref data);
        }
    }

    public void NotifyOnTakeTurn(IBattleEntity owner)
    {
        foreach (var effect in effects)
        {
            if (effect.effectObject is IOnTakeTurn e)
                e.OnTakeTurn(owner, effect);
        }
    }

    public void ApplyEffect(Effect effect)
    {
        bool isAdded = false;
        switch ( effect.effectObject.stackState )
        {
            case StackState.Only:
            {
                var existingEffect = effects.Find(e => e.effectObject == effect.effectObject);
                if(existingEffect == null)
                    break;
                existingEffect.duration = Mathf.Max(existingEffect.duration, effect.duration);
                effect.isConsumed = false;
                isAdded = true;
                Debug.Log($"Effect {effect.effectObject.name} is Only. Refreshing duration.");
                break;
            }
                

            case StackState.Separate:
            {
                var existingEffects = effects.FindAll(e => e.effectObject == effect.effectObject);
                if(existingEffects == null)
                    break;
                foreach ( Effect existingEffect in existingEffects )
                {
                    if( existingEffect.duration == effect.duration )
                    {
                        existingEffect.stackCount += effect.stackCount;
                        effect.isConsumed = false;
                        isAdded = true;
                        Debug.Log($"Effect {effect.effectObject.name} is stacked. \nNow stacks : {existingEffect.stackCount}, Now duration : {existingEffect.duration}.");
                    }
                }
                break;
            }

            case StackState.Merge:
            {
                var existingEffect = effects.Find(e => e.effectObject == effect.effectObject);
                if(existingEffect == null)
                {
                    effect.duration = effect.stackCount;
                    isAdded = true;
                    effects.Add(effect);
                    Debug.Log($"{effect.effectObject.name} is added! \nNow stack : {effect.stackCount}, Now duration : {effect.duration}");
                    break;
                }
                    
                existingEffect.stackCount += effect.stackCount;
                existingEffect.duration = existingEffect.stackCount;
                effect.isConsumed = false;
                isAdded = true;
                Debug.Log($"Effect {effect.effectObject.name} is merged. \nNow stack : {existingEffect.stackCount}, Now duration : {existingEffect.duration}.");
                break;
            }                
        }

        if( !isAdded)
        {
            effects.Add(effect);
            Debug.Log($"{effect.effectObject.name} is added! \nNow stack : {effect.stackCount}, Now duration : {effect.duration}");
        }            

        effects.Sort((a, b) => a.effectObject.priority.CompareTo(b.effectObject.priority));
        effect.effectObject.OnApply(this.GetComponent<IBattleEntity>(), effect);
        OnEffectChange?.Invoke();
    }

    public int GetTotalStackCount(EffectObject effectObject)
    {
        int totalStack = 0;
        var targetEffectList = effects.FindAll(e => e.effectObject == effectObject);
        foreach (Effect effect in targetEffectList)
        {
            totalStack += effect.stackCount;
        }
        return totalStack;
    }

    public void RemoveExpiredEffects()
    {
        effects.RemoveAll(e => e.duration <= 0);
        OnEffectChange?.Invoke();
    }

    public void RemoveEffect(Effect effect)
    {
        if (effects.Remove(effect))
        {
            effect.effectObject.OnExpire(this.GetComponent<IBattleEntity>(), effect);
            OnEffectChange?.Invoke();
            Debug.Log($"Effect {effect.effectObject.name} removed manually.");
        }
    }

    public void ModifyEffectStackCount(Effect effect, int stackOffset)
    {
        effect.stackCount -= stackOffset;
        if(effect.stackCount <= 0)
            RemoveEffect(effect);
            
        OnEffectChange?.Invoke();
    }

    public void OnTurnStart(IBattleEntity entity)
    {
        foreach (var effect in effects)
        {
            effect.effectObject.OnTurnStart(entity, effect);
        }
    }

    public void OnTurnEnd(IBattleEntity entity)
    {
        foreach (var effect in effects)
        {
            effect.effectObject.OnTurnEnd(entity, effect);
            effect.duration--;
            if( effect.effectObject.stackState == StackState.Merge )
                effect.stackCount--;
            
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
        OnEffectChange?.Invoke();
    }

    public void CleanEffect()
    {
        effects = new();
        Debug.Log("Effects clear");
    }
}
