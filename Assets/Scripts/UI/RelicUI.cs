using System.Collections.Generic;
using UnityEngine;

public class RelicUI : MonoBehaviour
{
    public RectTransform content;
    public RelicUIPrefab relicPrefab;
    private Dictionary<RelicObject, RelicUIPrefab> relicUIs = new();

    void OnEnable()
    {
        RelicManager.OnRelicAddedUI += AddRelic;
        RelicManager.OnRelicRemovedUI += RemoveRelic;
        RelicManager.OnRelicTriggered += RelicTrigger;
    }

    void OnDisable()
    {
        RelicManager.OnRelicAddedUI -= AddRelic;
        RelicManager.OnRelicRemovedUI -= RemoveRelic;
        RelicManager.OnRelicTriggered -= RelicTrigger;
    }

    public void AddRelic(RelicObject relic, int value)
    {
        if (!relicUIs.ContainsKey(relic))
        {
            RelicUIPrefab ui = Instantiate(relicPrefab, content);
            ui.SetRelicUI(relic, value);
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

    public void RelicTrigger(RelicObject relic)
    {
        relicUIs[relic].Glow();
    }
}
