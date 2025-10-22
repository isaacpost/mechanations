using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerProjectile : Projectile
{
    [SerializeField] private GameObject explosionPrefab;

    private Camera mainCamera;
    private Vector2 shootDirection;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Cache shoot direction at spawn
        shootDirection = GetShootDirection();

        // Rotate the projectile to face the shoot direction
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        // If your sprite faces "up" by default in the sprite editor, subtract 90; if it faces "right", use angle directly.
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // Debug
        Debug.DrawRay(transform.position, (Vector3)shootDirection * 3f, Color.cyan, 1.5f);
        // Debug.Log($"Shoot dir: {shootDirection}, angle: {angle}");
    }

    private Vector2 GetShootDirection()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        // IMPORTANT: Use the correct depth (distance from camera to projectile’s plane)
        float depthForUnproject = 0f;
        if (!mainCamera.orthographic)
        {
            // Distance along camera forward to the projectile’s Z-plane
            depthForUnproject = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        }

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, depthForUnproject));
        Vector2 dir = ((Vector2)mouseWorld - (Vector2)transform.position);

        if (dir.sqrMagnitude < 0.000001f)
            return transform.up; // fallback if somehow on top of cursor

        return dir.normalized;
    }

    public override void Move()
    {
        // Simple, non-physics movement along the cached direction
        transform.position += (Vector3)(shootDirection * speed * Time.deltaTime);
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
            if (bossController != null)
                bossController.TakeDamage(0.5f);

            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, transform.rotation, transform.parent.parent);

            SFXManager.Instance.PlaySound("TurretHit");
            Destroy(gameObject);
        }

        if (other.CompareTag("Drone"))
        {
            IDamagable droneController = other.GetComponent<IDamagable>();
            if (droneController != null)
                droneController.TakeDamage(1f);

            Destroy(gameObject);
        }
    }
}
