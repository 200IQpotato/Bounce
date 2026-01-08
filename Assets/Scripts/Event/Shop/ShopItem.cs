using UnityEngine;

public enum ShopItemType { Potion, Relic }

public class ShopItem : MonoBehaviour
{
    private ShopController shopController;
    [SerializeField] private SpriteRenderer itemSpriteRenderer;
    private int itemCost;
    private ShopItemType itemType;
    private string description;
    public System.Action onItemPurchased;

    void OnMouseDown()
    {
        if (  GameManager.Instance.playerInstance.stats.IsAffordable(itemCost) )
        {
            GameManager.Instance.playerInstance.stats.ModifyMoney(-itemCost);
            onItemPurchased?.Invoke();
            ShopItemUI.Instance.Hide();
            shopController.BuyItem(gameObject);
        }
    }

    void OnMouseEnter()
    {
        ShopItemUI.Instance.Show(
            description,
            itemCost
        );
    }

    void OnMouseExit()
    {
        ShopItemUI.Instance.Hide();
    }

    public void Init( Sprite itemSprite, int cost, ShopItemType itemType, string desc, ShopController shopController )
    {
        this.shopController = shopController;
        itemSpriteRenderer.sprite = itemSprite;
        itemCost = cost;
        description = desc;
        this.itemType = itemType;
    }
}
