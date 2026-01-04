using UnityEngine;

[CreateAssetMenu(fileName = "Protein Powder", menuName = "Relics/ProteinPowder")]
public class ProteinPowder : RelicObject
{
    public int healthIncrease;
    public int attackIncrease;
    public override void OnEquip(Player player)
    {
        player.stats.ModifyMaxHealth(healthIncrease);
        player.stats.Heal(healthIncrease);
        player.stats.ModifyAttack(attackIncrease);
    }
    public override void OnUnequip(Player player)
    {
        player.stats.ModifyMaxHealth(-healthIncrease);
        player.stats.ModifyAttack(-attackIncrease);
    }
}
