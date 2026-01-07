using UnityEngine;

public enum ShopItemType { Potion, Relic }

public class ShopItem : MonoBehaviour
{
    private ShopController shopController;
    [SerializeField] private SpriteRenderer itemSpriteRenderer;
    private int itemCost;
    private ShopItemType itemType;
    public System.Action onItemPurchased;

    void OnMouseDown()
    {
        if (  GameManager.Instance.playerInstance.stats.IsAffordable(itemCost) )
        {
            GameManager.Instance.playerInstance.stats.ModifyMoney(-itemCost);
            onItemPurchased?.Invoke();
            shopController.BuyItem(gameObject);
        }
    }

    public void Init( Sprite itemSprite, int cost, ShopItemType itemType, ShopController shopController )
    {
        this.shopController = shopController;
        itemSpriteRenderer.sprite = itemSprite;
        itemCost = cost;
        this.itemType = itemType;
    }
}
