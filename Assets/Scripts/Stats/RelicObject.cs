using UnityEngine;

public class RelicObject : ScriptableObject
{
    public string relicName;
    public string description;
    public int priority;
    public virtual void OnEquip(Player player) { }
    public virtual void OnUnequip(Player player) { }
    public virtual void OnHit(Player player, IBattleEntity entity, ref int damage) { }
    public virtual void OnTakeDamage(Player player, ref int damage) { }
    public virtual void onHealthChange(Player player ) { }
    public virtual void OnTurnStart(Player player) { }
    public virtual void OnTurnEnd(Player player) { }
}
