using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UICursorFollower : MonoBehaviour
{
    [SerializeField] private RectTransform uiCursor; // Your cursor image
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color darkenedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private Canvas parentCanvas;
    private Image cursorImage;
    private PointerEventData pointerData;
    private List<GraphicRaycaster> allRaycasters = new List<GraphicRaycaster>();
    private RectTransform canvasRect;

    private void Start()
    {
        if (uiCursor != null)
            uiCursor.gameObject.SetActive(true);

        parentCanvas = uiCursor.GetComponentInParent<Canvas>();
        canvasRect = parentCanvas.GetComponent<RectTransform>();
        cursorImage = uiCursor.GetComponent<Image>();

        // Get all active GraphicRaycasters in the scene except this cursor canvas
        foreach (var ray in FindObjectsOfType<GraphicRaycaster>())
        {
            if (ray.gameObject != parentCanvas.gameObject)
                allRaycasters.Add(ray);
        }

        pointerData = new PointerEventData(EventSystem.current);
        Cursor.visible = false; // hide system cursor
    }

    private void Update()
    {
        if (uiCursor == null || Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 newPos = uiCursor.position;

        // --- Move cursor depending on render mode ---
        switch (parentCanvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                newPos = mousePos;
                break;

            case RenderMode.ScreenSpaceCamera:
                if (parentCanvas.worldCamera != null)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentCanvas.transform as RectTransform,
                        mousePos,
                        parentCanvas.worldCamera,
                        out Vector2 localPoint
                    );
                    uiCursor.localPosition = localPoint;
                    return;
                }
                break;

            case RenderMode.WorldSpace:
                if (parentCanvas.worldCamera != null)
                {
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        parentCanvas.transform as RectTransform,
                        mousePos,
                        parentCanvas.worldCamera,
                        out Vector3 worldPoint
                    );
                    newPos = worldPoint;
                }
                break;
        }

        // --- Clamp cursor position to canvas bounds ---
        if (canvasRect != null)
        {
            Vector3[] corners = new Vector3[4];
            canvasRect.GetWorldCorners(corners);

            float clampedX = Mathf.Clamp(newPos.x, corners[0].x, corners[2].x);
            float clampedY = Mathf.Clamp(newPos.y, corners[0].y, corners[2].y);
            newPos = new Vector3(clampedX, clampedY, newPos.z);
        }

        uiCursor.position = newPos;

        // --- Check if cursor is over a clickable element ---
        pointerData.position = mousePos;
        bool overClickable = false;

        foreach (var raycaster in allRaycasters)
        {
            if (raycaster == null || !raycaster.isActiveAndEnabled)
                continue;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            foreach (var r in results)
            {
                if (!r.gameObject.activeInHierarchy) continue;

                if (r.gameObject.GetComponent<Button>() ||
                    r.gameObject.GetComponent<Toggle>() ||
                    r.gameObject.GetComponent<Slider>() ||
                    r.gameObject.GetComponent<Scrollbar>())
                {
                    overClickable = true;
                    break;
                }
            }

            if (overClickable) break;
        }

        cursorImage.color = overClickable ? normalColor : darkenedColor;
    }
}
