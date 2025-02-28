using System.Collections.Generic;
using UnityEngine;

// Controls logic for placing and picking up item from the main grid
public class GridManager : MonoBehaviour, IPlaceableSurface
{

    [SerializeField] 
    private List<GameObject> gridDestroyOptions; // Options of tiles that could be destroyed

    private string tagToSearch; // Tag of objects on the grid to search for in CountChildrenWithTag()

    // Picks up part if there is one at position
    public GameObject PickUpItem(Vector2 position)
    {
        LayerMask partLayerMask = LayerMask.GetMask("Parts");
        Collider2D collider = Physics2D.OverlapPoint(position, partLayerMask);

        if (collider != null)
        {
            collider.gameObject.GetComponent<Part>().SetIsPlaced(false);
            return collider.gameObject; // On Success
        }
        else
        {
            return null; // On failiure
        }
    }

    // Places part at position
    public bool PlaceItem(GameObject item, Vector2 position)
    {
        LayerMask gridLayerMask = LayerMask.GetMask("Grid");
        Collider2D gridCollider = Physics2D.OverlapPoint(position, gridLayerMask);

        LayerMask partLayerMask = LayerMask.GetMask("Parts");
        Collider2D partCollider = Physics2D.OverlapPoint(position, partLayerMask);

        // Places only if there is a grid and there isnt a part there
        if (gridCollider != null && partCollider == null)
        {
            GameObject gridLocation = gridCollider.gameObject;

            item.transform.SetParent(gridLocation.transform);
            item.transform.localPosition = Vector2.zero;
            item.GetComponent<Part>().SetIsPlaced(true);

            return true; // On success
        }
        else
        {
            return false; // On failure
        }
    }

    public float GetBossRatio()
    {
        return CountChildrenWithTag("Part") / CountChildrenWithTag("Grid");
    }

    // This function counts the number of children with the specified tag
    private float CountChildrenWithTag(string tag)
    {
        float count = 0;
        tagToSearch = tag;
        CountChildrenRecursive(transform, ref count);
        return count;
    }

    // Recursive function to traverse and count children with the tag
    private void CountChildrenRecursive(Transform parent, ref float count)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tagToSearch))
            {
                count++;
            }

            // Recursively check the child's children
            CountChildrenRecursive(child, ref count);
        }
    }

    public Transform FindRightmostPart()
    {
        Transform rightmost = null;
        float maxX = float.MinValue;

        foreach (Transform child in transform)
        {
            foreach (Transform grandchild in child)
            {
                if (grandchild.position.x > maxX && grandchild.CompareTag("Part"))
                {
                    maxX = grandchild.position.x;
                    rightmost = grandchild;
                }
            }
        }
        return rightmost;
    }

    public GameObject GetAndRemoveRandomGrid()
    {
        if (gridDestroyOptions == null || gridDestroyOptions.Count == 0)
        {
            return null; // Return null if the list is empty
        }

        // First, try to find a grid with a child that has the specified tag
        GameObject selectedObject = null;

        foreach (var grid in gridDestroyOptions)
        {
            // Check if any child of the grid has the specified tag
            if (FindChildWithTag(grid.transform, "Part") != null)
            {
                selectedObject = grid;
                break; // Stop once we find a grid with the child tag
            }
        }

        // If no grid with the child tag is found, choose a random one
        if (selectedObject == null)
        {
            int index = Random.Range(0, gridDestroyOptions.Count);
            selectedObject = gridDestroyOptions[index];
        }

        // Remove the selected object from the list and return it
        gridDestroyOptions.Remove(selectedObject);
        return selectedObject;
    }

    private Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child; // Return the child with the tag
            }
        }
        return null; // Return null if no child with the tag is found
    }
}
