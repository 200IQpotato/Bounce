using UnityEngine;

public enum EffectType
{
    Buff,
    Debuff,
    Dot
}

public enum StackState
{
    Only,
    Separate,
    Merge
}

public interface IOnHitEffect
{
    void OnHit(IBattleEntity owner, Effect effect, IBattleEntity target);
}

public interface IOnDealDamageEffect
{
    void OnDealDamage(IBattleEntity owner, Effect effect, IBattleEntity target, ref int damage, DamageType damageType);
}

public interface IOnTakeDamageEffect
{
    void OnTakeDamage(IBattleEntity owner, Effect effect, ref int damage, DamageType damageType);
}

public interface IOnTakeTurn
{
    void OnTakeTurn( IBattleEntity owner, Effect effect );
}

public interface IOnSummon
{
    void OnSummon(IBattleEntity owner, Effect effect, ref SummonData data);
}

public class EffectObject : ScriptableObject
{
    public string effectName;
    public EffectType effectType;
    public StackState stackState;
    public int priority;
    public virtual void OnTurnStart(IBattleEntity entity, Effect effect) { }
    public virtual void OnTurnEnd(IBattleEntity entity, Effect effect) { }
    public virtual void OnApply(IBattleEntity entity, Effect effect) { }
    public virtual void OnExpire(IBattleEntity entity, Effect effect) { }
}
