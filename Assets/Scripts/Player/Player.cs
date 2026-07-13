using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IBattleEntity, ITurnBase
{
    public Rigidbody2D rb;
    [Header("Drag & Prediction Line")]
    [SerializeField] private LineRenderer DragLine;
    [SerializeField] private LineRenderer PredictionLine;
    [SerializeField] private GameObject DragPoint;
    [SerializeField] private List<GameObject> PredictionPoint;
    [SerializeField] private LayerMask collisionMask; // 設定可碰撞圖層（例如：牆、敵人等）
    [SerializeField] private float maxReflectionCount = 2; // 最多反彈次數
    [SerializeField] private float ballRadius = 0.55f;

    [Header("Drag Settings")]
    [SerializeField] private float dragLimit = 3f;
    [SerializeField] private float stopThreshold = 0.1f;

    [Header("Drag State")]
    public bool isDragging { get; private set; }
    public event System.Action<bool> OnDragStateChanged;
    private bool dragCancelled = false; // 取消後鎖住,直到滑鼠真的放開

    [Header("Cancel Drag")]
    [SerializeField] private KeyCode cancelDragKey = KeyCode.Mouse1; // 右鍵取消

    [Header("Camera Follow")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector2 cameraOffsetLimit = new Vector2(2f, 2f);
    [SerializeField] private float cameraFollowSpeed = 8f;
    [SerializeField] private float cameraReturnSpeed = 5f;
    [SerializeField] private float cameraDeadzone = 0.5f; // 力度小於這個值,鏡頭不動
    private Vector3 cameraOriginPosition;
    
    
    private Vector3 mousePosition
    {
        get
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            return mousePosition;
        }
    }

    public Stats stats { get; set; }
    public RelicHolder relicHolder;
    public bool isExpired { get; set; }
    private bool hasShoot = false;
    

    void Awake()
    {
        stats = GetComponent<Stats>();
        relicHolder = GetComponent<RelicHolder>();
        rb = GetComponent<Rigidbody2D>();
        DragLine.positionCount = 2;
        isExpired = false;
        DisableLine();
    }

    void Start()
    {
        BattleManager.Instance.RegisterRigidbody(rb);
        BattleManager.Instance.RegisterPlayer(this);
        BattleManager.Instance.RegisterEntity(this);
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (cameraTransform != null)
        {
            cameraOriginPosition = cameraTransform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (isDragging)
        {
            if (Input.GetKeyDown(cancelDragKey))
            {
                CancelDrag();
                return;
            }
            UpdateCameraFollow();
        }

        else
        {
            ReturnCameraToOrigin();
        }
    }

    void OnMouseDrag()
    {
        if (GameManager.Instance.isUIBlockingInput)
            return;

        if ( GameManager.Instance.CurrentState != GameState.PlayerTurn || hasShoot )
            return;

        if (dragCancelled) // 被取消過,滑鼠還沒放開之前完全不處理
            return;

        if (!isDragging)
            StartDragging();
        DrawDragLine();
        DrawPredictionLine();
    }

    void OnMouseUp()
    {
        if (GameManager.Instance.isUIBlockingInput)
            return;
            
        if (GameManager.Instance.CurrentState != GameState.PlayerTurn || hasShoot)
            return;

        dragCancelled = false; // 滑鼠放開,取消鎖住

        if (!isDragging)
            return;

        DisableLine();
        EndDragging();

        Vector3 distance = mousePosition - transform.position;
        if (distance.magnitude > dragLimit)
        {
            distance = distance.normalized * dragLimit;
        }
        rb.AddForce(-distance * stats.force, ForceMode2D.Impulse);
        hasShoot = true;
    }

    public void CancelDrag()
    {
        DisableLine();
        EndDragging();
        dragCancelled = true; // 鎖住,直到滑鼠放開
        // 注意:不設 hasShoot,玩家可以重新拖動
    }

    private void StartDragging()
    {
        isDragging = true;
        OnDragStateChanged?.Invoke(true);
    }

    private void EndDragging()
    {
        isDragging = false;
        OnDragStateChanged?.Invoke(false);
    }

    private void UpdateCameraFollow()
    {
        if (cameraTransform == null) return;

        Vector3 distance = mousePosition - transform.position;
        if (distance.magnitude < cameraDeadzone)
        {
            ReturnCameraToOrigin();
            return; // 力度不夠,鏡頭維持原地
        }
            

        Vector2 desiredOffset = (Vector2)distance;
        desiredOffset = Vector2.ClampMagnitude(desiredOffset, 1f); // 先正規化力度
        desiredOffset = new Vector2(
            Mathf.Clamp(desiredOffset.x * cameraOffsetLimit.x, -cameraOffsetLimit.x, cameraOffsetLimit.x),
            Mathf.Clamp(desiredOffset.y * cameraOffsetLimit.y, -cameraOffsetLimit.y, cameraOffsetLimit.y)
        );

        Vector3 targetPos = cameraOriginPosition + (Vector3)desiredOffset;
        targetPos.z = cameraTransform.position.z;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, cameraFollowSpeed * Time.deltaTime);
    }

    private void ReturnCameraToOrigin()
    {
        if (cameraTransform == null) return;
        if (cameraTransform.position == cameraOriginPosition) return;

        Vector3 target = cameraOriginPosition;
        target.z = cameraTransform.position.z;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, target, cameraReturnSpeed * Time.deltaTime);
    }

    public IEnumerator TakeTurn()
    {
        hasShoot = false;
        Debug.Log($"{name} Turn Start");
        yield return new WaitUntil(() => hasShoot);
        yield return new WaitUntil(BattleManager.Instance.AllObjectsStopped);
    }

    public void OnTurnStart()
    {
        RelicManager.Instance.OnRoundAdd();
        relicHolder.OnTurnStart(this);
        stats.OnTurnStart(this);
    }

    public void OnTakeTurn()
    {
        relicHolder.OnTakeTurn(this);
        stats.NotifyOnTakeTurn(this);
    }

    public void OnTurnEnd()
    {
        relicHolder.OnTurnEnd(this);
        stats.OnTurnEnd(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (GameManager.Instance.CurrentState == GameState.PlayerTurn)
            {
                int damage = stats.GetAttack();
                DealDamage(enemy, damage, DamageType.Hit);
                RelicManager.Instance.OnHitAdd();
                relicHolder.OnHit(this, enemy);   
                stats.NotifyOnHit(this, enemy); 
            }
        }

        RelicManager.Instance.OnBounceAdd();
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

    public void TakeDamage(IBattleEntity attacker, int rawDamage, DamageType damageType)
    {
        int damage = rawDamage;
        if (damage == 0)
            return;

        relicHolder.OnTakeDamage(this, attacker, ref damage, damageType);  
        stats.NotifyOnTakeDamage(this, attacker, ref damage, damageType);                
        stats.TakeDamage(damage);
            
        Debug.Log("Player Health: " + stats.health + "\ntake damage: " + damage + "\tdamage type: " + damageType);
        if (stats.health <= 0)
        {
            Die();
        }
    }

    public void DealDamage(IBattleEntity target, int rawDamage, DamageType damageType)
    {
        int damage = rawDamage;
        if(damage == 0)
            return;

        relicHolder.OnDealDamage(this, target, ref damage, damageType);
        stats.NotifyOnDealDamage(this, target, ref damage, damageType);
        target.TakeDamage(this, damage, damageType);
    }

    public void Heal(int rawHealAmount)
    {
        int healAmount = rawHealAmount;
        relicHolder.OnHeal(this, ref healAmount);
        stats.NotifyOnHeal(this, ref healAmount);
        stats.Heal( healAmount );
        Debug.Log("Player Healed: " + healAmount + ", Current Health: " + stats.health);
    }

    public void Die()
    {
        Debug.Log("Player Death");
        isExpired = true;
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.UnregisterRigidbody(rb);
            BattleManager.Instance.UnregisterPlayer(this);
            BattleManager.Instance.UnregisterEntity(this);
        }
        UIManager.Instance.ShowRetryButton();
        Destroy(gameObject);
    }

    public void Summon(GameObject prefab, SummonData rawData, Transform spawnPoint)
    {
        SummonData data = rawData;
        relicHolder.OnSummon(this, ref data);
        stats.NotifyOnSummon(this, ref data);

        for (int i = 0; i < data.summonCount; i++)
        {
            var go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            var summon = go.GetComponent<Summonable>();
            if (summon != null)
            {
                summon.Init(data);
            }
        }
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

        Vector3 initialVelocity = -distance * stats.force / rb.mass;
        Vector3 currentVelocity = initialVelocity;
        float timeStep = 0.02f;
        float currentTime = 0f;
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
