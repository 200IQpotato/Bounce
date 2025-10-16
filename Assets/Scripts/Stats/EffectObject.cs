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
    public virtual void OnTurnStart(IBattleEntity entity) { }
    public virtual void OnTurnEnd(IBattleEntity entity) { }
    public virtual void OnApply(IBattleEntity entity) { }
    public virtual void OnExpire(IBattleEntity entity) { }
}
