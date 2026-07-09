using UnityEngine;

[CreateAssetMenu(fileName = "PotionObject", menuName = "Scriptable Objects/PotionObject")]
public class PotionObject : ScriptableObject
{
    public string potionID;
    public Sprite icon;
    public string potionName => LocalizationManager.Instance.GetPotionName(potionID);
    public string description => LocalizationManager.Instance.GetPotionDescription(potionID);

    public virtual void OnUse(Player player) { }
}
