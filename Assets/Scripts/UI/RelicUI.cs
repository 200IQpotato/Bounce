using System.Collections.Generic;
using UnityEngine;

public class RelicUI : MonoBehaviour
{
    public RectTransform content;
    public RelicUIPrefab relicPrefab;
    private Dictionary<RelicObject, RelicUIPrefab> relicUIs = new();

    void Awake()
    {
        RelicHolder.OnRelicAddedUI += AddRelic;
        RelicHolder.OnRelicRemovedUI += RemoveRelic;
        RelicHolder.OnRelicValueUpdatedUI += UpdateValue;
    }

    public void AddRelic(RelicObject relic, int value)
    {
        if (!relicUIs.ContainsKey(relic))
        {
            RelicUIPrefab ui = Instantiate(relicPrefab, content);
            ui.SetRelicUI(relic, relic.GetUIValue(value));
            relicUIs[relic] = ui;
        }
    }

    public void RemoveRelic(RelicObject relic)
    {
        if (relicUIs.ContainsKey(relic))
        {
            Destroy(relicUIs[relic].gameObject);
            relicUIs.Remove(relic);
        }
    }

    public void UpdateValue(ValueType type, int value)
    {
        foreach (var kvp in relicUIs)
        {
            RelicObject relic = kvp.Key;
            RelicUIPrefab ui = kvp.Value;
            if (relic.valueType == type)
            {
                ui.SetRelicUI(relic, relic.GetUIValue(value));
            }
        }
    }
}
