using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The projectile the player's peashooter launches
public class PlayerProjectile : Projectile
{
    // Explosion prefab if collided with boss projectile
    [SerializeField] GameObject explosionPrefab;

    private Camera mainCamera;
    private Vector2 shootDirection; // The direction the projectile was shot in

    private void Awake()
    {
        mainCamera = Camera.main;
        shootDirection = GetShootDirection();

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // Apply the rotation with the pivot at the bottom
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private Vector2 GetShootDirection()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector2 clickPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));

        // Calculate the direction vector
        Vector2 currentPosition = transform.position;
        return (clickPosition - currentPosition).normalized;
    }

    public override void Move()
    {
        // Move the object in the direction of the target point
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BossProjectile"))
        {
            Destroy(other.gameObject);
            SFXManager.Instance.PlaySound("Trash");
        }

        if (other.CompareTag("Boss"))
        {
            IDamagable bossController = other.GetComponent<IDamagable>();
            bossController.TakeDamage(0.5f);

            Instantiate(explosionPrefab, transform.position, transform.rotation);

            SFXManager.Instance.PlaySound("TurretHit");
            Destroy(gameObject); // Destroy the projectile
        }

        if (other.CompareTag("Drone"))
        {
            IDamagable droneController = other.GetComponent<IDamagable>();
            droneController.TakeDamage(1);

            Destroy(gameObject);
        }
    }
}
