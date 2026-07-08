using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class DescriptionLinkHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI tmpText;
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        DescriptionsListController.Instance.Clear();

        var seenIds = new HashSet<string>();
        var linkInfos = tmpText.textInfo.linkInfo;

        for (int i = 0; i < linkInfos.Length; i++)
        {
            string termId = linkInfos[i].GetLinkID();
            if (seenIds.Add(termId)) // 同一個詞出現兩次只加一次
            {
                DescriptionData data = LocalizationManager.Instance.ResolveLink(termId);
                DescriptionsListController.Instance.AddDescription(data.name, data.description);
            }
        }

        DescriptionsListController.Instance.Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescriptionsListController.Instance.Hide();
    }
}
