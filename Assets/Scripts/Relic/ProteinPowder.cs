using UnityEngine;

[CreateAssetMenu(fileName = "Protein Powder", menuName = "Relics/ProteinPowder")]
public class ProteinPowder : RelicObject
{
    public int healthIncrease;
    public int attackIncrease;
    public override void OnEquip(Player player)
    {
        player.stats.maxHealth += healthIncrease;
        player.stats.health += healthIncrease;
        player.stats.attack += attackIncrease;
    }
    public override void OnUnequip(Player player)
    {
        player.stats.maxHealth -= healthIncrease;
        player.stats.health = Mathf.Min(player.stats.health, player.stats.maxHealth);
        player.stats.attack -= attackIncrease;
    }
}
