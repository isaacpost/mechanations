using System.Collections;
using UnityEngine;

// Controller for the first boss, "The Spy"
public class EyeBossController : MonoBehaviour, IDamagable
{
    [SerializeField] GameObject projectilePrefab;
    
    // Movement and shooting attributes
    [SerializeField] float leftBound = -5f;
    [SerializeField] float rightBound = 5f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float maxAngle = 30;
    [SerializeField] GridManager gridManager;

    private bool bossAlive = true;
    private Animator animator; // Controls current animations
    private Health health; // Controls health, which controls healthbar display
    private float targetPosition; // Next position to move to
    private float moveTimer; // Keeps track of time since last movement
    private float moveInterval = 5f; // Time interval between random movements

    // Start is called before the first frame update
    void Start()
    {
        moveTimer = moveInterval;

        // Gets attached components
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

        // The main boss logic that gets ran every frame
        StartCoroutine(EyeBossSequence());

    }

    // Update is called once per frame
    private void Update()
    {
        UpdateEyeOpenSpeed();

        // Countdown the move timer
        moveTimer -= Time.deltaTime;

        // If the timer is up, pick a new random target position
        if (moveTimer <= 0f && bossAlive)
        {
            // Randomly choose a new target position within the bounds
            ChangeMovePosition();

            // Reset the timer
            moveTimer = moveInterval;
        }

        // Gradually move the object toward the target position
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, targetPosition, Time.deltaTime * moveSpeed),
            transform.position.y,
            transform.position.z
        );
    }

    // Sets the new position randomly
    private void ChangeMovePosition()
    {
        targetPosition = Random.Range(leftBound, rightBound);
    }

    // Creates a projectile, angle it is shot at depends on remaining health
    void Shoot()
    {
        // Gets scale factor based on health
        float scaleFactor = health.CalculateScaleFactor();

        float leftBound = -1 * maxAngle * scaleFactor;
        float rightBound = maxAngle * scaleFactor;

        float zAngle = Random.Range(leftBound, rightBound);

        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, zAngle));
    }

    // Call this to transition to the "closing" animation and to take damage
    public void TakeDamage(float dmg)
    {
        animator.SetTrigger("Close");

        health.TakeDamage(dmg);

        ChangeMovePosition();
    }

    // Changes the speed at which the eye opens based on current health and number of parts placed
    private void UpdateEyeOpenSpeed()
    {
        float value = gridManager.GetBossRatio() * 0.5f + health.CalculateScaleFactor();

        animator.SetFloat("EyeOpenScaler", value);
    }

    // Calls lifecycleManager when boss dies
    public void Died()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;

        bossAlive = false;
        LifecycleManager.Instance.BossDied();
    }

    // The main movement/shooting sequence for the boss
    private IEnumerator EyeBossSequence()
    {
        while (true)
        {
            // Check the current state in layer 0 (default layer)
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Compare the current state's name
            if (stateInfo.IsName("EyeLooking"))
            {
                Shoot();
                yield return new WaitForSeconds(((1 - health.CalculateScaleFactor())) + 0.25f);
            }
            else
            {
                yield return null;
            }
        }
    }
}
