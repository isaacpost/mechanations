using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The projectile that turrets shoot
public class TurretProjectile : Projectile
{
    // Explosion on destroying of the projectile
    [SerializeField] private GameObject explosionPrefab;
    
    public override void Move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the specific tag
        if (other.CompareTag("Boss"))
        {
            IDamagable bossController = other.GetComponent<IDamagable>();
            bossController.TakeDamage(3f);

            Instantiate(explosionPrefab, transform.position, transform.rotation);
            SFXManager.Instance.PlaySound("TurretHit");
            Destroy(gameObject); // Destroy the projectile
        }

        if (other.CompareTag("Drone"))
        {
            IDamagable droneController = other.GetComponent<IDamagable>();
            droneController.TakeDamage(1.0f);

            Destroy(gameObject);
        }
    }

}
