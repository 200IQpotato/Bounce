using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicChoose : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text relicName;
    [SerializeField] public Image selectedImage;
    private RelicObject relicObject;

    public void Init( RelicObject relicObject )
    {
        this.relicObject = relicObject;
        text.text = relicObject.description;
        image.sprite = relicObject.icon;
        relicName.text = relicObject.relicName;
    }

    private void Refresh()
    {
        text.text = relicObject.description;
        relicName.text = relicObject.relicName;
    }

    public void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += Refresh;
    }

    public void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= Refresh;
    }
}
