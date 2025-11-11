using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Part that is a gear but automatically rotates nearby turrets
public class AutoGearPart : GearPart
{
    private new void Update()
    {
        base.Update();
        RotateAdjacentTurrets();
    }

    // Searching method to see if there are any adjacent gears that are active
    protected void RotateAdjacentTurrets()
    {
        // Keeps track of the powered gears found
        List<TurretPart> foundPoweredParts = new List<TurretPart>();

        if (GetIsPlaced() && IsPowered())
        {
            // Get the local position and convert to world position
            Vector2 localPosition = transform.localPosition;

            // Check adjacent directions: up, down, left, right
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

                // Found
                if (hit != null)
                {
                    TurretPart turret = hit.GetComponent<TurretPart>();

                    // If the object is an TurretPart and the gear is powered
                    if (turret != null)
                    {
                        RotateTowardsClosest(turret.gameObject, "Boss");
                    }
                }
            }
        }
    }

    public static void RotateTowardsClosest(GameObject rotatingObject, string targetTag)
    {
        GameObject closestTarget = FindClosestTarget(rotatingObject.transform.position, targetTag);
        if (closestTarget != null)
        {
            Vector2 direction = (closestTarget.transform.position - rotatingObject.transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            float currentAngle = rotatingObject.transform.rotation.eulerAngles.z;
            float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

            // Determine the closest 15-degree step
            float step = 10f;
            float stepCount = Mathf.Round(angleDifference / step);
            float snappedAngle = currentAngle + stepCount * step;

            rotatingObject.transform.rotation = Quaternion.Euler(0, 0, snappedAngle);
        }
    }

    private static GameObject FindClosestTarget(Vector2 position, string targetTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            float distance = Vector2.Distance(position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = target;
            }
        }
        return closest;
    }
}
