using System.Collections.Generic;
using UnityEngine;

// Abstract part class that gives general behavior to parts
public abstract class Part : MonoBehaviour, IDamagable
{
    [SerializeField] bool isPlaced = false;

    protected float checkRadius = 1f; // Adjust the radius based on grid spacing
    protected LayerMask checkLayer; // Defines which objects to check

    private Health health; // Health of part, shown on slider

    protected virtual void Awake()
    {
        checkLayer = LayerMask.GetMask("Parts");
        health = GetComponent<Health>();
    }

    public abstract void Rotate(float degrees);

    // Returns value of isPlaced
    public bool GetIsPlaced()
    {
        return isPlaced;
    }

    // Sets value of isPlaced
    public void SetIsPlaced(bool val)
    {
        isPlaced = val;
    }

    // Searching method to see if there are any adjacent gears that are active
    protected List<IGear> FindAdjacentGears()
    {
        // Keeps track of the powered gears found
        List<IGear> foundPoweredParts = new List<IGear>();

        if (isPlaced)
        {
            // Get the bounds of the object
            Vector2 position = transform.position;

            // Check adjacent directions: up, down, left, right
            Vector2[] directions = {
                new Vector2(0, checkRadius),    // Up
                new Vector2(0, -checkRadius),   // Down
                new Vector2(-checkRadius, 0),   // Left
                new Vector2(checkRadius, 0)     // Right
            };

            // For each direction, check for a collider
            foreach (Vector2 direction in directions)
            {
                // Perform the check for an overlap box in the adjacent direction
                Collider2D hit = Physics2D.OverlapPoint(position + direction, checkLayer);

                // Found
                if (hit != null)
                {
                    IGear gear = hit.GetComponent<IGear>();

                    // If the object is an IGear and the gear is powered
                    if (gear != null && gear.IsPowered())
                    {
                        // Found an adjacent object
                        foundPoweredParts.Add(hit.gameObject.GetComponent<IGear>());
                    }
                }
            }
        }

        // No adjacent objects found
        return foundPoweredParts;
    }

    public void TakeDamage(float dmg)
    {
        health.TurnOnHealthBar();
        health.TakeDamage(dmg);
    }

    public void Died()
    {
        Destroy(gameObject);
    }
}