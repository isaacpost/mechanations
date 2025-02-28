using UnityEngine;

// Script on menu manager to change cursor
public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorSprite;  // Assign this in the Inspector
    public Vector2 hotSpot = Vector2.zero; // Adjust to center cursor if needed

    void Start()
    {
        Cursor.SetCursor(cursorSprite, hotSpot, CursorMode.Auto);
    }
}
