using UnityEngine;

// Indicates a gameobject that can have parts on it
public interface IPlaceableSurface
{
    // Returns the item if there is an item at that position
    GameObject PickUpItem(Vector2 position);

    // Returns bool based on if item was successfully placed at that position
    bool PlaceItem(GameObject item, Vector2 position);
}