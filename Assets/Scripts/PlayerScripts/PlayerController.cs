using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Main controller for player. Holds attributes, sets health, and performs actions based on input.
public class PlayerController : MonoBehaviour, IDamagable
{
    [SerializeField] private float moveSpeed = 5f; // Speed the player can move around
    [SerializeField] private BoxCollider2D boundaryCollider; // Collider of play area
    [SerializeField] private BoxCollider2D innerBoundaryCollider; // OPTIONAL: inner collider of play area
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject heldItem; // The part in the player's hands
    [SerializeField] private float interactRange = 2f; // Distance the player can grab parts from
    [SerializeField] private GameObject highlightBorder; // The highlight on the grid
    [SerializeField] private GameObject projectilePrefab; // Projectile the pea shooter shoots
    [SerializeField] private Sprite staticSprite; // Static sprite of player to set when not moving
    [SerializeField] private AmmoDisplay ammoDisplay; // The text on the ui display of the player's ammo
    [SerializeField] private float flashDuration = 0.1f; // Time the sprite is visible or invisible.
    [SerializeField] private int flashCount = 5; // Number of flashes.

    private bool invincibilityActive = false;
    private Health health; // Players health
    private int currentAmmo = 3;
    private readonly int maxAmmo = 3;
    private readonly float partRotateAmt = 30f; // The amount to rotate parts by
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 movementInput; // Current movement input from player
    private Rigidbody2D rb;
    private BoxCollider2D playerCollider; // Collider of player object
    private Vector2 itemOffset = new(0f, 0.5f); // moves item up into player's hand
    private LayerMask playerIgnoreMask; // The mask the player should ignore

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

        playerIgnoreMask = ~LayerMask.GetMask("Player");

        if (boundaryCollider == null)
        {
            Debug.LogError("Boundary Collider is not assigned!");
        }

        if (rb.bodyType != RigidbodyType2D.Kinematic)
        {
            Debug.LogWarning("Rigidbody2D should be set to Kinematic for this script to work properly.");
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
            Vector2 halfSize = playerCollider.bounds.extents;

            // Clamp the position within the outer boundary
            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(newPosition.x, boundaryCollider.bounds.min.x + halfSize.x, boundaryCollider.bounds.max.x - halfSize.x),
                Mathf.Clamp(newPosition.y, boundaryCollider.bounds.min.y + halfSize.y, boundaryCollider.bounds.max.y - halfSize.y)
            );

            // Check if the new position would be inside the inner boundary
            if (innerBoundaryCollider != null && innerBoundaryCollider.bounds.Contains(clampedPosition))
            {
                // Calculate movement direction
                Vector2 movementDirection = (clampedPosition - rb.position).normalized;

                // Try stepping back along the movement direction until outside the inner collider
                while (innerBoundaryCollider.bounds.Contains(clampedPosition))
                {
                    clampedPosition -= movementDirection * 0.01f; // Small step backward
                }
            }

            rb.MovePosition(clampedPosition);
        }

        HighlightHoverWithinRange();
    }


    private void HighlightHoverWithinRange()
    {
        GameObject obj = GetTileAtPosition(GetMousePosition());

        if (obj != null) {
            if (IsTileWithinRange(obj)&& IsTileWithinRange(gameObject) && obj != heldItem) {
                highlightBorder.SetActive(true);
                highlightBorder.transform.SetParent(obj.transform);
                highlightBorder.transform.localPosition = Vector2.zero;
            } else
            {
                highlightBorder.SetActive(false);
            }
        }
    }

    private void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();

        if (movementInput.magnitude == 0)
        {
            if (animator != null)
            {
                animator.enabled = false;
                spriteRenderer.sprite = staticSprite;
            }
        }
        else
        {
            animator.enabled = true;
        }
    }

    public void OnLeftClick()
    {
        Vector2 clickPosition = GetMousePosition();
        IPlaceableSurface surface = GetSurfaceAtPosition(clickPosition);

        // Pick Up Part
        if (surface != null && heldItem == null && IsTileWithinRange(GetTileAtPosition(clickPosition)))
        {
            GameObject pickedUpPart = surface.PickUpItem(clickPosition);

            if (pickedUpPart)
            {
                if (pickedUpPart.GetComponent<AmmoRefillPart>() != null)
                {
                    SFXManager.Instance.PlaySound("PickUpItem");

                    ammoDisplay.ResetAmmo();

                    currentAmmo = maxAmmo;

                    Destroy(pickedUpPart);
                }
                else
                {
                    SFXManager.Instance.PlaySound("PickUpPart");

                    highlightBorder.SetActive(false);

                    heldItem = pickedUpPart;
                    heldItem.GetComponent<BoxCollider2D>().enabled = false;
                    pickedUpPart.transform.SetParent(transform);
                    pickedUpPart.transform.localPosition = itemOffset;
                }
            }
        }

        // Place Part
        else if (surface != null && heldItem != null && IsTileWithinRange(GetTileAtPosition(clickPosition)))
        {
            if (surface.PlaceItem(heldItem, clickPosition))
            {
                SFXManager.Instance.PlaySound("PlacePart");

                heldItem.GetComponent<BoxCollider2D>().enabled = true;
                heldItem = null;
            }
            else
            {
                SFXManager.Instance.PlaySound("Error");
                heldItem.GetComponent<SpriteShaker>().Shake();
            }
        }
    }

    public void OnRightClick()
    {
        if (currentAmmo > 0)
        {
            ammoDisplay.DecreaseAmmo(1);
            currentAmmo--;
            SFXManager.Instance.PlaySound("TurretShoot");
            Shoot();
        }
        else
        {
            ammoDisplay.GetComponent<SpriteShaker>().Shake();
            SFXManager.Instance.PlaySound("Empty");
        }
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, transform.position, transform.rotation);
    }

    public void OnE()
    {
        RotatePartAtHighlight(partRotateAmt * -1f);
    }

    public void OnQ()
    {
        RotatePartAtHighlight(partRotateAmt);
    }

    public void OnP()
    {
        MenuManager.Instance.TogglePause();
    }

    private void RotatePartAtHighlight(float degrees)
    {
        Transform tileTransform = highlightBorder.transform.parent;

        if (tileTransform != null)
        {
            // Loop through all child objects of the parent
            foreach (Transform child in tileTransform)
            {
                Part part = child.GetComponent<Part>();

                // Check if the child object has the desired tag
                if (part)
                {
                    part.Rotate(degrees);
                    return;  // Exit once a match is found
                }
            }
        }
    }

    private Vector2 GetMousePosition()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        return mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
    }

    private IPlaceableSurface GetSurfaceAtPosition(Vector2 position)
    {

        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, playerIgnoreMask);

        if (hit)
        {
            return hit.collider.GetComponentInParent<IPlaceableSurface>();

        }
        return null;
    }

    private GameObject GetTileAtPosition(Vector2 position)
    {
        LayerMask tileLayers = LayerMask.GetMask("Grid");

        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, tileLayers);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    private bool IsTileWithinRange(GameObject tile)
    {
        if (tile != null)
        {
            Vector2 thisCenter = GetComponent<SpriteRenderer>().bounds.center;
            Vector2 targetCenter = tile.GetComponent<SpriteRenderer>().bounds.center;

            float distance = Vector2.Distance(thisCenter, targetCenter);

            return distance <= interactRange;
        }

        return false;
    }

    public void Died()
    {
        LifecycleManager.Instance.PlayerDied();
    }

    public void TakeDamage(float dmg)
    {
        StartCoroutine(FlashCoroutine());

        health.TakeDamage(dmg);
    }

    public bool IsInvinsible()
    {
        return invincibilityActive;
    }

    private IEnumerator FlashCoroutine()
    {
        invincibilityActive = true;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle visibility.
            yield return new WaitForSeconds(flashDuration);
        }

        spriteRenderer.enabled = true; // Ensure the sprite is visible at the end.

        invincibilityActive = false;
    }
}
