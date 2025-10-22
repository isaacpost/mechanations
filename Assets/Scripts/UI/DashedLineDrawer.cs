using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class DashedLineDrawer : MonoBehaviour
{
    public Canvas cursorCanvas;

    [Header("Line Settings")]
    public float lineLength = 5f;            // How far the line extends
    public float lineWidth = 0.05f;          // Thickness of the line
    public float startOffset = 0.2f;         // Distance from transform before the line starts
    public Color lineColor = Color.white;    // Line color
    public float rotationOffset = 0f;        // Extra rotation (e.g., 90 degrees for sideways)

    [Header("Dash Settings")]
    [Min(0.01f)] public float dashLength = 1f; // World units per dash+gap cycle

    private LineRenderer lineRenderer;
    private Material lineMaterial;

    private enum LineMode { ObjectRotation, MouseDirection }
    private LineMode mode;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Setup line renderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true; // Positions are absolute in world space

        // Create and assign dashed texture
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        Texture2D dashTex = MakeDashTexture();
        lineMaterial.mainTexture = dashTex;
        lineMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;

        lineRenderer.material = lineMaterial;

        // Proper texture tiling mode
        lineRenderer.textureMode = LineTextureMode.Tile;

        // Keep line rotation consistent with object
        lineRenderer.alignment = LineAlignment.TransformZ;

        // Initial color
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        // Decide line behavior based on tag
        if (CompareTag("Player"))
            mode = LineMode.MouseDirection;
        else
            mode = LineMode.ObjectRotation;
    }

    private void Update()
    {
        if (!lineRenderer.enabled) return;

        Vector3 direction;

        // Determine direction based on mode
        if (mode == LineMode.ObjectRotation)
        {
            direction = transform.up;
        }
        else // MouseDirection
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                cursorCanvas.transform as RectTransform,
                mousePos,
                cursorCanvas.worldCamera,
                out Vector3 worldPoint
            );

            // Direction toward mouse
            direction = (worldPoint - transform.position).normalized;
        }

        // Apply additional rotation offset
        direction = Quaternion.Euler(0, 0, rotationOffset) * direction;

        // ✅ Ignore Z component completely so it's locked to XY plane
        direction.z = 0f;
        direction.Normalize();

        // Compute start and end points using full lineLength only on X/Y axes
        Vector3 start = transform.position + direction * startOffset;
        Vector3 end = start + direction * lineLength;

        // ✅ Keep line flat on the same Z as the object
        float fixedZ = transform.position.z;
        start.z = fixedZ;
        end.z = fixedZ;

        // Apply line positions
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Apply color
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        // DASH SCALING FIX — use constant line length
        float repeats = lineLength / dashLength;
        lineRenderer.material.mainTextureScale = new Vector2(repeats, 1f);
    }


    /// <summary>
    /// Creates a simple white dash texture: white dash followed by transparent gap
    /// </summary>
    private Texture2D MakeDashTexture()
    {
        int dashPixels = 8; // Length of the visible dash in pixels
        int gapPixels = 8;  // Length of the gap in pixels
        int texWidth = dashPixels + gapPixels;

        Texture2D tex = new Texture2D(texWidth, 1);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;

        // Fill dash section
        for (int i = 0; i < dashPixels; i++)
            tex.SetPixel(i, 0, Color.white);

        // Fill gap section
        for (int i = dashPixels; i < texWidth; i++)
            tex.SetPixel(i, 0, Color.clear);

        tex.Apply();
        return tex;
    }

    // --- Public Methods ---

    /// <summary>
    /// Toggle line visibility on/off
    /// </summary>
    public void SetActive(bool state)
    {
        lineRenderer.enabled = state;
    }

    /// <summary>
    /// Update the line's color dynamically
    /// </summary>
    public void SetColor(Color newColor)
    {
        lineColor = newColor;
        lineRenderer.startColor = newColor;
        lineRenderer.endColor = newColor;
    }
}
