using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceText : MonoBehaviour
{
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;

    public void Init( EventChoice choice )
    {
        description.text = choice.choiceText;
        if ( choice.isAvailable )
        {
            image.color = Color.gray;
            description.color = Color.white;
        }
        else
        {
            image.color = Color.cyan;
            description.color = Color.red;
            description.text += $"( {choice.lockText} )";
        }
    }
}
