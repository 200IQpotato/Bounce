using UnityEngine;
using UnityEngine.UI;

public class EventTextUI : MonoBehaviour
{
    public static EventTextUI Instance;
    [SerializeField] private RectTransform root;
    [SerializeField] private Text descriptionText;
    
    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string desc)
    {
        descriptionText.text = desc;
        root.gameObject.SetActive(true);

        // 強制更新大小
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    public void Hide()
    {
        root.gameObject.SetActive(false);
    }
}
