using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class DescriptionLinkHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI tmpText;
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        var linkInfos = tmpText.textInfo.linkInfo;
        if (linkInfos.Length == 0) return;
        DescriptionsListController.Instance.Clear();
        for (int i = 0; i < linkInfos.Length; i++)
        {
            DescriptionsListController.Instance.ContinueCreateDescriptionBoxWithLink(linkInfos[i].GetLinkID());
        }

        DescriptionsListController.Instance.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescriptionsListController.Instance.Hide();
    }
}
