using UnityEngine;

public class SlimeObstacle : Obstacle, ITerrainZone
{
    public float damping = 4f;
    public bool IsExclusive => true;
    public float PredictBreakTime => 0f;

    public void ModifyVelocity(Vector2 ballPosition, ref Vector2 velocity, float deltaTime, float baseDamping)
    {
        velocity /= (1 - baseDamping * deltaTime); // 先抵銷Unity自動套用的預設阻力
        velocity *= (1 - damping * deltaTime);      // 換成地形自己的阻力
    }
}
