using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RelicHolder : MonoBehaviour
{
    public void EquipRelic(RelicObject relic)
    {
        relic.OnEquip(GetComponent<Player>());
        Debug.Log($"Equipped relic: {relic.relicName}");
        
    }

    public void UnequipRelic(RelicObject relic)
    {
        relic.OnUnequip(GetComponent<Player>());
        Debug.Log($"Unequipped relic: {relic.relicName}");
        
    }

    public void OnHit(Player player, IBattleEntity entity)
    {
        foreach (var relic in RelicManager.Instance.playerRelics.OrderBy(r => r.priority))
        {
            relic.OnHit(player, entity);
        }
    }

    public void OnDealDamage(Player player, IBattleEntity entity, ref int damage)
    {
        foreach (var relic in RelicManager.Instance.playerRelics.OrderBy(r => r.priority))
        {
            relic.OnDealDamage(player, entity, ref damage);
        }
    }

    public void OnTakeDamage(Player player, ref int damage)
    {
        foreach (var relic in RelicManager.Instance.playerRelics.OrderBy(r => r.priority))
        {
            relic.OnTakeDamage(player, ref damage);
        }
    }

    public void OnHealthChange(Player player)
    {
        foreach (var relic in RelicManager.Instance.playerRelics.OrderBy(r => r.priority))
        {
            relic.onHealthChange(player);
        }
    }

    public void OnTurnStart(Player player)
    {
        foreach (var relic in RelicManager.Instance.playerRelics.OrderBy(r => r.priority))
        {
            relic.OnTurnStart(player);
        }
    }

    public void OnTurnEnd(Player player)
    {
        foreach (var relic in RelicManager.Instance.playerRelics.OrderBy(r => r.priority))
        {
            relic.OnTurnEnd(player);
        }
    }
}
