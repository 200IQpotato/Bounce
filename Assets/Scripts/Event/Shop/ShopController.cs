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
    [SerializeField] 
    private List<PotionObject> potions = new List<PotionObject>();
    public List<Vector2> shopItemSpawnPoint;
    public int shopPotionCount;
    public int shopRelicCount;
    
    private bool isShopping = false;

    public IEnumerator RunShop(EventManager manager)
    {
        isShopping = true;
        merchantInstance = Instantiate(merchantPrefab, merchantSpawnPoint, Quaternion.identity).GetComponent<Merchant>();
        merchantInstance.Init(this);

        for ( int i = 0; i < shopPotionCount; i++ )
        {
            var item = Instantiate(shopItemPrefab, shopItemSpawnPoint[i], Quaternion.identity);
            int potionIndex = Random.Range(0, potions.Count);
            
            item.GetComponent<ShopItem>().Init(potions[potionIndex].icon, 50, ShopItemType.Potion, potions[potionIndex].potionID, this);
            item.GetComponent<ShopItem>().onItemPurchased += () => { potions[potionIndex].OnUse(GameManager.Instance.playerInstance); };
            shopItems.Add(item);
        }

        var relics = RelicManager.Instance.GetRandomRelic(shopRelicCount);
        for ( int i = 0; i < shopRelicCount; i++ )
        {
            if(relics != null)
            {
                var item = Instantiate(shopItemPrefab, shopItemSpawnPoint[i+shopPotionCount], Quaternion.identity);
                var relic = relics[i];
                item.GetComponent<ShopItem>().Init(relic.icon, 100, ShopItemType.Relic, relic.relicID, this);
                item.GetComponent<ShopItem>().onItemPurchased += () =>{
                    RelicManager.Instance.AddRelicToPlayer(relic);
                };
                shopItems.Add(item);
            }            
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
