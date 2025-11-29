using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicUIPrefab : MonoBehaviour
{
    [SerializeField] private Image relicIcon;
    [SerializeField] private TextMeshProUGUI value;
    private ValueType valueType;
    private delegate int GetValue(int rawValue);
    GetValue getValue;

    public void OnEnable()
    {
        RelicManager.OnRelicValueUpdatedUI += UpdateValue;
    }

    public void OnDisable()
    {
        RelicManager.OnRelicValueUpdatedUI -= UpdateValue;
    }

    public void SetRelicUI(RelicObject relic, int value)
    {
        relicIcon.sprite = relic.icon;
        getValue = relic.GetUIValue;
        valueType = relic.valueType;
        if ( relic.valueType != ValueType.None )
        {
            this.value.text = getValue(value).ToString();
            this.value.gameObject.SetActive(true);
        }
        else
        {
            this.value.gameObject.SetActive(false);
        }
    }

    public void UpdateValue(ValueType valueType, int newValue)
    {
        if ( this.valueType == valueType )
        {
            value.text = getValue(newValue).ToString();
        }
    }
}
