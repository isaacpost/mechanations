using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Checks if there is a certain type of prefab part
// on all the tiles in the given list
public class GridChecker : MonoBehaviour
{
    [SerializeField] 
    private Transform root; // Parent

    [SerializeField] 
    private List<Transform> possibleTiles; // The tiles to search for the part

    [SerializeField] 
    private GameObject partPrefab; // Part that needs to be on all of the tiles

    private bool easterEggFound = false;

    private void Update()
    {
        if (!easterEggFound)
        {
            easterEggFound = ArePartsOnTiles();

            if (easterEggFound) 
            {
                GameManager.Instance.EasterEggFound(SceneManager.GetActiveScene().name);
                SFXManager.Instance.PlaySound("EasterEgg");
            }
        }
    }

    public bool ArePartsOnTiles()
    {
        foreach (Transform tile in possibleTiles)
        {
            if (tile == null || !HasPart(tile))
            {
                return false;
            }
        }
        return true;
    }

    private bool HasPart(Transform tile)
    {
        foreach (Transform child in tile)
        {
            if (child.gameObject.name.StartsWith(partPrefab.name))
            {
                return true;
            }
        }
        return false;
    }
}
