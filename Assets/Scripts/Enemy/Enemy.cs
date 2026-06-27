using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IBattleEntity, ITurnBase
{
    public Rigidbody2D rb;
    public Stats stats { get; set; }
    [SerializeField] private EnemyUI enemyUI;
    [SerializeField] private float stopThreshold = 0.1f;
    public bool isExpired { get; set; }

    [Header("Animator")]
    public Animator animator;

    [Header("Enemy Skills")]
    [SerializeField] private List<GameObject> skillPreviews = new List<GameObject>();
    private List<EnemySkill> skills = new List<EnemySkill>();
    private int currentSkillIndex = 0;
    private EnemySkill currentSkill;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<Stats>();
        enemyUI.SetCaster(this);
        
        isExpired = false;

        foreach (var preview in skillPreviews)
        {
            if (preview != null)
                preview.SetActive(false);
        }
        
        InitializeSkills();
    }

    protected void Start()
    {        
        BattleManager.Instance.RegisterRigidbody(rb);
        BattleManager.Instance.RegisterEnemy(this);
        BattleManager.Instance.RegisterEntity(this);
    }

    protected void Update()
    {
        if (rb != null && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (currentSkill != null)
        {
            currentSkill.UpdatePreview();
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                if (GameManager.Instance.CurrentState == GameState.EnemyTurn)
                    DealDamage(player, stats.GetAttack(), DamageType.Hit);
            }
        }
    }

    protected virtual void InitializeSkills() { }

    protected void AddSkill(EnemySkill skill)
    {
        skills.Add(skill);
    }

    public IEnumerator TakeTurn()
    {
        Debug.Log($"{name} Turn Start");
        if (currentSkill != null)
        {            
            yield return StartCoroutine(currentSkill.Execute());
            currentSkill.HidePreview();
            currentSkill = null;
        }
        yield return new WaitUntil(BattleManager.Instance.AllObjectsStopped);
    }

    public virtual void OnTurnStart()
    {
        stats.OnTurnStart(this);
        currentSkill = SelectSkill();
        
        if (currentSkill != null)
        {
            currentSkill.GetSkillData();
            currentSkill.ShowPreview();
        }
    }

    public virtual void OnTakeTurn()
    {
        stats.NotifyOnTakeTurn(this);
    }

    public virtual void OnTurnEnd()
    {
        stats.OnTurnEnd(this);
    }

    public virtual void TakeDamage(int rawDamage, DamageType damageType)
    {
        int damage = rawDamage;
        if(damage == 0)
            return;

        stats.NotifyOnTakeDamage(this, ref damage, damageType);
        stats.TakeDamage(damage);
        Debug.Log("Enemy Health: " + stats.health + "\ntake damage: " + damage + "\tdamage type: " + damageType);
        if (stats.health <= 0)
        {
            Die();
        }
    }

    public virtual void DealDamage(IBattleEntity target, int rawDamage, DamageType damageType)
    {
        int damage = rawDamage;
        if(damage == 0)
            return;

        stats.NotifyOnDealDamage(this, target, ref damage, damageType);
        target.TakeDamage(damage, damageType);
    }

    public virtual void Heal( int healAmount )
    {
        stats.Heal( healAmount );
        Debug.Log($"{this} Healed: " + healAmount + ", Current Health: " + stats.health);
    }

    public virtual void Summon(GameObject prefab, SummonData rawData, Transform spawnPoint)
    {
        SummonData data = rawData;
        stats.NotifyOnSummon(this, ref rawData);

        var go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        var summon = go.GetComponent<Summonable>();
        if (summon != null)
        {
            summon.Init(data);
        }
    }

    public void Die()
    {
        Debug.Log("Enemy Death");
        Destroy(gameObject);
    }

    protected virtual EnemySkill SelectSkill()
    {
        if (skills.Count == 0) return null;
        
        EnemySkill selected = skills[currentSkillIndex];
        currentSkillIndex = (currentSkillIndex + 1) % skills.Count;
        return selected;
    }

    public GameObject GetPreview(int index)
    {
        if (index < 0 || index >= skillPreviews.Count)
        {
            Debug.LogWarning($"Preview index {index} out of range!");
            return null;
        }
        return skillPreviews[index];
    }

    protected void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.UnregisterRigidbody(rb);
            BattleManager.Instance.UnregisterEnemy(this);
            BattleManager.Instance.UnregisterEntity(this);
        }
            
    }
}

public abstract class EnemySkill
{
    protected Enemy caster;
    protected SkillData skillData;
    
    public abstract string skillName { get; }
    protected abstract int previewIndex { get; }
    protected virtual string animatorTrigger => null;

    protected GameObject CurrentPreview => caster.GetPreview(previewIndex);

    public EnemySkill(Enemy caster)
    {
        this.caster = caster;
    }

    public SkillData GetSkillData()
    {
        skillData = CalculateSkillData();
        return skillData;
    }
    
    protected abstract SkillData CalculateSkillData();

    public void ShowPreview()
    {
        if (skillData == null)
        {
            Debug.LogWarning($"{skillName}: SkillData is null!");
            return;
        }
        
        GameObject preview = CurrentPreview;
        if (preview != null)
        {
            preview.SetActive(true);
        }
    }

    public void HidePreview()
    {
        GameObject preview = CurrentPreview;
        if (preview != null)
        {
            preview.SetActive(false);
        }
    }
    
    // 更新預覽（每幀呼叫）
    public void UpdatePreview()
    {
        GameObject preview = CurrentPreview;
        if (preview == null || !preview.activeSelf) return;
        
        UpdatePreviewTransform();
    }
    
    // 子類覆寫來控制預覽的位置和旋轉
    protected virtual void UpdatePreviewTransform()
    {
        GameObject preview = CurrentPreview;
        if (preview == null) return;
        
        // 預設：只更新旋轉（位置跟著敵人）
        if (skillData.direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(skillData.direction.y, skillData.direction.x) * Mathf.Rad2Deg;
            preview.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public abstract IEnumerator Execute();
    protected virtual float GetAnimationDelay() => 0.5f;
}

public class SkillData
{
    public Vector2 position;    // 技能施放位置
    public Vector2 direction;   // 技能施放方向
    
    public SkillData(Vector2 position, Vector2 direction)
    {
        this.position = position;
        this.direction = direction;
    }
}