using UnityEngine;
using TMPro;
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;

    public void UpdateAttack(int attack)
    {
        attackText.text = attack.ToString();
    }
    public void UpdateHealth(int health)
    {
        healthText.text = health.ToString();
    }
}
