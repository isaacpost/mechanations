using UnityEngine;

// Used on explosion object to destroy it after it has played its animation
public class PlayAnimationAndDestroy : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();

        // Case where no animator on object
        if (animator == null)
        {
            Destroy(gameObject);
            return;
        }

        // Play the default animation
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        // Destroy the GameObject after the animation duration
        Destroy(gameObject, animationLength);
    }
}
