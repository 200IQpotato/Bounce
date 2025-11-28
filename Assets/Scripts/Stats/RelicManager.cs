using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }

    public List<RelicObject> allRelics = new();
    public List<RelicObject> playerRelics = new();

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

    public RelicObject GetRandomRelic()
    {
        var available = allRelics.Except(playerRelics).ToList();
        if (available.Count == 0)
            return null;
        return available[Random.Range(0, available.Count)];
    }
    
    public void AddRelicToPlayer(RelicObject relic)
    {
        if (!playerRelics.Contains(relic))
        {
            playerRelics.Add(relic);
        }
    }

    public void RemoveRelicFromPlayer(RelicObject relic)
    {
        if (playerRelics.Contains(relic))
        {
            playerRelics.Remove(relic);
        }
    }
}
