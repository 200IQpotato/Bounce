using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }

    public List<RelicObject> allRelics = new();
    public List<RelicObject> playerRelics = new();

    public static event Action<RelicObject, int> OnRelicAddedUI;
    public static event Action<RelicObject> OnRelicRemovedUI;
    public static event Action<ValueType, int> OnRelicValueUpdatedUI;
    public int bounceCount = 0;
    public int roundCount = 0;
    public int hitCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<RelicObject> GetRandomRelic(int count)
    {
        var available = allRelics.Except(playerRelics).ToList();
        if (available.Count == 0)
            return null;

        List<RelicObject> randomRelics = new List<RelicObject>();
        for (int i = 0; i < count && available.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, available.Count);
            randomRelics.Add(available[index]);
            available.RemoveAt(index);
        }
        return randomRelics;
    }
    
    public void AddRelicToPlayer(RelicObject relic)
    {
        if (!playerRelics.Contains(relic))
        {
            playerRelics.Add(relic);
            GameManager.Instance.playerInstance.relicHolder.EquipRelic(relic);
            OnRelicAddedUI?.Invoke(relic, GetTypeValue(relic.valueType));
        }
    }

    public void RemoveRelicFromPlayer(RelicObject relic)
    {
        if (playerRelics.Contains(relic))
        {
            playerRelics.Remove(relic);
            GameManager.Instance.playerInstance.relicHolder.UnequipRelic(relic);
            OnRelicRemovedUI?.Invoke(relic);
        }
    }

    public void OnCountInit()
    {
        bounceCount = 0;
        roundCount = 0;
        hitCount = 0;
        OnRelicValueUpdatedUI?.Invoke(ValueType.Bounce, bounceCount);
        OnRelicValueUpdatedUI?.Invoke(ValueType.Round, roundCount);
        OnRelicValueUpdatedUI?.Invoke(ValueType.Hit, hitCount);
    }

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

    public int GetTypeValue( ValueType valueType )
    {
        switch (valueType)
        {
            case ValueType.Round:
                return roundCount;
            case ValueType.Bounce:
                return bounceCount;
            case ValueType.Hit:
                return hitCount;
            default:
                return 0;
        }
    }
}
