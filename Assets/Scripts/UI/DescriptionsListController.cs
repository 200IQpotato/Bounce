using UnityEngine;
using UnityEngine.UI;

public class DescriptionsListController : MonoBehaviour
{
    public static DescriptionsListController Instance { get; private set; }
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
        Debug.Log($"Mouse Pos: {pos}, Box Size: ({boxWidth}, {boxHeight}), Parent Size: ({parentWidth}, {parentHeight}), Overflow Right: {overflowRight}, Overflow Bottom: {overflowBottom}");

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

    public void Clear()
    {
        foreach (Transform child in root)
        {
            Destroy(child.gameObject);
        }
    }
}
