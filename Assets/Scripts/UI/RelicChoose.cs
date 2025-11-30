using UnityEngine;
using UnityEngine.UI;

public class RelicChoose : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Image image;
    [SerializeField] public Image selectedImage;

    public void Init( RelicObject relicObject )
    {
        text.text = relicObject.description;
        image.sprite = relicObject.icon;
    }
}
