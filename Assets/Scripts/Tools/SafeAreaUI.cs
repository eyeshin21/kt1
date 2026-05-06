using UnityEngine;

public class SafeAreaUI : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField] bool isSafeBottom;
    [SerializeField] bool isSafeTop;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Rect safeArea = Screen.safeArea;
        Vector2 minAnchor = safeArea.position;
        Vector2 maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        if (isSafeBottom)
        {
            rectTransform.anchorMin = minAnchor;
        }

        if (isSafeTop)
        {
            rectTransform.anchorMax = maxAnchor;
        }
    }
}
