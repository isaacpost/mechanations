using System.Collections;
using UnityEditor;
using UnityEngine;

// Inherits methods from LifecycleManager to manage boss lifecycle
public class DroneBossLifecycleManager : LifecycleManager
{
    [SerializeField] GameObject droneBoss; // The boss gameobject
    [SerializeField] GameObject introCanvas; // The canvas that is shown at the beginning
    [SerializeField] ConveyorBeltManager conveyorBeltManagerTop;
    [SerializeField] ConveyorBeltManager conveyorBeltManagerBottom;
    [SerializeField] GameObject explosionPrefab; // Explosion prefab to instantate

    private Animator bossAnimator; // Controls current animations
    private Coroutine mainDroneLoop; // Tracks the loop the drones follow during fight

    protected override void Awake()
    {
        base.Awake();

        bossAnimator = droneBoss.GetComponent<Animator>();

        SFXManager.Instance.LoadMusic("BossTwoMusic");
        SFXManager.Instance.PauseMusic();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override IEnumerator IntroSequence()
    {
        DroneBossController controller = droneBoss.GetComponent<DroneBossController>();

        while (!bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("DroneBossStatic"))
        {
            yield return null; // Wait for the next frame
        }

        yield return controller.DroneBossSignal();

        controller.SpawnDronesInCircle(10);

        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(TypeText("THE SWARM"));

        yield return new WaitForSeconds(1f);

        conveyorBeltManagerTop.SetIsRunning(true);
        conveyorBeltManagerBottom.SetIsRunning(true);
        EnableGameObjects();
        mainDroneLoop =  StartCoroutine(controller.DroneBossSequence());
        introCanvas.SetActive(false);
    }

    protected override IEnumerator DeathSequence()
    {
        DroneBossController controller = droneBoss.GetComponent<DroneBossController>();

        StopCoroutine(mainDroneLoop);

        yield return controller.DroneBossSignal();

        controller.SpawnDronesInCircle(50);

        controller.UpdateDroneStates(DroneState.Protect);

        for (int i = 0; i < 10; i++) 
        {
            GameObject explosion = Instantiate(explosionPrefab, droneBoss.transform.position, droneBoss.transform.rotation);
            explosion.transform.localScale = new Vector3(2f, 2f, 2f);
            SFXManager.Instance.PlaySound("TurretHit");

            yield return new WaitForSeconds(0.5f);
        }

        droneBoss.GetComponent<Animator>().enabled = false;
        droneBoss.GetComponent<SpriteRenderer>().enabled = false;

        controller.UpdateDroneStates(DroneState.Scatter);

        yield return new WaitForSeconds(3f);

        SFXManager.Instance.PlaySound("BossDefeat");

        Time.timeScale = 0f;

        MenuManager.Instance.OpenMenu("YouWinMenuCanvas");

        GameManager.Instance.DefeatBoss(2);
    }
}
