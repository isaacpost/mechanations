using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

// Gear part that arrives on conveyor belt
public class GearPart : Part, IDamagable
{
    [SerializeField] Sprite staticSprite; // Sprite for when not powered
    [SerializeField] GameObject spriteObject; // Sprite for when not powered
    [SerializeField] private bool isPowerSource = false; // Animation speed during shutdown

    private SpriteRenderer spriteRenderer; // Sprite component
    private Animator animator; // animator components
    private List<GearPart> adjacentPoweredParts; // Keeps list of adjacent powered gears
    private bool isPowered;
    private bool isShuttingDown; // To prevent multiple coroutines running
    private float shutdownDelay = 3f; // How long to slow before stopping
    private float slowedSpeed = 0.1f; // Animation speed during shutdown
    private Vector3 lastPosition;
    private float threshold = 0.001f;

    // Called when the script instance is loaded
    protected override void Awake()
    {
        base.Awake();

        // Get the components
        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        animator = spriteObject.GetComponent<Animator>();

        adjacentPoweredParts = new List<GearPart>();

        animator.enabled = false;
        spriteRenderer.sprite = staticSprite;

        lastPosition = transform.position;
    }

    // Update is called once per frame
    protected void Update()
    {
        GearPoweredLogic();
        AnimationLogic();
    }

    // Calls shaker animation since cant be rotated
    public override void Rotate(float degrees)
    {
        SFXManager.Instance.PlaySound("Error");
        spriteObject.GetComponent<SpriteShaker>().Shake();
    }

    // Return value of isPowered
    public bool IsPowered()
    {
        return isPowered;
    }

    // Stops being powered before its destroyed to alert other gears
    private void OnDestroy()
    {
        isPowered = false;
    }

    // Called once per frame to see if the gear is powered or not
    private void GearPoweredLogic()
    {
        if (isPowerSource)
        {
            isPowered = PowerManager.Instance.IsNotEmpty();
            return;
        }

        // If no adjacent powered parts, search for them
        if (adjacentPoweredParts.Count == 0)
        {
            adjacentPoweredParts = FindAdjacentGears();

            // If found adjacent powered parts, set current gear to powered and aniamte
            if (adjacentPoweredParts.Count > 0)
            {
                isPowered = true;
                animator.enabled = true;
            }
        }

        // If any of the surrounding parts aren't powered anymore, reset gear
        // Or if the gear was picked up, reset gear
        else if (adjacentPoweredParts.Any(obj => !obj.IsPowered()) || !GetIsPlaced() || Vector3.Distance(transform.position, lastPosition) > threshold)
        {
            isPowered = false;
            adjacentPoweredParts = new List<GearPart>();
            lastPosition = transform.position;
        }
    }

    private void AnimationLogic()
    {
        if (IsPowered())
        {
            // Cancel any shutdown process if we get powered again
            if (isShuttingDown)
            {
                StopAllCoroutines();
                isShuttingDown = false;
                animator.speed = 1f;
            }

            animator.enabled = true;
        }
        else if (!isShuttingDown && animator.enabled)
        {
            // Begin shutdown process
            StartCoroutine(SlowDownBeforeStop());
        }
    }

    private IEnumerator SlowDownBeforeStop()
    {
        isShuttingDown = true;

        // Gradually slow down
        float originalSpeed = animator.speed;
        float elapsed = 0f;
        while (elapsed < shutdownDelay)
        {
            animator.speed = Mathf.Lerp(originalSpeed, slowedSpeed, elapsed / shutdownDelay);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Finalize shutdown
        animator.speed = 1f; // Reset speed for next activation
        animator.enabled = false;
        spriteRenderer.sprite = staticSprite;

        isShuttingDown = false;
    }
}
