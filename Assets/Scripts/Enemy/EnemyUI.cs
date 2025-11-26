using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text attackText;

    public void UpdateAttack(int attack)
    {
        attackText.text = attack.ToString();
    }
    public void UpdateHealth(int health)
    {
        healthText.text = health.ToString();
    }
}
