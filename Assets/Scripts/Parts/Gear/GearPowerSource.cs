using UnityEngine;
using System.Collections;

// Logic for a gear that is always powered and not a part that can be picked up
public class GearPowerSource : MonoBehaviour, IGear
{
    [SerializeField] private Sprite staticSprite; // Sprite for when not powered
    [SerializeField] private float shutdownDelay = 1f; // How long to slow before stopping
    [SerializeField] private float slowedSpeed = 0.3f; // Animation speed during shutdown

    private SpriteRenderer spriteRenderer; // Sprite component
    private Animator animator; // Animator component
    private bool isShuttingDown; // To prevent multiple coroutines running

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        animator.enabled = false;
        spriteRenderer.sprite = staticSprite;
    }

    void Update()
    {
        PowerSourcePoweredLogic();
    }

    public bool IsPowered()
    {
        return PowerManager.Instance.IsNotEmpty();
    }

    private void PowerSourcePoweredLogic()
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
