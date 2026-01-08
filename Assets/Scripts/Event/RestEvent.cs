using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RestEvent", menuName = "Event/RestEvent")]
public class RestEvent : EventObject
{
    public Sprite image;
    public List<EventChoice> choices;
    public int healPercentage;
    public int increaseMaxHealth;
    public int increaseAttack;
    public override IEnumerator Execute(EventManager manager)
    {
        yield return manager.eventUI.ShowChoices("You have time to rest now.", image, choices);
        int result = manager.eventUI.GetResult();

        if (result == 0)
        {
            Player player = GameManager.Instance.playerInstance;
            player.stats.Heal(player.stats.maxHealth * healPercentage / 100);
        }
        else if (result == 1)
        {
            Player player = GameManager.Instance.playerInstance;
            player.stats.ModifyMaxHealth(increaseMaxHealth);
            player.stats.Heal(increaseMaxHealth);
        }
        else
        {
            Player player = GameManager.Instance.playerInstance;
            player.stats.ModifyAttack(increaseAttack);
        }
    }
}
