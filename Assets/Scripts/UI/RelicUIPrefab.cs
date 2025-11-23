using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicUIPrefab : MonoBehaviour
{
    [SerializeField] private Image relicIcon;
    [SerializeField] private TextMeshProUGUI value;

    public void SetRelicUI(RelicObject relic, int value)
    {
        relicIcon.sprite = relic.icon;
        if ( relic.valueType != ValueType.None )
        {
            this.value.text = value.ToString();
            this.value.gameObject.SetActive(true);
        }
        else
        {
            this.value.gameObject.SetActive(false);
        }
    }
}
