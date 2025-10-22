using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Logic to controll damage delt by the final boss's blade
public class FinalBossBlade : MonoBehaviour
{
    [SerializeField] 
    GameObject explosionPrefab; // Prefab instantated when part is hit

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Part"))
        {
            if (other.gameObject.GetComponent<Part>().GetIsPlaced())
            {
                if (other.gameObject.GetComponent<WallPart>() != null)
                {
                    SFXManager.Instance.PlaySound("WallHit");
                }
                else
                {
                    SFXManager.Instance.PlaySound("TurretHit");
                }

                Instantiate(explosionPrefab, gameObject.transform.position, transform.rotation);
                other.gameObject.GetComponent<Part>().TakeDamage(3f);
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
            }

        }
    }
}
