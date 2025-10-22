using System.Collections;
using UnityEditor;
using UnityEngine;

// Inherits methods from LifecycleManager to manage boss lifecycle
public class EyeBossLifecycleManager : LifecycleManager
{
    [SerializeField] GameObject eyeBoss; // The boss gameobject
    [SerializeField] GameObject introCanvas; // The canvas that is shown at the beginning
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject conveyorBeltManager;

    private float bossMovementLength = 0.5f; // How long it takes the boss to move to the starting pos
    private Vector2 finalBossPosition = new Vector2(0, 35f); // Final position of the boss
    private Animator bossAnimator; // Controls current animations

    protected override void Awake()
    {
        base.Awake();

        bossAnimator = eyeBoss.GetComponent<Animator>();

        SFXManager.Instance.LoadMusic("BossOneMusic");
        SFXManager.Instance.PauseMusic();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override IEnumerator IntroSequence()
    {
        // Waits for the eye to finish open animation
        while (!bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("EyeLookingIntro"))
        {
            yield return null; // Wait for the next frame
        }

        // Writes text to screen
        yield return StartCoroutine(TypeText("THE SPY"));

        yield return new WaitForSeconds(1f);

        // Moves the boss to the starting position
        yield return StartCoroutine(MoveToPosition(eyeBoss.transform, finalBossPosition, bossMovementLength));

        // triggers starting animations
        bossAnimator.SetTrigger("TextFinished");

        conveyorBeltManager.GetComponent<ConveyorBeltManager>().SetIsRunning(true);
        EnableGameObjects();
        introCanvas.SetActive(false);
        bossAnimator.SetTrigger("StartGame");
    }

    // The corroutine that runs when the boss dies
    protected override IEnumerator DeathSequence()
    {
        conveyorBeltManager.GetComponent<ConveyorBeltManager>().SetIsRunning(false);

        bossAnimator.SetTrigger("BossDefeated");

        yield return new WaitForSeconds(3f);

        bossAnimator.SetTrigger("BackToCenter");

        yield return new WaitForSeconds(1f);

        GameObject explosion = Instantiate(explosionPrefab, eyeBoss.transform.position, eyeBoss.transform.rotation);
        explosion.transform.localScale = new Vector3(2f, 2f, 2f);
        SFXManager.Instance.PlaySound("TurretHit");

        Destroy(eyeBoss);

        yield return new WaitForSeconds(1.5f);

        SFXManager.Instance.PlaySound("BossDefeat");

        Time.timeScale = 0f;

        MenuManager.Instance.OpenMenu("YouWinMenuCanvas");

        GameManager.Instance.DefeatBoss(1);
    }

    private IEnumerator MoveToPosition(Transform obj, Vector2 target, float duration)
    {
        Vector3 startPosition = obj.position;
        float startZ = obj.position.z; // ✅ Keep the original Z
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpolate X and Y, keep Z fixed
            Vector2 newPos2D = Vector2.Lerp(startPosition, target, elapsedTime / duration);
            obj.position = new Vector3(newPos2D.x, newPos2D.y, startZ);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap exactly to target, still preserving Z
        obj.position = new Vector3(target.x, target.y, startZ);
    }
}
