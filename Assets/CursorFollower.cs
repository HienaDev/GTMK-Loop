using UnityEngine;
using UnityEngine.UI;

public class CursorFollower : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Optional: hide system cursor
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out position
        );
        rectTransform.anchoredPosition = position;
    }
}
