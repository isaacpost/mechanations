using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Spawns and despawns parts onto a conveyorbelt
public class ConveyorBeltManager : MonoBehaviour, IPlaceableSurface
{

    [SerializeField] List<GameObject> prefabs; // List of possible prefabs to instantiate
    [SerializeField] float moveSpeed = 5f; // The speed at which instantiated parts move
    [SerializeField] float spawnInterval = 2f; // How often the parts are spawned
    [SerializeField] GameObject gridPrefab; // The grid prefab to instantate the parts on
    [SerializeField] float destroyDistance = 17f; // How far the parts need to travel before getting destroyed

    private bool isRunning = false;
    private float timer = 0f; // Keeps track of when to next instantate a part
    private static List<GameObject> instantiatedGrids; // List of grid on the ConveyorBelt
    private List<GameObject> prefabQueue;

    // Called when the script instance is loaded
    private void Awake()
    {
        instantiatedGrids = new List<GameObject>();
        prefabQueue = prefabs.ToList();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isRunning)
        {
            MoveGridsRight();

            timer += Time.deltaTime;

            // When timer is complete
            if (timer >= spawnInterval)
            {
                timer = 0f; // Reset timer

                // Randomly choose a prefab from the list
                int randomIndex = Random.Range(0, prefabQueue.Count - 1);
                GameObject prefabToInstantiate = prefabQueue[randomIndex];
                prefabQueue.RemoveAt(randomIndex);

                // Instantiate the grid at the spawn position
                GameObject instantiatedGrid = Instantiate(gridPrefab, transform.position, Quaternion.identity, transform);

                // Instantiate the part at the grid's position and set it as a child
                GameObject instantatedPart = Instantiate(
                    prefabToInstantiate,
                    instantiatedGrid.transform.position, // Match grid's position
                    Quaternion.identity,
                    instantiatedGrid.transform // Parent directly to the grid
                );

                instantiatedGrids.Add(instantiatedGrid);

                if (prefabQueue.Count == 0)
                {
                    prefabQueue = prefabs.ToList();
                }
            }
        }
    }

    public void SetIsRunning(bool val)
    {
        isRunning = val;
    }

    // On each Update(), moves each Grid/Part to the right
    private void MoveGridsRight()
    {
        // Iterates through each grid
        for (int i = 0; i < instantiatedGrids.Count; i++)
        {
            GameObject obj = instantiatedGrids[i];

            // Move the object in the positive X direction relative to its parent's orientation
            Vector3 moveDirection = obj.transform.parent.right;
            obj.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            if (obj.transform.localPosition.x > destroyDistance)
            {
                // If the player's tile highlight is on the grid, moves it before tile destruction
                Transform tileHighlight = GetChildWithName("TileHighlight", obj);
                if (tileHighlight)
                {
                    tileHighlight.SetParent(null);
                    tileHighlight.gameObject.SetActive(false);
                }

                instantiatedGrids.RemoveAt(i);
                Destroy(obj);
                i--;
            }
        }
    }

    // Used to search a grid for a certain prefab
    private Transform GetChildWithName(string name, GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            if (child.name == name)
            {
                return child; // Found
            }
        }

        return null; // Not found
    }

    // Picks up an item from a grid on the conveyor belt
    // Decoupled from GridManager due to not setting parts as "isPlaced"
    public GameObject PickUpItem(Vector2 position)
    {
        // Looks for parts to pick up
        LayerMask partLayerMask = LayerMask.GetMask("Parts");
        Collider2D collider = Physics2D.OverlapPoint(position, partLayerMask);

        // Picks up part if found
        if (collider != null)
        {
            GameObject part = collider.gameObject;

            return part;
        }

        // Null if not found
        else
        {
            return null;
        }
    }

    // Places a part onto the conveyor belt
    // Decoupled from GridManager due to not setting parts as "isPlaced"
    public bool PlaceItem(GameObject item, Vector2 position)
    {
        // Looks for Grid
        LayerMask gridLayerMask = LayerMask.GetMask("Grid");
        Collider2D gridCollider = Physics2D.OverlapPoint(position, gridLayerMask);

        // Looks for Part
        LayerMask partLayerMask = LayerMask.GetMask("Parts");
        Collider2D partCollider = Physics2D.OverlapPoint(position, partLayerMask);

        // If there is a grid and there isnt a part at the position, places player's part
        if (gridCollider != null && partCollider == null)
        {
            GameObject grid = gridCollider.gameObject;

            item.transform.SetParent(grid.transform);
            item.transform.localPosition = Vector3.zero;

            return true; // Success and returns true
        }

        // Otherwise, not a success and returns false
        else
        {
            return false;
        }
    }
}
