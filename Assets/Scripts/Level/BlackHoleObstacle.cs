using UnityEngine;

public class BlackHoleObstacle : Obstacle, ITerrainZone
{
    public float pullForce = 8f;
    public float minDistance = 0.5f; // 避免反平方公式在中心附近數值爆炸
    public float predictBreakDuration = 0.4f; // 預測進場域後跑幾秒才切斷改箭頭
    public bool IsExclusive => false;
    public float PredictBreakTime => predictBreakDuration;

    public void ModifyVelocity(Vector2 ballPosition, ref Vector2 velocity, float deltaTime, float baseDamping)
    {
        Vector2 toCenter = (Vector2)transform.position - ballPosition;
        float distance = Mathf.Max(toCenter.magnitude, minDistance);
        velocity += toCenter.normalized * (pullForce / (distance * distance)) * deltaTime;
    }
}
