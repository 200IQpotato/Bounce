using UnityEngine;

public class IceObstacle : Obstacle, ITerrainZone
{
    public float dampingMultiplier = 0.1f; // 遠小於預設 rb.linearDamping,幾乎不衰減
    public float PredictBreakTime => 0f;

    public void ModifyVelocity(Vector2 ballPosition, ref Vector2 velocity, float deltaTime)
    {
        velocity *= (1 - dampingMultiplier * deltaTime);
    }
}
