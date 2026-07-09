using UnityEngine;

[CreateAssetMenu(fileName = "Magic Book", menuName = "Relics/Magic Book")]
public class MagicBook : RelicObject
{
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private int fireDamage;
    public override void OnTurnStart(Player player)
    {
        player.Summon(fireBallPrefab, new SummonData(fireDamage, player), player.transform);
        OnTrigger();
    }
}
