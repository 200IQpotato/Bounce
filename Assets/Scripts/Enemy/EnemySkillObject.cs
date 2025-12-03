using UnityEngine;

public enum PreviewType
{
    None,
    Arrow,    // 衝刺方向
    Circle,   // 範圍 AoE
    Sector,   // 扇形
    Area,     // 格子 AoE
    Donut,    // 環形
    Single    // 單格 / 單體
}

public class SkillPreviewData
{
    public PreviewType type;
    public float radius;          // Circle
    public float angle;           // Sector
    public Vector2 direction;     // Arrow
    public Vector2 position;      // Single / AoE 中心
}

public abstract class EnemySkillObject : ScriptableObject
{
    public string skillName;
    public Sprite intentIcon;
    public PreviewType previewType;

    // 敵人在計畫下一步時叫
    public virtual SkillPreviewData GetPreviewData(Enemy enemy){ return null; }

    // 敵人回合真正執行
    public virtual void Execute(Enemy enemy, MeshCollider meshCollider) {}
}