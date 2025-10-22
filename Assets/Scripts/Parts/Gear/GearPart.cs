using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// Gear part that arrives on conveyor belt
public class GearPart : Part, IGear, IDamagable
{
    [SerializeField] Sprite staticSprite; // Sprite for when not powered
    [SerializeField] GameObject spriteObject; // Sprite for when not powered

    private SpriteRenderer spriteRenderer; // Sprite component
    private Animator animator; // animator components
    private List<IGear> adjacentPoweredParts; // Keeps list of adjacent powered gears
    protected bool isPowered;

    private Vector3 lastPosition;
    private float threshold = 0.001f;

    // Called when the script instance is loaded
    protected override void Awake()
    {
        base.Awake();

        // Get the components
        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        animator = spriteObject.GetComponent<Animator>();

        adjacentPoweredParts = new List<IGear>();

        animator.enabled = false;
        spriteRenderer.sprite = staticSprite;

        lastPosition = transform.position;
    }

    // Update is called once per frame
    protected void Update()
    {
        GearPoweredLogic();
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
            adjacentPoweredParts = new List<IGear>();
            animator.enabled = false;
            spriteRenderer.sprite = staticSprite;
            lastPosition = transform.position;
        }
    }
}
