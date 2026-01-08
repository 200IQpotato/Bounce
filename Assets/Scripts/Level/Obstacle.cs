using UnityEngine;

public class Obstacle : MonoBehaviour, IBattleEntity
{
    
    public Stats stats { get; set; }
    public bool isExpired{ get; set; }
    public bool isDestructible;
    public bool isTouchable;

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTurnStart()
    {
        
    }

    public void OnTurnEnd()
    {
        
    }

    public void TakeDamage(int damage)
    {
        
    }

    public void Heal(int healAmount)
    {
        
    }
}
