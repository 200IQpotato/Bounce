using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicChoose : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text relicName;
    [SerializeField] public Image selectedImage;

    public void Init( RelicObject relicObject )
    {
        text.text = relicObject.description;
        image.sprite = relicObject.icon;
        relicName.text = relicObject.relicName;
    }
}
