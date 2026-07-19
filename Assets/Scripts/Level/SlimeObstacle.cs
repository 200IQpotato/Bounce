using UnityEngine;

public class SlimeObstacle : Obstacle, ITerrainZone
{
    public float extraDampingMultiplier = 3f;
    public float PredictBreakTime => 0f;

    public void ModifyVelocity(Vector2 ballPosition, ref Vector2 velocity, float deltaTime)
    {
        velocity *= (1 - extraDampingMultiplier * deltaTime);
    }
}
