using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RelicUIPrefab : MonoBehaviour
{
    [SerializeField] private Image relicIcon;
    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private Outline outline;
    public float glowDuration; // Duration of the glow effect in seconds
    public float glowAlpha; // Alpha value for the glow effect
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

    public void Glow()
    {
        StartCoroutine(GlowCoroutine());
    }

    private IEnumerator GlowCoroutine()
    {
        float t = 0f;

        while (t < glowDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(glowAlpha, 0f, t / glowDuration);
            outline.effectColor = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        outline.effectColor = new Color(1f, 1f, 1f, 0f);
    }
}
