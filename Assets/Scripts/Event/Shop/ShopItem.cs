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
        DescriptionsListController.Instance.Clear();

        DescriptionData data = LocalizationManager.Instance.Get(itemID);
        string fullDescription = $"Cost: <color=#FFFA67>{itemCost}</color><br>{data.description}";
        DescriptionsListController.Instance.AddDescription(data.name, fullDescription); // 主要說明本身(第一格,原始文字保留<link>讓TMP渲染出顏色/樣式)

        var seenIds = new HashSet<string>();
        foreach (Match match in linkPattern.Matches(data.description))
        {
            string linkId = match.Groups[1].Value; // 例如 "Effects/Poison"
            if (seenIds.Add(linkId))
            {
                DescriptionData resolved = LocalizationManager.Instance.ResolveLink(linkId);
                DescriptionsListController.Instance.AddDescription(resolved.name, resolved.description);
            }
        }

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

    public void Init( Sprite itemSprite, int cost, ShopItemType itemType, DescriptionData data, ShopController shopController )
    {
        this.shopController = shopController;
        itemSpriteRenderer.sprite = itemSprite;
        itemCost = cost;
        this.itemType = itemType;
    }
}
