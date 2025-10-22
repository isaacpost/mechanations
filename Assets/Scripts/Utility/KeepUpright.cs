using UnityEngine;

// Keeps the gameobject it is attached to upright
public class KeepUpright : MonoBehaviour
{
    void Update()
    {
        // Keep the object upright by locking its rotation
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
