using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class GearPart : Part, IDamagable
{
    [Header("Visuals")]
    [SerializeField] private GameObject spriteObject;
    [SerializeField] private bool isPowerSource = false;
    [SerializeField] private bool isPowerStore = false;

    [Header("Shutdown Settings")]
    [SerializeField] private float shutdownDelay = 3f; // Time to fade and slow
    [SerializeField] private float slowedSpeed = 0.1f; // Animation speed during fade
    [SerializeField, Range(0f, 1f)] private float dimmedBrightness = 0.4f; // Brightness when unpowered
    [SerializeField] private float brightenSpeed = 3f; // How fast it brightens when powered

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private List<GearPart> adjacentPoweredParts;
    private bool isPowered;
    private bool isShuttingDown;
    private Vector3 lastPosition;
    private float threshold = 0.001f;
    private Color fullColor;
    private Color dimColor;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        animator = spriteObject.GetComponent<Animator>();
        adjacentPoweredParts = new List<GearPart>();

        fullColor = Color.white;
        dimColor = new Color(fullColor.r * dimmedBrightness, fullColor.g * dimmedBrightness, fullColor.b * dimmedBrightness, fullColor.a);

        // Start dimmed and animation off
        spriteRenderer.color = dimColor;
        animator.enabled = false;

        lastPosition = transform.position;
    }

    protected void Update()
    {
        GearPoweredLogic();
        AnimationLogic();
    }

    public override void Rotate(float degrees)
    {
        SFXManager.Instance.PlaySound("Error");
        spriteObject.GetComponent<SpriteShaker>().Shake();
    }

    public bool IsPowered() => isPowered;

    private void OnDestroy()
    {
        isPowered = false;
    }

    private void GearPoweredLogic()
    {
        if (isPowerSource)
        {
            isPowered = PowerManager.Instance.IsNotEmpty();
            return;
        }

        if (adjacentPoweredParts.Count == 0 && (GetIsPlaced() || isPowerStore))
        {
            adjacentPoweredParts = FindAdjacentGears();
            if (adjacentPoweredParts.Count > 0)
            {
                isPowered = true;
                animator.enabled = true;
            }
        }
        else if (adjacentPoweredParts.Any(obj => !obj.IsPowered())
                 || !GetIsPlaced()
                 || Vector3.Distance(transform.position, lastPosition) > threshold)
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
            if (isShuttingDown)
            {
                StopAllCoroutines();
                isShuttingDown = false;
                animator.speed = 1f;
            }

            animator.enabled = true;
            // Smoothly brighten back to full color
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, fullColor, Time.deltaTime * brightenSpeed);
        }
        else if (!isShuttingDown && animator.enabled)
        {
            StartCoroutine(SlowDownAndDim());
        }
    }

    private IEnumerator SlowDownAndDim()
    {
        isShuttingDown = true;

        float originalSpeed = animator.speed;
        float elapsed = 0f;
        Color startColor = spriteRenderer.color;

        while (elapsed < shutdownDelay)
        {
            float t = elapsed / shutdownDelay;
            animator.speed = Mathf.Lerp(originalSpeed, slowedSpeed, t);
            spriteRenderer.color = Color.Lerp(startColor, dimColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        animator.speed = 1f;
        animator.enabled = false;
        spriteRenderer.color = dimColor;

        isShuttingDown = false;
    }
}
