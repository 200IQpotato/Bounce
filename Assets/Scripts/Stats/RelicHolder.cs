using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelicHolder : MonoBehaviour
{
    public List<RelicObject> relics = new();

    public void EquipRelic(RelicObject relic)
    {
        if (!relics.Contains(relic))
        {
            relics.Add(relic);
            relic.OnEquip(GetComponent<Player>());
            Debug.Log($"Equipped relic: {relic.relicName}");
            RelicManager.Instance.AddRelicToPlayer(relic);
        }
    }

    public void UnequipRelic(RelicObject relic)
    {
        if (relics.Contains(relic))
        {
            relics.Remove(relic);
            relic.OnUnequip(GetComponent<Player>());
        }
    }

    public void OnHit(Player player, IBattleEntity entity)
    {
        foreach (var relic in relics.OrderBy(r => r.priority))
        {
            relic.OnHit(player, entity);
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
