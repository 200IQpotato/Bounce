using UnityEngine;

public class IceObstacle : Obstacle, ITerrainZone
{
    public float damping = 0.02f;
    public bool IsExclusive => true;
    public float PredictBreakTime => 0f;

    public void ModifyVelocity(Vector2 position, ref Vector2 velocity, float deltaTime, float baseDamping)
    {
        velocity /= (1 - baseDamping * deltaTime);
        velocity *= (1 - damping * deltaTime);
    }
}
