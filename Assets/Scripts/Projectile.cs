using UnityEngine;

// Abstract class for all projectiles, destroy's self when not visable
public abstract class Projectile : MonoBehaviour
{
    public float speed;
    public abstract void Move();

    void Update()
    {
        Move();
    }

    void OnBecameInvisible()
    {
        // Destroy when the projectile leaves the screen
        Destroy(gameObject);
    }
}