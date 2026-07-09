using UnityEngine;

[CreateAssetMenu(fileName = "Strength Potion", menuName = "Potions/Strength Potion")]
public class StrengthPotion : PotionObject
{
    public int attackIncrease;

    public override void OnUse(Player player)
    {
        player.stats.ModifyAttack(attackIncrease);
        Debug.Log($"Used Strength Potion. Increased attack by {attackIncrease}.");
    }
}
