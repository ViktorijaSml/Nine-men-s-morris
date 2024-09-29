using UnityEngine;

public class ResizeCanvas : MonoBehaviour
{
    /// <summary>
    /// This scripts helps to solve a bug with positioning of the generated lines to match the screen size.
    /// Good to use this method to manipulate the borders of the GameCanvas
    /// </summary>
    void OnEnable()
    {
        RectTransform parentRect = GetComponentInParent<RectTransform>(); //MainCanvas
        RectTransform rectTransform = GetComponent<RectTransform>(); //GameCanvas

        float widthFactor = 0.7f;
        float heightFactor = 0.7f;

        float newWidth = parentRect.rect.width * widthFactor;
        float newHeight = parentRect.rect.height * heightFactor;

        rectTransform.sizeDelta = new Vector2(newWidth, newHeight);

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
