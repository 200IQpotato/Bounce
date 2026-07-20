using UnityEngine;

public interface ITerrainZone
{
    bool IsExclusive { get; } // true = 互斥(Ice/Slime),同時只有priority最高的生效; false = 疊加(BlackHole),全部生效
    void ModifyVelocity(Vector2 ballPosition, ref Vector2 velocity, float deltaTime, float baseDamping);
    float PredictBreakTime { get; } // 0 = 完整模擬到底,不中斷預測
}

public interface IBounceModifier
{
    float GetForceMultiplier();
}

public class Obstacle : MonoBehaviour, IBattleEntity
{
    
    public Stats stats { get; set; }
    public bool isExpired{ get; set; }
    public bool isDestructible;
    public bool isTouchable;
    public int priority; // 互斥型terrain重疊時,數字越大越優先生效
    public int lifespanTurns = -1; // -1 = 永久, >0 = 該回合數後自動消失
    [SerializeField] private LayerMask terrainMask;

    private Collider2D obstacleCollider;

    void Awake()
    {
        stats = GetComponent<Stats>();
        isExpired = false;
        obstacleCollider = GetComponent<Collider2D>();
        if (!isTouchable && obstacleCollider != null)
        {
            obstacleCollider.isTrigger = true;
        }    
    }

    void Start()
    {
        BattleManager.Instance.RegisterEntity(this);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isTouchable) return;
        if (this is ITerrainZone zone && other.TryGetComponent<Rigidbody2D>(out var rb))
        {
            if (zone.IsExclusive)
            {
                Obstacle primary = TerrainResolver.ResolvePrimaryExclusive(rb.position, terrainMask);
                if (primary != this) return;
            }

            Vector2 vel = rb.linearVelocity;
            zone.ModifyVelocity(rb.position, ref vel, Time.fixedDeltaTime, rb.linearDamping);
            rb.linearVelocity = vel;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isTouchable) return;
        if (this is IBounceModifier bounce && collision.rigidbody != null)
        {
            collision.rigidbody.linearVelocity *= bounce.GetForceMultiplier();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTurnStart()
    {
        
    }

    public void OnTakeTurn()
    {
        
    }

    public void OnTurnEnd()
    {
        if (lifespanTurns > 0)
        {
            lifespanTurns--;
            if (lifespanTurns == 0)
                ExpireSelf();
        }
    }

    private void ExpireSelf()
    {
        isExpired = true;
        BattleManager.Instance.UnregisterEntity(this);
        Destroy(gameObject);
    }

    public void TakeDamage(IBattleEntity attacker, int damage, DamageType damageType)
    {
        if(!isDestructible) return;
    }

    public void Heal(int healAmount)
    {
        
    }

    public void Summon(GameObject prefab, SummonData rawData, Transform spawnPoint)
    {
        
    }

    public void SetLifeTurns(int lifeTurns)
    {
        lifespanTurns = lifeTurns;
    }
}
