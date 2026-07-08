using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DescriptionBoxPrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    public void Init(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
    }
}