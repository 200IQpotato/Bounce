using UnityEngine;

[CreateAssetMenu(fileName = "Health Potion", menuName = "Potions/Health Potion")]
public class HealthPotion : PotionObject
{
    public int healAmount;
    public override void OnUse(Player player)
    {
        player.Heal(healAmount);
        Debug.Log($"Used Health Potion. Healed {healAmount} health.");
    }
}
