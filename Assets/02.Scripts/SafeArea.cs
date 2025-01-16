using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private RectTransform rectTr = null;
    private Rect screenSafeArea = new Rect(0f, 0f, 0f, 0f);
    private Vector2 minAnchor = Vector2.zero;
    private Vector2 maxAnchor = Vector2.zero;

    private void Awake()
    {
        MatchRectTrasnformToSafeArea();
    }

    private void FixedUpdate()
    {
        MatchRectTrasnformToSafeArea();
    }

    private void MatchRectTrasnformToSafeArea()
    {
        rectTr = GetComponent<RectTransform>();
        screenSafeArea = Screen.safeArea;
        minAnchor = screenSafeArea.position;
        maxAnchor = minAnchor + screenSafeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTr.anchorMin = minAnchor;
        rectTr.anchorMax = maxAnchor;
    }
}
