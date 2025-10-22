using UnityEngine;
using System.Collections;

// Shakes a sprite a certain amount
public class SpriteShaker : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.2f; // Total time of the shake
    [SerializeField] private float shakeMagnitude = 0.1f; // Magnitude of the shake

    private bool isShakeCoroutineRunning = false;

    public void Shake()
    {
        if (!isShakeCoroutineRunning)
        {
            isShakeCoroutineRunning = true;
            StartCoroutine(ShakeCoroutine());
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;
        Vector2 originalPosition = transform.localPosition;

        while (elapsedTime < shakeDuration)
        {
            // Generate a random offset
            Vector2 randomOffset = new Vector2(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude));

            // Apply the offset to the sprite's position
            transform.localPosition = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isShakeCoroutineRunning = false;
    }
}
