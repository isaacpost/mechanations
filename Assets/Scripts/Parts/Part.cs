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
    protected List<GearPart> FindAdjacentGears()
    {
        // Keeps track of the powered gears found
        List<GearPart> foundPoweredParts = new List<GearPart>();
        
        // Get the local position and convert to world position
        Vector2 localPosition = transform.localPosition;

        // Adjacent directions in local space
        Vector2[] directions = {
            new Vector2(0, checkRadius),    // Up
            new Vector2(0, -checkRadius),   // Down
            new Vector2(-checkRadius, 0),   // Left
            new Vector2(checkRadius, 0)     // Right
        };

        foreach (Vector2 direction in directions)
        {
            // Convert the local offset into world space before doing the overlap check
            Vector3 worldPoint = transform.parent.TransformPoint(localPosition + direction);

            // Perform overlap in world space
            Collider2D hit = Physics2D.OverlapPoint(worldPoint, checkLayer);

            if (hit != null)
            {
                GearPart gear = hit.GetComponent<GearPart>();

                if (gear != null && gear.IsPowered())
                {
                    foundPoweredParts.Add(gear);
                }
            }
        }
        

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