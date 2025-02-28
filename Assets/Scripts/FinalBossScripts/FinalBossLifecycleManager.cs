using System.Collections;
using UnityEngine;

// Inherits methods from LifecycleManager to manage boss lifecycle
public class FinalBossLifecycleManager : LifecycleManager
{
    [SerializeField] FinalBossController finalBoss; // The boss gameobject
    [SerializeField] GameObject introCanvas; // The canvas that is shown at the beginning
    [SerializeField] GameObject explosionPrefab; // Explosion prefab to instantate
    [SerializeField] GameObject conveyorBeltManager;

    protected override void Awake()
    {
        base.Awake();

        SFXManager.Instance.LoadMusic("BossThreeMusic");
        SFXManager.Instance.PauseMusic();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override IEnumerator IntroSequence()
    {
        yield return finalBoss.MoveArmsIntoPlace();

        yield return new WaitForSeconds(1f);

        // Writes text to screen
        yield return StartCoroutine(TypeText("THE DIRECTOR"));

        yield return new WaitForSeconds(1f);

        conveyorBeltManager.GetComponent<ConveyorBeltManager>().SetIsRunning(true);
        EnableGameObjects();
        introCanvas.SetActive(false);
        Coroutine mainSequenceCoroutine = StartCoroutine(finalBoss.FinalBossSequence());
        finalBoss.SetMainSequenceCoroutine(mainSequenceCoroutine);
    }

    protected override IEnumerator DeathSequence()
    {
        yield return StartCoroutine(finalBoss.DeathAnimation());

        finalBoss.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        SFXManager.Instance.PlaySound("BossDefeat");

        Time.timeScale = 0f;

        MenuManager.Instance.OpenMenu("YouWinFinalMenuCanvas");
    }
}
