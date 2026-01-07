using UnityEngine;

public class Merchant : MonoBehaviour
{
    private ShopController shopController;
    public void Init( ShopController shopController )
    {
        this.shopController = shopController;
    }

    void OnMouseDown()
    {
        shopController.EndShopping();
    }
}
