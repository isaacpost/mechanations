using UnityEngine;

// The projectile after the boss has captured a part and flung it back at the player
public class FinalBossPartProjectile : Projectile
{
    [SerializeField]
    GameObject explosionPrefab; // Explosion prefab to instantiate when part is hit

    private SpriteRenderer spriteRenderer; // Gets sprite of picked up part
    private readonly string startHex = "#7E7E7E"; // Sets color of part sprite

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // The movement method required by Projectile parent class
    public override void Move()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    public void SetSprite(Sprite partSprite)
    {
        spriteRenderer.sprite = partSprite;

        if (partSprite.name == "TurretActiveSprite")
        {
            Color newColor;
            if (ColorUtility.TryParseHtmlString(startHex, out newColor))
            {
                spriteRenderer.color = newColor;
            }
        }

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
                other.gameObject.GetComponent<Part>().TakeDamage(2f);
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
