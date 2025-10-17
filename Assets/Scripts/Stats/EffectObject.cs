using UnityEngine;

public enum EffectType
{
    Buff,
    Debuff
}

public class EffectObject : ScriptableObject
{
    public string effectName;
    public EffectType effectType;
    public bool isStackable;
    public virtual void OnTurnStart(IBattleEntity entity, Effect effect) { }
    public virtual void OnTurnEnd(IBattleEntity entity, Effect effect) { }
    public virtual void OnApply(IBattleEntity entity, Effect effect) { }
    public virtual void OnExpire(IBattleEntity entity, Effect effect) { }
}
