using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour, IDamagable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Boundaries")]
    [SerializeField] private BoxCollider2D boundaryCollider;
    [SerializeField] private BoxCollider2D innerBoundaryCollider;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject heldItem;
    [SerializeField] private GameObject highlightBorder;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Sprite staticSprite;
    [SerializeField] private AmmoDisplay ammoDisplay;
    [SerializeField] private GameObject gearPrefab;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private float partRotateAmt = 30f;

    [Header("Damage Flash")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 5;

    [Header("Range Indicator")]
    [SerializeField] private LineRenderer rangeIndicator;
    [SerializeField] private int circleSegments = 60;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private bool showRangeIndicator = true;

    public Vector2 MovementInput => movementInput;

    private bool invincibilityActive = false;
    private Health health;
    private int currentAmmo = 3;
    private readonly int maxAmmo = 3;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;
    private Vector2 itemOffset = new(0f, 0.5f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();

        if (rb.bodyType != RigidbodyType2D.Kinematic)
            rb.bodyType = RigidbodyType2D.Kinematic;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (rangeIndicator != null)
        {
            if (circleSegments < 3) circleSegments = 3;
            rangeIndicator.loop = true;
            rangeIndicator.positionCount = circleSegments;
            rangeIndicator.startWidth = lineWidth;
            rangeIndicator.endWidth = lineWidth;
            rangeIndicator.useWorldSpace = true;
            rangeIndicator.enabled = showRangeIndicator;
        }
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
            Vector2 halfSize = playerCollider.bounds.extents;

            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(newPosition.x, boundaryCollider.bounds.min.x + halfSize.x, boundaryCollider.bounds.max.x - halfSize.x),
                Mathf.Clamp(newPosition.y, boundaryCollider.bounds.min.y + halfSize.y, boundaryCollider.bounds.max.y - halfSize.y)
            );

            if (innerBoundaryCollider != null && innerBoundaryCollider.bounds.Contains(clampedPosition))
            {
                Vector2 movementDirection = (clampedPosition - rb.position).normalized;
                while (innerBoundaryCollider.bounds.Contains(clampedPosition))
                    clampedPosition -= movementDirection * 0.01f;
            }

            rb.MovePosition(clampedPosition);
        }

        HighlightHoverWithinRange();
    }

    private void LateUpdate()
    {
        UpdateRangeIndicator();
    }

    private void HighlightHoverWithinRange()
    {
        GameObject obj = GetTileAtPosition(GetMousePosition());

        if (obj != null)
        {
            if (IsTileWithinRange(obj))
            {
                highlightBorder.SetActive(true);
                highlightBorder.transform.SetParent(obj.transform);
                highlightBorder.transform.localPosition = Vector2.zero;
            }
            else highlightBorder.SetActive(false);
        }
        else highlightBorder.SetActive(false);
    }

    private void OnEsc()
    {
        MenuManager.Instance.TogglePause();
    }

    private void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();

        if (movementInput.magnitude == 0)
        {
            animator.enabled = false;
            spriteRenderer.sprite = staticSprite;
        }
        else animator.enabled = true;
    }

    public void OnLeftClick()
    {
        Vector2 clickPosition = GetMousePosition();
        GameObject tileObj = GetTileAtPosition(clickPosition);
        if (tileObj == null) return;

        IPlaceableSurface surface = tileObj.GetComponentInParent<IPlaceableSurface>();
        if (surface == null) return;

        if (!IsTileWithinRange(tileObj)) return;

        if (heldItem == null)
        {
            TryPickupFromSurface(surface, clickPosition);
        }
        else
        {
            TryPlaceOnSurface(surface, clickPosition);
        }
    }

    private void TryPickupFromSurface(IPlaceableSurface surface, Vector2 clickPosition)
    {
        GameObject pickedUpPart = surface.PickUpItem(clickPosition);
        if (pickedUpPart == null) return;

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
            SetHeldItem(pickedUpPart);
        }
    }

    private void TryPlaceOnSurface(IPlaceableSurface surface, Vector2 clickPosition)
    {
        if (heldItem == null) return;

        if (surface.PlaceItem(heldItem, clickPosition))
        {
            SFXManager.Instance.PlaySound("PlacePart");
            EnableHeldItemCollider();
            heldItem = null;
        }
        else
        {
            SFXManager.Instance.PlaySound("Error");
            heldItem.GetComponentInChildren<SpriteShaker>()?.Shake();
        }
    }

    private void SetHeldItem(GameObject item)
    {
        if (item == null) return;

        heldItem = item;
        heldItem.transform.SetParent(transform);
        heldItem.transform.localPosition = itemOffset;

        DisableHeldItemCollider();
    }

    private void DisableHeldItemCollider()
    {
        if (heldItem == null) return;

        BoxCollider2D col = heldItem.GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = false;
    }

    private void EnableHeldItemCollider()
    {
        if (heldItem == null) return;

        BoxCollider2D col = heldItem.GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = true;
    }

    public void OnRightClick() { }

    public void OnSpace()
    {
        if (heldItem == null)
        {
            GameObject instantiatedPart = Instantiate(
                gearPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            SetHeldItem(instantiatedPart);
        }
    }

    public void OnE() => RotatePartAtHighlight(partRotateAmt * -1f);
    public void OnQ() => RotatePartAtHighlight(partRotateAmt);
    public void OnP() => MenuManager.Instance.TogglePause();

    private void RotatePartAtHighlight(float degrees)
    {
        Transform tileTransform = highlightBorder.transform.parent;
        if (tileTransform == null) return;

        foreach (Transform child in tileTransform)
        {
            Part part = child.GetComponent<Part>();
            if (part)
            {
                part.Rotate(degrees);
                return;
            }
        }
    }

    private Vector2 GetMousePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            return new Vector2(worldPoint.x, worldPoint.y);
        }

        return Vector2.zero;
    }

    private GameObject GetTileAtPosition(Vector2 position)
    {
        LayerMask tileLayers = LayerMask.GetMask("Grid");
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, tileLayers);
        return hit.collider ? hit.collider.gameObject : null;
    }

    private bool IsTileWithinRange(GameObject tile)
    {
        if (tile == null) return false;

        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();
        SpriteRenderer tileSR = tile.GetComponent<SpriteRenderer>();

        if (playerSR == null || tileSR == null)
        {
            float fallbackDistance = Vector2.Distance(transform.position, tile.transform.position);
            return fallbackDistance <= interactRange;
        }

        Bounds a = playerSR.bounds;
        Bounds b = tileSR.bounds;

        float dx = 0f;
        if (a.max.x < b.min.x)        dx = b.min.x - a.max.x;
        else if (b.max.x < a.min.x)   dx = a.min.x - b.max.x;

        float dy = 0f;
        if (a.max.y < b.min.y)        dy = b.min.y - a.max.y;
        else if (b.max.y < a.min.y)   dy = a.min.y - b.max.y;

        float edgeDistance = Mathf.Sqrt(dx * dx + dy * dy);
        return edgeDistance <= interactRange;
    }

    public void Died() => LifecycleManager.Instance.PlayerDied();

    public void TakeDamage(float dmg)
    {
        StartCoroutine(FlashCoroutine());
        health.TakeDamage(dmg);
    }

    public bool IsInvinsible() => invincibilityActive;

    private IEnumerator FlashCoroutine()
    {
        invincibilityActive = true;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashDuration);
        }

        spriteRenderer.enabled = true;
        invincibilityActive = false;
    }

    private void UpdateRangeIndicator()
    {
        if (rangeIndicator == null) return;

        if (!showRangeIndicator)
        {
            rangeIndicator.enabled = false;
            return;
        }

        if (!rangeIndicator.enabled)
            rangeIndicator.enabled = true;

        float angleStep = 360f / circleSegments;
        Vector3 center = transform.position;
        float z = center.z;

        for (int i = 0; i < circleSegments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            float x = center.x + Mathf.Cos(angle) * interactRange;
            float y = center.y + Mathf.Sin(angle) * interactRange;
            rangeIndicator.SetPosition(i, new Vector3(x, y, z));
        }
    }
}
