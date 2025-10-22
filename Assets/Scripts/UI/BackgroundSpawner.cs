using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to create the background of the title screen
public class BackgroundSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Sprite> sprites;       // List of sprites to choose from
    [SerializeField] private float spawnInterval = 2f;   // Time between spawns in seconds
    [SerializeField] private int spriteCount = 10;       // Number of sprites to spawn in a line
    [SerializeField] private float spacing = 1f;         // Spacing between sprites in the line
    [SerializeField] private float moveSpeed = 1f;       // Speed at which the sprites move down
    [SerializeField] private float destroyYPosition = -6f; // Y position at which sprites are destroyed
    [SerializeField] private Transform parent;           // Parent of the spawned sprites (optional)

    [Header("Parenting Options")]
    [Tooltip("If true, SetParent(..., false) so the child adopts the parent's scale/rotation/position in local space.")]
    [SerializeField] private bool inheritParentTransform = true;

    [Tooltip("If set, children will be placed using the parent's local space X (instead of world X).")]
    [SerializeField] private bool spawnInParentLocalSpace = false;

    private void Start()
    {
        StartCoroutine(SpawnSpritesInLine());
    }

    private IEnumerator SpawnSpritesInLine()
    {
        while (true)
        {
            SpawnLine();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnLine()
    {
        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("[BackgroundSpawner] No sprites available to spawn.");
            return;
        }

        // Determine the base position (either in world or relative to parent)
        Vector3 origin = transform.position;

        // Compute startX in the chosen space
        float totalWidth = (spriteCount - 1) * spacing;
        float startX = -totalWidth * 0.5f; // centered around zero; we'll offset later

        for (int i = 0; i < spriteCount; i++)
        {
            float offsetX = startX + i * spacing;

            Vector3 spawnPos;
            if (spawnInParentLocalSpace && parent != null)
            {
                // Build a local position under the parent and convert to world after parenting
                // We'll set localPosition after parenting below.
                spawnPos = new Vector3(offsetX, 0f, 0f); // local X spread; Y/Z handled after parenting
            }
            else
            {
                // Pure world-space layout
                spawnPos = new Vector3(origin.x + offsetX, origin.y, origin.z);
            }

            SpawnSpriteAtPosition(spawnPos);
        }
    }

    private void SpawnSpriteAtPosition(Vector3 positionOrLocal)
    {
        // Choose a random sprite from the list
        Sprite selectedSprite = sprites[Random.Range(0, sprites.Count)];
        GameObject spriteObject = new GameObject("BackgroundSprite");

        // Add a SpriteRenderer and assign the selected sprite
        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = selectedSprite;
        spriteRenderer.sortingOrder = 0; // Ensure the sprite is behind everything else

        // Parent first, so local transform is correct relative to parent scaling
        if (parent != null)
        {
            // IMPORTANT: worldPositionStays = false makes the child adopt the parent's scale/rotation.
            spriteObject.transform.SetParent(parent, worldPositionStays: !inheritParentTransform);

            if (inheritParentTransform)
            {
                // We want the child to inherit parent scale:
                // With SetParent(..., false), localScale stays (1,1,1) so worldScale = parentScale * 1.
                // Now decide whether the provided position is local or world:
                if (spawnInParentLocalSpace)
                {
                    // Place using local position (spread along X under parent)
                    Vector3 local = positionOrLocal;
                    // Keep Y/Z aligned with spawner's Y/Z under parent's space
                    // Convert the spawner's world Y/Z to parent's local to keep rows aligned
                    Vector3 parentLocalOfSpawner = parent.InverseTransformPoint(transform.position);
                    local.y = parentLocalOfSpawner.y;
                    spriteObject.transform.localPosition = local;
                }
                else
                {
                    // Place using world position (convert to local after parenting)
                    spriteObject.transform.position = positionOrLocal; // Unity handles local from world
                }

                // Ensure no skew from unexpected local scale
                spriteObject.transform.localScale = Vector3.one;
                spriteObject.transform.localRotation = Quaternion.identity;
                spriteObject.transform.localRotation = Quaternion.Euler(-45f, 0f, 0f);
            }
            else
            {
                // Keeping world transform (default Unity behavior)
                spriteObject.transform.position = positionOrLocal;
                // localScale will be adjusted to counteract parent's scale (appears unchanged in world)
            }
        }
        else
        {
            // No parent assigned, just use world position
            spriteObject.transform.position = positionOrLocal;
            spriteObject.transform.localScale = Vector3.one;
        }

        // Attach a mover script to the sprite
        SpriteMover mover = spriteObject.AddComponent<SpriteMover>();
        mover.moveSpeed = moveSpeed;
        mover.destroyYPosition = destroyYPosition;
    }
}

public class SpriteMover : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float destroyYPosition = -6f;

    private void Update()
    {
        // Move the sprite down
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);

        // Destroy the sprite if it moves past the destroy position
        if (transform.position.y <= destroyYPosition)
        {
            Destroy(gameObject);
        }
    }
}
