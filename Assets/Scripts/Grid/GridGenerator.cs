using UnityEngine;

// Instantates a grid based on given attributes
public class GridGenerator : MonoBehaviour
{
    [SerializeField] int rows = 5;
    [SerializeField] int columns = 5;

    [SerializeField] float cellSize = 1.0f;

    [SerializeField] GameObject gridCellPrefab; // Prefab of each tile in the grid

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // The main generation logic
    void GenerateGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                Instantiate(gridCellPrefab, position + transform.position, Quaternion.identity, transform);
            }
        }
    }
}
