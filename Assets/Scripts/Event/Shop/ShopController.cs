using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopController : MonoBehaviour
{
    public GameObject merchantPrefab;
    private Merchant merchantInstance;
    public Vector2 merchantSpawnPoint;

    public GameObject shopItemPrefab;
    private List<GameObject> shopItems = new List<GameObject>();
    public List<Vector2> shopItemSpawnPoint;
    public int shopPotionCount;
    public int shopRelicCount;
    
    private bool isShopping = false;

    public Sprite healthPotionSprite;
    public Sprite maxHealthPotionSprite;
    public Sprite attackPotionSprite;

    public IEnumerator RunShop(EventManager manager)
    {
        isShopping = true;
        merchantInstance = Instantiate(merchantPrefab, merchantSpawnPoint, Quaternion.identity).GetComponent<Merchant>();
        merchantInstance.Init(this);

        for ( int i = 0; i < shopPotionCount; i++ )
        {
            var item = Instantiate(shopItemPrefab, shopItemSpawnPoint[i], Quaternion.identity);
            int potionIndex = Random.Range(0, 3);
            if ( potionIndex == 0 )
            {
                item.GetComponent<ShopItem>().Init(healthPotionSprite, 50, ShopItemType.Potion, "heal 25", this);
                item.GetComponent<ShopItem>().onItemPurchased += () => {
                    GameManager.Instance.playerInstance.stats.Heal(25);
                };
            }
            else if ( potionIndex == 1 )
            {
                item.GetComponent<ShopItem>().Init(maxHealthPotionSprite, 75, ShopItemType.Potion, "+3 max health", this);
                item.GetComponent<ShopItem>().onItemPurchased += () => {
                    GameManager.Instance.playerInstance.stats.ModifyMaxHealth(3);
                };
            }
            else if ( potionIndex == 2 )
            {
                item.GetComponent<ShopItem>().Init(attackPotionSprite, 75, ShopItemType.Potion, "+1 attack", this);
                item.GetComponent<ShopItem>().onItemPurchased += () => {
                    GameManager.Instance.playerInstance.stats.ModifyAttack(1);
                };
            }
            shopItems.Add(item);
        }

        var relics = RelicManager.Instance.GetRandomRelic(shopRelicCount);
        for ( int i = 0; i < shopRelicCount; i++ )
        {
            var item = Instantiate(shopItemPrefab, shopItemSpawnPoint[i+shopPotionCount], Quaternion.identity);
            var relic = relics[i];
            item.GetComponent<ShopItem>().Init(relic.icon, 100, ShopItemType.Relic, relic.description, this);
            item.GetComponent<ShopItem>().onItemPurchased += () =>{
                RelicManager.Instance.AddRelicToPlayer(relic);
            };
            shopItems.Add(item);
        }

        while ( isShopping )
        {
             yield return manager.StartCoroutine(GameManager.Instance.playerInstance.TakeTurn());
        }
        yield return new WaitUntil(() => !isShopping);
        CloseShop();
    }
    
    public void BuyItem(GameObject item)
    {
        if ( shopItems.Contains(item) )
        {
            shopItems.Remove(item);
            Destroy(item);
        }
    }

    public void EndShopping()
    {
        isShopping = false;
    }

    public void CloseShop()
    {
        if ( merchantInstance != null )
        {
            Destroy(merchantInstance.gameObject);
        }

        foreach ( var item in shopItems )
        {
            Destroy(item);
        }
        shopItems.Clear();
    }
}
