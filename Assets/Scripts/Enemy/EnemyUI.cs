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

    void Start()
    {
        caster = GetComponentInParent<Enemy>();

        caster.stats.OnHealthChanged += UpdateHealth;
        caster.stats.OnAttackChanged += UpdateAttack;
    }

    void Update()
    {
        healthText.rectTransform.position = Camera.main.WorldToScreenPoint(new Vector2(caster.transform.position.x - offsetX, caster.transform.position.y + offsetY));
        attackText.rectTransform.position = Camera.main.WorldToScreenPoint(new Vector2(caster.transform.position.x + offsetX, caster.transform.position.y + offsetY));
    }

    public void UpdateAttack(int attack)
    {
        attackText.text = attack.ToString();
    }
    public void UpdateHealth(int health)
    {
        healthText.text = health.ToString();
    }
}
