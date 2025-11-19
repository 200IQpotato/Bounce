using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenBackdropClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseCurrentScreen();
        }
    }
}
