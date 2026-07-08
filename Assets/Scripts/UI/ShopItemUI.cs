using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public static ShopItemUI Instance;

    [SerializeField] private RectTransform root;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text costText;

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
        Hide();
    }

    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform,
            Input.mousePosition,
            null,
            out pos
        );
        root.localPosition = pos + new Vector2(20, -20);
    }

    public void Show(string desc, int cost)
    {
        descriptionText.text = desc;
        costText.text = $"Cost: {cost}";
        root.gameObject.SetActive(true);

        // 強制更新大小
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    public void Hide()
    {
        root.gameObject.SetActive(false);
    }
}