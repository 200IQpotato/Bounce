using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RelicHolder : MonoBehaviour
{
    public List<RelicObject> relics = new();

    // relic related counts & UI events
    public static event Action<RelicObject, int> OnRelicAddedUI;
    public static event Action<RelicObject> OnRelicRemovedUI;
    public static event Action<ValueType, int> OnRelicValueUpdatedUI;
    public int bounceCount = 0;
    public int roundCount = 0;
    public int hitCount = 0;

    public void OnBounceAdd()
    {
        bounceCount++;
        OnRelicValueUpdatedUI?.Invoke(ValueType.Bounce, bounceCount);
    }

    public void OnRoundAdd()
    {
        roundCount++;
        OnRelicValueUpdatedUI?.Invoke(ValueType.Round, roundCount);
    }

    public void OnHitAdd()
    {
        hitCount++;
        OnRelicValueUpdatedUI?.Invoke(ValueType.Hit, hitCount);
    }

    public void EquipRelic(RelicObject relic)
    {
        if (!relics.Contains(relic))
        {
            relics.Add(relic);
            relic.OnEquip(GetComponent<Player>());
            Debug.Log($"Equipped relic: {relic.relicName}");
            RelicManager.Instance.AddRelicToPlayer(relic);
            OnRelicAddedUI?.Invoke(relic, relic.GetValue(this));
        }
    }

    public void UnequipRelic(RelicObject relic)
    {
        if (relics.Contains(relic))
        {
            relics.Remove(relic);
            relic.OnUnequip(GetComponent<Player>());
            OnRelicRemovedUI?.Invoke(relic);
        }
    }

    public void OnHit(Player player, IBattleEntity entity)
    {
        foreach (var relic in relics.OrderBy(r => r.priority))
        {
            relic.OnHit(player, entity);
        }
    }

    public void OnDealDamage(Player player, IBattleEntity entity, ref int damage)
    {
        foreach (var relic in relics.OrderBy(r => r.priority))
        {
            relic.OnDealDamage(player, entity, ref damage);
        }
    }

    public void OnTakeDamage(Player player, ref int damage)
    {
        foreach (var relic in relics.OrderBy(r => r.priority))
        {
            relic.OnTakeDamage(player, ref damage);
        }
    }

    public void OnHealthChange(Player player)
    {
        foreach (var relic in relics.OrderBy(r => r.priority))
        {
            relic.onHealthChange(player);
        }
    }

    public void OnTurnStart(Player player)
    {
        foreach (var relic in relics.OrderBy(r => r.priority))
        {
            relic.OnTurnStart(player);
        }
    }

    public void OnTurnEnd(Player player)
    {
        foreach (var relic in relics.OrderBy(r => r.priority))
        {
            relic.OnTurnEnd(player);
        }
    }
}
