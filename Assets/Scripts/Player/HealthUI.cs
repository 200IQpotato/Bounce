using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Text healthText;
    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance.playerInstance != null)
        {
            Bind(GameManager.Instance.playerInstance);
        }
        else
        {
            GameManager.Instance.OnPlayerSpawned += Bind;
        }
    }

    void Bind(Player player)
    {
        this.player = player;

        player.stats.OnHealthChanged += HealthChange;
        player.stats.OnMaxHealthChanged += HealthChange;

        HealthChange();
    }

    public void HealthChange()
    {
        healthText.text = player.stats.health.ToString() + " / " + player.stats.maxHealth.ToString();
    }

    void OnDestroy()
    {
        player.stats.OnHealthChanged -= HealthChange;
        player.stats.OnMaxHealthChanged -= HealthChange;
        GameManager.Instance.OnPlayerSpawned -= Bind;
    }
}
