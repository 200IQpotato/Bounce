using UnityEngine;

public class RubberObstacle : Obstacle, IBounceModifier
{
    public float forceMultiplier = 1.5f;
    public float GetForceMultiplier() => forceMultiplier;
}
