using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EnemyDescriptionEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EffectObject effectObject;
    private Stats ownerStats;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI stackText;

    public void SetEffect(EffectObject effectObject, Stats stats)
    {
        this.effectObject = effectObject;
        ownerStats = stats;
        icon.sprite = effectObject.icon;
        stackText.text = stats.GetTotalStackCount(effectObject).ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DescriptionsListController.Instance.Clear();
        var instances = ownerStats.effects.FindAll(e => e.effectObject == effectObject);
        string effectInfo = string.Empty;
        foreach (var instance in instances)
        {
            string effectStackCount = DescriptionsListController.Instance.GetGlossaryFormat("StackCount", instance.stackCount);
            string effectDuration = DescriptionsListController.Instance.GetGlossaryFormat("TurnsRemaining", instance.duration);
            effectInfo += effectStackCount + ", " + effectDuration + "\n";
        }
        DescriptionsListController.Instance.AddDescription(LocalizationManager.Instance.GetEffectName(effectObject.effectID), effectInfo);
        DescriptionsListController.Instance.ContinueCreateDescriptionBoxWithId(effectObject.effectID);
        DescriptionsListController.Instance.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescriptionsListController.Instance.Hide();
    }
}
