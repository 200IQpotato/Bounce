using UnityEngine;

public enum ValueType{ None, Round, Bounce, Hit }

public class RelicObject : ScriptableObject
{
    public string relicID;
    public Sprite icon;
    public ValueType valueType;
    public int priority;
    public string relicName => LocalizationManager.Instance.GetRelicName(relicID);
    public string description => LocalizationManager.Instance.GetRelicDescription(relicID);
    public virtual void OnEquip(Player player) { }
    public virtual void OnUnequip(Player player) { }
    public virtual void OnHit(Player player, IBattleEntity entity) { }
    public virtual void OnDealDamage(Player player, IBattleEntity entity, ref int damage, DamageType damageType) { }
    public virtual void OnTakeDamage(Player player, ref int damage, DamageType damageType) { }
    public virtual void OnHealthChange(Player player ) { }
    public virtual void OnSummon(Player player, ref SummonData data) { }
    public virtual void OnTurnStart(Player player) { }
    public virtual void OnTakeTurn(Player player) { }
    public virtual void OnTurnEnd(Player player) { }
    public virtual int GetUIValue(int rawValue){ return rawValue;}
    protected void OnTrigger()
    {
        RelicManager.Instance.OnRelicTrigger(this);
    }
    
}
