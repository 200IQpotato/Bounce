using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class DescriptionsListController : MonoBehaviour
{
    public static DescriptionsListController Instance { get; private set; }
    private static readonly Regex linkPattern = new Regex("<link=([^>]+)>");
    private readonly HashSet<string> seenIds = new HashSet<string>();
    [SerializeField] private RectTransform root;
    [SerializeField] private GameObject descriptionBox;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!root.gameObject.activeSelf) return;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform,
            Input.mousePosition,
            null,
            out pos
        );

        LayoutRebuilder.ForceRebuildLayoutImmediate(root);

        float boxWidth = root.rect.width;
        float boxHeight = root.rect.height;

        RectTransform parentRect = root.parent as RectTransform;
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        // 判斷是否超出右邊界 / 下邊界(以滑鼠為原點,parent的座標系通常是以中心為(0,0))
        bool overflowRight = pos.x + 2 + boxWidth > parentWidth / 2f;
        bool overflowBottom = pos.y - 2 - boxHeight < -parentHeight / 2f;
        //Debug.Log($"Mouse Pos: {pos}, Box Size: ({boxWidth}, {boxHeight}), Parent Size: ({parentWidth}, {parentHeight}), Overflow Right: {overflowRight}, Overflow Bottom: {overflowBottom}");

        float offsetX = overflowRight ? -2 - boxWidth : 2;
        float offsetY = overflowBottom ? 2 + boxHeight : -2;

        root.localPosition = pos + new Vector2(offsetX, offsetY);

    }

    public void Show()
    {
        root.gameObject.SetActive(true);
    }

    public void Hide()
    {
        root.gameObject.SetActive(false);
    }

    public void AddDescription(string name, string description)
    {
        GameObject descBox = Instantiate(descriptionBox, root);
        descBox.GetComponent<DescriptionBoxPrefab>().Init(name, description);
    }

    public void CreateDescriptionBoxWithId(string id)
    {
        Clear();
        DescriptionData data = LocalizationManager.Instance.Get(id);
        if (seenIds.Add(LocalizationManager.Instance.GetLink(id))) // 確保同一個ID不會被重複加入
        {
            AddDescription(data.name, data.description);
            foreach (Match match in linkPattern.Matches(data.description))
            {
                string linkId = match.Groups[1].Value; // 例如 "Effects/Poison"
                ContinueCreateDescriptionBoxWithLink(linkId);
            }
        }
        
    }

    public void CreateDescriptionBoxWithLink(string LinkId)
    {
        Clear();
        ContinueCreateDescriptionBoxWithLink(LinkId);
    }

    public void ContinueCreateDescriptionBoxWithLink(string LinkId)
    {
        DescriptionData data = ResolveLink(LinkId);
        if (seenIds.Add(LinkId))
        {
            AddDescription(data.name, data.description);
            foreach (Match match in linkPattern.Matches(data.description))
            {
                string linkId = match.Groups[1].Value; // 例如 "Effects/Poison"
                ContinueCreateDescriptionBoxWithLink(linkId);
            }
        }
    }

    public DescriptionData ResolveLink(string linkId) //傳入 "Relics/RelicID" 或 "Effects/EffectID" 或 "Glossary/GlossaryID" 這種格式的字串, 回傳對應的 DescriptionData
    {
        var parts = linkId.Split('/');
        if (parts.Length != 2)
        {
            Debug.LogWarning($"Malformed link id: {linkId}");
            return new DescriptionData { name = "Unknown", description = "Unknown ID" };
        }

        string category = parts[0];
        string id = parts[1];
        Debug.Log($"Resolving link: Category={category}, ID={id}");

        switch (category)
        {
            case "Relics":
                return LocalizationManager.Instance.GetRelicData(id);
            case "Effects":
                return LocalizationManager.Instance.GetEffectData(id);
            case "Glossary":
                return LocalizationManager.Instance.GetGlossaryData(id);
            default:
                Debug.LogWarning($"Unknown link category: {category}");
                return new DescriptionData { name = "Unknown", description = "Unknown ID" };
        }
    }

    public void Clear()
    {
        foreach (Transform child in root)
        {
            Destroy(child.gameObject);
        }
        seenIds.Clear();
    }
}
