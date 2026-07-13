using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class RelicUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image relicIcon;
    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private Outline outline;
    private RelicObject relic;
    public float glowDuration; // Duration of the glow effect in seconds
    public float glowAlpha; // Alpha value for the glow effect
    private ValueType valueType;
    private bool isDragging = false;
    private delegate int GetValue(int rawValue);
    GetValue getValue;

    public void OnEnable()
    {
        RelicManager.OnRelicValueUpdatedUI += UpdateValue;
        GameManager.Instance.playerInstance.OnDragStateChanged += SetCanShowDescription;
    }

    public void OnDisable()
    {
        RelicManager.OnRelicValueUpdatedUI -= UpdateValue;
        GameManager.Instance.playerInstance.OnDragStateChanged -= SetCanShowDescription;
    }

    public void SetRelicUI(RelicObject relic, int value)
    {
        this.relic = relic;
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

    public void SetCanShowDescription(bool isDragging)
    {
        this.isDragging = isDragging;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        DescriptionsListController.Instance.Clear();
        DescriptionsListController.Instance.CreateDescriptionBoxWithId(relic.relicID);
        DescriptionsListController.Instance.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescriptionsListController.Instance.Hide();
    }
}
