using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to create the background of the title screen
public class BackgroundSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Sprite> sprites; // List of sprites to choose from
    [SerializeField] private float spawnInterval = 2f; // Time between spawns in seconds
    [SerializeField] private int spriteCount = 10; // Number of sprites to spawn in a line
    [SerializeField] private float spacing = 1f; // Spacing between sprites in the line
    [SerializeField] private float moveSpeed = 1f; // Speed at which the sprites move down
    [SerializeField] private float destroyYPosition = -6f; // Y position at which sprites are destroyed

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
            Debug.LogWarning("No sprites available to spawn.");
            return;
        }

        float startX = transform.position.x - ((spriteCount - 1) * spacing) / 2;
        float spawnY = transform.position.y;

        for (int i = 0; i < spriteCount; i++)
        {
            Vector3 spawnPosition = new Vector3(startX + i * spacing, spawnY, 0);
            SpawnSpriteAtPosition(spawnPosition);
        }
    }

    private void SpawnSpriteAtPosition(Vector3 position)
    {
        // Choose a random sprite from the list
        Sprite selectedSprite = sprites[Random.Range(0, sprites.Count)];
        GameObject spriteObject = new GameObject("BackgroundSprite");
        spriteObject.transform.position = position;

        // Add a SpriteRenderer and assign the selected sprite
        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = selectedSprite;
        spriteRenderer.sortingOrder = -10; // Ensure the sprite is behind everything else

        // Attach a mover script to the sprite
        SpriteMover mover = spriteObject.AddComponent<SpriteMover>();
        mover.moveSpeed = moveSpeed;
        mover.destroyYPosition = destroyYPosition;
    }
}

public class SpriteMover : MonoBehaviour
{
    public float moveSpeed;
    public float destroyYPosition;

    private void Update()
    {
        // Move the sprite down
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // Destroy the sprite if it moves past the destroy position
        if (transform.position.y <= destroyYPosition)
        {
            Destroy(gameObject);
        }
    }
}
