using UnityEngine;

public enum EffectType
{
    Buff,
    Debuff,
    Dot
}

public interface IOnHitEffect
{
    void OnHit(IBattleEntity owner, IBattleEntity target);
}

public interface IOnDealDamageEffect
{
    void OnDealDamage(IBattleEntity owner, IBattleEntity target, ref int damage);
}

public interface IOnTakeDamageEffect
{
    void OnTakeDamage(IBattleEntity owner, ref int damage);
}

public class EffectObject : ScriptableObject
{
    public string effectName;
    public EffectType effectType;
    public bool isStackable;
    public int priority;
    public virtual void OnTurnStart(IBattleEntity entity, Effect effect) { }
    public virtual void OnTurnEnd(IBattleEntity entity, Effect effect) { }
    public virtual void OnApply(IBattleEntity entity, Effect effect) { }
    public virtual void OnExpire(IBattleEntity entity, Effect effect) { }
}
