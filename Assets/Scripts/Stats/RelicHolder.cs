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
        foreach (var relic in RelicManager.Instance.playerRelics)
        {
            relic.OnHit(player, entity);
        }
    }

    public void OnDealDamage(Player player, IBattleEntity entity, ref int damage, DamageType damageType)
    {
        foreach (var relic in RelicManager.Instance.playerRelics)
        {
            relic.OnDealDamage(player, entity, ref damage, damageType);
        }
    }

    public void OnTakeDamage(Player player, ref int damage, DamageType damageType)
    {
        foreach (var relic in RelicManager.Instance.playerRelics)
        {
            relic.OnTakeDamage(player, ref damage, damageType);
        }
    }

    public void OnHealthChange(Player player)
    {
        foreach (var relic in RelicManager.Instance.playerRelics)
        {
            relic.onHealthChange(player);
        }
    }

    public void OnSummon(Player player, ref SummonData data)
    {
        foreach (var relic in RelicManager.Instance.playerRelics)
        {
            relic.OnSummon(player, ref data);
        }
    }

    public void OnTurnStart(Player player)
    {
        foreach (var relic in RelicManager.Instance.playerRelics)
        {
            relic.OnTurnStart(player);
        }
    }

    public void OnTakeTurn(Player player)
    {
        foreach (var relic in RelicManager.Instance.playerRelics)
        {
            relic.OnTakeTurn(player);
        }
    }

    public void OnTurnEnd(Player player)
    {
        foreach (var relic in RelicManager.Instance.playerRelics)
        {
            relic.OnTurnEnd(player);
        }
    }
}
