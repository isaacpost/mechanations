using UnityEngine;
using System.Collections;

// Rotates an attached object over a number of seconds
public class RotateOverTime : MonoBehaviour
{
    public IEnumerator RotateObject(float newZRotation, float duration)
    {
        float startZRotation = transform.eulerAngles.z;
        float endZRotation = startZRotation + newZRotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float zRotation = Mathf.Lerp(startZRotation, endZRotation, elapsedTime / duration);
            transform.rotation = Quaternion.Euler(0, 0, zRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, endZRotation);
    }
}
