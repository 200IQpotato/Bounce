using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopEvent", menuName = "Event/ShopEvent")]
public class ShopEvent : EventObject
{
    public Sprite image;
    public List<EventChoice> choices;
    public ShopController shopControllerPrefab;

    public override IEnumerator Execute(EventManager manager)
    {
        yield return manager.eventUI.ShowChoices("Welcome to the shop! What would you like to do?", image, choices);
        int result = manager.eventUI.GetResult();

        if ( result == 0)
        {
            var shop = Instantiate(shopControllerPrefab);
            yield return shop.RunShop(manager);
        }
        else if (result == 1)
        {
            // rob the shop logic
        }
        else
        {
            //leave
        }

        yield break;
    }

    
}
