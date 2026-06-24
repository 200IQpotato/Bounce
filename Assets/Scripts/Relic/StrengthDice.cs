using UnityEngine;

[CreateAssetMenu(fileName = "Strength Dice", menuName = "Relics/Strength Dice")]
public class StrengthDice : RelicObject
{
    public EffectObject strengthEffect;
    public int maxStack;
    public int maxDuration;

    public override void OnTurnStart(Player player)
    {
        player.stats.ApplyEffect(new Effect(strengthEffect, Random.Range(0, maxDuration+1), Random.Range(0, maxStack+1))); 
        OnTrigger();
    }
}
