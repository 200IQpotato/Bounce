using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IBattleEntity, ITurnBase
{
    private Rigidbody2D rb;
    public Stats enemyStat;
    [SerializeField] private EnemyUI enemyUI;
    [SerializeField] private float stopThreshold = 0.1f;
    public bool isExpired { get; set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyStat = GetComponent<Stats>();
        enemyUI.UpdateHealth(enemyStat.health);
        enemyUI.UpdateAttack(enemyStat.attack);
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
                if (BattleManager.Instance.CurrentState == GameState.EnemyTurn)
                    player.TakeDamage(enemyStat.attack);
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
        enemyStat.OnTurnEnd(this);
    }

    public void TakeDamage(int damage)
    {
        enemyStat.health -= damage;
        enemyUI.UpdateHealth(enemyStat.health);
        Debug.Log("Enemy Health: " + enemyStat.health);
        if (enemyStat.health <= 0)
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
