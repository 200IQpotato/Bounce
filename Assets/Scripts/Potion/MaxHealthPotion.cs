using UnityEngine;

[CreateAssetMenu(fileName = "Max Health Potion", menuName = "Potions/Max Health Potion")]
public class MaxHealthPotion : PotionObject
{
    public int maxHealthIncrease;

    public override void OnUse(Player player)
    {
        player.stats.ModifyMaxHealth(maxHealthIncrease);
        Debug.Log($"Used Max Health Potion. Increased max health by {maxHealthIncrease}.");
    }
}
