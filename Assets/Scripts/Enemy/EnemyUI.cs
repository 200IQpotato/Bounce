using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text attackText;
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;
    private Enemy caster;

    void Awake()
    {
        
    }

    void Update()
    {
        healthText.rectTransform.position = Camera.main.WorldToScreenPoint(new Vector2(caster.transform.position.x - offsetX, caster.transform.position.y + offsetY));
        attackText.rectTransform.position = Camera.main.WorldToScreenPoint(new Vector2(caster.transform.position.x + offsetX, caster.transform.position.y + offsetY));
    }

    public void SetCaster(Enemy e)
    {
        caster = e;
        UpdateHealth();
        UpdateAttack();
        caster.stats.OnHealthChanged += UpdateHealth;
        caster.stats.OnAttackChanged += UpdateAttack;
    }

    public void UpdateAttack()
    {
        attackText.text = caster.stats.GetAttack().ToString();
    }
    public void UpdateHealth()
    {
        healthText.text = caster.stats.health.ToString();
    }

    void OnDestroy()
    {
        caster.stats.OnHealthChanged -= UpdateHealth;
        caster.stats.OnAttackChanged -= UpdateAttack;
    }
}
