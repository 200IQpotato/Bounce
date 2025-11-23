using UnityEngine;

public enum ValueType{ None, Round, Bounce, Hit }

public class RelicObject : ScriptableObject
{
    public string relicName;
    public string description;
    public Sprite icon;
    public ValueType valueType;
    public int priority;
    public virtual void OnEquip(Player player) { }
    public virtual void OnUnequip(Player player) { }
    public virtual void OnHit(Player player, IBattleEntity entity) { }
    public virtual void OnDealDamage(Player player, IBattleEntity entity, ref int damage) { }
    public virtual void OnTakeDamage(Player player, ref int damage) { }
    public virtual void onHealthChange(Player player ) { }
    public virtual void OnTurnStart(Player player) { }
    public virtual void OnTurnEnd(Player player) { }
    public virtual int GetUIValue(int rawValue){ return rawValue;}
    public int GetValue(RelicHolder relicHolder)
    {
        switch (valueType)
        {
            case ValueType.Round:
                return relicHolder.roundCount;
            case ValueType.Bounce:
                return relicHolder.bounceCount;
            case ValueType.Hit:
                return relicHolder.hitCount;
            default:
                return 0;
        }
    }

    
}
