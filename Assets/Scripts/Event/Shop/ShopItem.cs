using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public enum ShopItemType { Potion, Relic }

public class ShopItem : MonoBehaviour
{
    private static readonly Regex linkPattern = new Regex("<link=([^>]+)>");
    private ShopController shopController;
    [SerializeField] private SpriteRenderer itemSpriteRenderer;
    private int itemCost;
    private ShopItemType itemType;
    private string itemID;
    public System.Action onItemPurchased;

    void OnMouseDown()
    {
        if (  GameManager.Instance.playerInstance.stats.IsAffordable(itemCost) )
        {
            GameManager.Instance.playerInstance.stats.ModifyMoney(-itemCost);
            onItemPurchased?.Invoke();
            DescriptionsListController.Instance.Hide();
            shopController.BuyItem(gameObject);
        }
    }

    void OnMouseEnter()
    {
        DescriptionsListController.Instance.CreateDescriptionBoxWithId(itemID);
        DescriptionsListController.Instance.Show();
    }

    void OnMouseExit()
    {
        DescriptionsListController.Instance.Hide();
    }

    public void Init( Sprite itemSprite, int cost, ShopItemType itemType, string ItemID, ShopController shopController )
    {
        this.shopController = shopController;
        itemSpriteRenderer.sprite = itemSprite;
        itemCost = cost;
        this.itemID = ItemID;
        this.itemType = itemType;
    }
}
