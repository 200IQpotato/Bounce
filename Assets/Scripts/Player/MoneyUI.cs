using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private Text moneyText;
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

        player.stats.OnMoneyChanged += MoneyChange;

        MoneyChange();
    }

    public void MoneyChange()
    {
        moneyText.text = player.stats.money.ToString();
    }

    void OnDestroy()
    {
        player.stats.OnMoneyChanged -= MoneyChange;
        GameManager.Instance.OnPlayerSpawned -= Bind;
    }
}
