using UnityEngine;

public static class TerrainResolver
{
    // 用 OverlapPoint 找出目前重疊中,互斥型 zone 裡 priority 最高的那個 Obstacle
    public static Obstacle ResolvePrimaryExclusive(Vector2 position, LayerMask mask)
    {
        var hits = Physics2D.OverlapPointAll(position, mask);
        Obstacle best = null;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<ITerrainZone>(out var zone) && zone.IsExclusive
                && hit.TryGetComponent<Obstacle>(out var obstacle))
            {
                if (best == null || obstacle.priority > best.priority)
                    best = obstacle;
            }
        }
        return best;
    }
}