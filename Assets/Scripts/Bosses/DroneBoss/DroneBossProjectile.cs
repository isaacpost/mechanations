using System;
using UnityEngine;

// Controls logic of the projectile the boss shoots
public class DroneBossProjectile : Projectile
{
    [SerializeField] GameObject explosionPrefab;

    // The movement method required by Projectile parent class
    public override void Move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    // When it enters another collider, checks for type for logic
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Part"))
        {
            if (other.gameObject.GetComponent<Part>().GetIsPlaced())
            {
                if (other.gameObject.GetComponent<WallPart>() != null)
                {
                    Destroy(gameObject);
                    SFXManager.Instance.PlaySound("WallHit");
                }
                else
                {
                    SFXManager.Instance.PlaySound("TurretHit");
                }

                Instantiate(explosionPrefab, gameObject.transform.position, transform.rotation);
                other.gameObject.GetComponent<Part>().TakeDamage(0.5f);
            }
        }

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            // Only deals damage if player is damageable
            if (!player.IsInvinsible())
            {
                player.TakeDamage(1f);
                SFXManager.Instance.PlaySound("PlayerHurt");
                Destroy(gameObject);
            }    
           
        }
    }
}
