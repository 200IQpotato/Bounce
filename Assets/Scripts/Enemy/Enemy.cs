using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IBattleEntity, ITurnBase
{
    private Rigidbody2D rb;
    public Stats stats { get; set; }
    [SerializeField] private EnemyUI enemyUI;
    [SerializeField] private float stopThreshold = 0.1f;
    public bool isExpired { get; set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Stats>();
        enemyUI.UpdateHealth(stats.health);
        enemyUI.UpdateAttack(stats.attack);
        isExpired = false;
    }
    void Start()
    {
        BattleManager.Instance.RegisterRigidbody(rb);
        BattleManager.Instance.RegisterEnemy(this);
        BattleManager.Instance.RegisterEntity(this);
    }

    void Update()
    {
        if (rb != null && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                if (GameManager.Instance.CurrentState == GameState.EnemyTurn)
                    player.TakeDamage(stats.attack);
            }
        }
    }

    public IEnumerator TakeTurn()
    {
        Debug.Log($"{name} Turn Start");
        Act();
        yield return new WaitUntil(BattleManager.Instance.AllObjectsStopped);
    }

    public void OnTurnStart()
    {
        
    }
    public void OnTurnEnd()
    {
        stats.OnTurnEnd(this);
    }

    public void TakeDamage(int damage)
    {
        stats.health -= damage;
        enemyUI.UpdateHealth(stats.health);
        Debug.Log("Enemy Health: " + stats.health);
        if (stats.health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Enemy Death");
        Destroy(gameObject);
    }

    public void Act()
    {
        Debug.Log($"{name} is acting!");
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.AddForce(randomDir * 50f, ForceMode2D.Impulse);
    }

    void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.UnregisterRigidbody(rb);
            BattleManager.Instance.UnregisterEnemy(this);
            BattleManager.Instance.UnregisterEntity(this);
        }
            
    }
}
