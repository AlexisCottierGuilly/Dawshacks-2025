using UnityEngine;

public class LinePositionManager : MonoBehaviour
{
    public float widthRatio = 0.5f;
    public GameObject parent;

    void Update()
    {
        // update position X to the parent object's position, with ratio
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentRectTransform = parent.GetComponent<RectTransform>();

        Vector3 parentPos = parentRectTransform.anchoredPosition3D;
        Vector3 newPos = new Vector3(parentPos.x * widthRatio, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);
        rectTransform.anchoredPosition3D = newPos;
    }
}
