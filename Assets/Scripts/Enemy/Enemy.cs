using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyStat enemyStat;
    [SerializeField] private EnemyUI enemyUI;
    [SerializeField] private float stopThreshold = 0.1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyStat = GetComponent<EnemyStat>();
        enemyUI.UpdateHealth(enemyStat.health);
        enemyUI.UpdateAttack(enemyStat.attack);
    }
    void Start()
    {
        GameManager.Instance.RegisterRigidbody(rb);
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
                player.TakeDamage(enemyStat.attack);
            }
        }
    }


    public void TakeDamage(int damage)
    {
        if (GameManager.Instance.CurrentState != GameState.PlayerRunning)
            return;
            
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
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterRigidbody(rb);
    }
}
