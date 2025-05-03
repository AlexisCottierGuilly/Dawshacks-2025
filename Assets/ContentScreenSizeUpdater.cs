using UnityEngine;

public class ContentScreenSizeUpdater : MonoBehaviour
{
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // match total child height and set to rect height

        float totalHeight = 0f;
        foreach (RectTransform child in rectTransform)
        {
            if (child.gameObject.activeSelf) // Only consider active children
            {
                totalHeight += child.rect.height;
            }
        }

        // Add padding if needed
        float padding = 20f; // Adjust this value as needed
        totalHeight += padding;

        // Set the height of the parent RectTransform
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width); // Maintain width
    }
}
