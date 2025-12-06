using System.Collections.Generic;
using UnityEngine;

public class ColliderHits : MonoBehaviour
{
    private List<IBattleEntity> hits = new List<IBattleEntity>();

    void OnTriggerStay2D(Collider2D collision)
    {
        IBattleEntity entity = collision.GetComponent<IBattleEntity>();
        if (entity != null && !hits.Contains(entity))
        {
            hits.Add(entity);
        }
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        IBattleEntity entity = collision.GetComponent<IBattleEntity>();
        if(entity != null && hits.Contains(entity))
        {
            hits.Remove(entity);
        }
    }

    public List<IBattleEntity> GetHits()
    {
        return hits;
    }

    public void ClearHits()
    {
        hits.Clear();
    }
}
