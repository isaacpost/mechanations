using System.Collections;
using UnityEngine;

// Used on title screen for rotation of the game logo
public class RandomRotation : MonoBehaviour
{
    private readonly float minAngle = -5f;
    private readonly float maxAngle = 5f;
    private readonly float minDuration = 1.5f;
    private readonly float maxDuration = 3f;

    private float currentAngle = 0f;
    private float targetAngle = 0f;
    private float rotationDuration;

    void Start()
    {
        // Initialize the first rotation cycle
        StartCoroutine(RotateRandomlyWithinRange());
    }

    IEnumerator RotateRandomlyWithinRange()
    {
        while (true)
        {
            // Determine a new target angle within the restricted range
            targetAngle = Random.Range(minAngle, maxAngle);
            rotationDuration = Random.Range(minDuration, maxDuration);

            // Store the starting angle
            float startAngle = currentAngle;

            float elapsedTime = 0f;
            while (elapsedTime < rotationDuration)
            {
                currentAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime / rotationDuration);
                transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentAngle = targetAngle;
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }
}
