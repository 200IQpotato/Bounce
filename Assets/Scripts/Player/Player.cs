using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour, IBattleEntity, TurnBase
{
    private Rigidbody2D rb;
    [Header("Drag & Prediction Line")]
    [SerializeField] private LineRenderer DragLine;
    [SerializeField] private LineRenderer PredictionLine;
    [SerializeField] private GameObject DragPoint;
    [SerializeField] private List<GameObject> PredictionPoint;
    [SerializeField] private LayerMask collisionMask; // 設定可碰撞圖層（例如：牆、敵人等）
    [SerializeField] private float maxReflectionCount = 2; // 最多反彈次數

    [Header("Drag Settings")]
    [SerializeField] private float dragLimit = 3f;
    [SerializeField] private float stopThreshold = 0.1f;
    
    
    private Vector3 mousePosition
    {
        get
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            return mousePosition;
        }
    }

    private PlayerStat playerStat;
    private Action<Enemy> onHit;
    private Action onHealthChange;
    public bool isExpired { get; set; }
    private bool hasShoot = false;


    void Awake()
    {
        playerStat = GetComponent<PlayerStat>();
        rb = GetComponent<Rigidbody2D>();
        DragLine.positionCount = 2;
        isExpired = false;
    }

    void Start()
    {
        BattleManager.Instance.RegisterRigidbody(rb);
        BattleManager.Instance.RegisterPlayer(this);
        BattleManager.Instance.RegisterEntity(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnMouseDrag()
    {
        if ( BattleManager.Instance.CurrentState != GameState.PlayerTurn || hasShoot )
            return;

        DrawDragLine();
        DrawPredictionLine();
    }

    void OnMouseUp()
    {
        if (BattleManager.Instance.CurrentState != GameState.PlayerTurn || hasShoot)
            return;

        DisableLine();

        Vector3 distance = mousePosition - transform.position;
        if (distance.magnitude > dragLimit)
        {
            distance = distance.normalized * dragLimit;
        }
        rb.AddForce(-distance * playerStat.force, ForceMode2D.Impulse);
        hasShoot = true;
    }

    public IEnumerator TakeTurn()
    {
        Debug.Log($"{name} Turn Start");
        OnTurnStart();
        yield return new WaitUntil(() => hasShoot);
        yield return new WaitUntil(BattleManager.Instance.AllObjectsStopped);
        OnTurnEnd();
    }

    public void OnTurnStart()
    {
        hasShoot = false;
    }

    public void OnTurnEnd()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (BattleManager.Instance.CurrentState == GameState.PlayerTurn)
                enemy.TakeDamage(playerStat.attack);
                
            onHit?.Invoke(collision.gameObject.GetComponent<Enemy>());
        }
    }

    void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.UnregisterRigidbody(rb);
            BattleManager.Instance.UnregisterPlayer(this);
            BattleManager.Instance.UnregisterEntity(this);
        }
    }

    public void TakeDamage(int damage)
    {
        if (BattleManager.Instance.CurrentState != GameState.EnemyTurn)
            return;
        playerStat.health -= damage;
        if (damage != 0)
            onHealthChange?.Invoke();
            
        Debug.Log("Player Health: " + playerStat.health);
        if (playerStat.health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player Death");
        // Implement game over logic here
    }

    void DrawDragLine()
    {
        DragLine.enabled = true;
        DragLine.SetPosition(0, transform.position + (mousePosition - transform.position).normalized * 0.5f);

        if (Vector3.Distance(mousePosition, transform.position) <= dragLimit)
        {
            DragLine.SetPosition(1, mousePosition);
        }
        else
        {
            Vector3 direction = (mousePosition - transform.position).normalized;
            DragLine.SetPosition(1, transform.position + direction * dragLimit);
        }
        SetDragPoint(DragLine.GetPosition(1));
    }

    void DrawPredictionLine()
    {
        PredictionLine.enabled = true;
        Vector3 startPosition = transform.position;

        Vector3 distance = mousePosition - transform.position;
        if (distance.magnitude > dragLimit)
        {
            distance = distance.normalized * dragLimit;
        }

        Vector3 initialVelocity = -distance * playerStat.force / rb.mass;
        Vector3 currentVelocity = initialVelocity;
        float timeStep = 0.02f;
        float currentTime = 0f;
        float ballRadius = 0.5f;
        int reflectionCount = 0;
        bool isHittingEnemy = false;

        PredictionLine.positionCount = 1;
        PredictionLine.SetPosition(0, startPosition + currentVelocity.normalized * 0.5f );

        while (currentVelocity.magnitude > 0.1f && reflectionCount < maxReflectionCount)
        {
            float stepDistance = currentVelocity.magnitude * timeStep;
            Vector3 direction = currentVelocity.normalized;

            RaycastHit2D hit = Physics2D.CircleCast(startPosition, ballRadius, direction, stepDistance, collisionMask);
            if (hit.collider != null)
            {
                PredictionLine.positionCount++;
                PredictionLine.SetPosition(PredictionLine.positionCount - 1, hit.point + hit.normal * ballRadius);
                currentVelocity = Vector3.Reflect(currentVelocity, hit.normal);
                startPosition = hit.point + hit.normal * (ballRadius + 0.01f);
                reflectionCount++;
                SetPredictionPoint(startPosition, reflectionCount - 1);

                if (hit.collider.CompareTag("Enemy"))
                {
                    isHittingEnemy = true;
                    break;
                }
            }
            else
            {
                startPosition += direction * stepDistance;
                currentTime += timeStep;
                currentVelocity *= (1 - rb.linearDamping * timeStep);
            }
        }

        if (reflectionCount < maxReflectionCount && !isHittingEnemy)
        {
            PredictionLine.positionCount++;
            PredictionLine.SetPosition(PredictionLine.positionCount - 1, startPosition);
        }

        SetPredictionPoint(startPosition, PredictionLine.positionCount - 2);
        if(PredictionLine.positionCount < 3)
            PredictionPoint[1].SetActive(false);
    }

    void SetPredictionPoint(Vector3 position, int index)
    {
        PredictionPoint[index].SetActive(true);
        PredictionPoint[index].transform.position = position;
    }

    void SetDragPoint(Vector3 position)
    {
        DragPoint.SetActive(true);
        DragPoint.transform.position = position;
    }

    void DisableLine()
    {
        DragLine.enabled = false;
        PredictionLine.enabled = false;
        DragPoint.SetActive(false);
        PredictionPoint[0].SetActive(false);
        PredictionPoint[1].SetActive(false);
    }
}
