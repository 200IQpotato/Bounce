using UnityEngine;
using UnityEngine.UI;

public class PixelPerfectScrollRect : ScrollRect
{
    protected override void LateUpdate()
    {
        base.LateUpdate();

        // Ensure the content position is pixel perfect
        Vector2 position = content.anchoredPosition;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        content.anchoredPosition = position;
    }
}
