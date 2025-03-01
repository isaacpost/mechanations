using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class GoatBossLifecycleManager : LifecycleManager
{
    [SerializeField] GameObject goatBoss; // The boss gameobject
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject introCanvas; // The canvas that is shown at the beginning
    [SerializeField] GameObject conveyorBeltManager;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject coffinPrefab;

    private Coroutine bossCoroutine;

    protected override void Awake()
    {
        base.Awake();

        SFXManager.Instance.LoadMusic("BossFourMusic");
        SFXManager.Instance.PauseMusic();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override IEnumerator IntroSequence()
    {
        GoatBossController controller = goatBoss.GetComponent<GoatBossController>();

        SFXManager.Instance.PlaySound("Static");

        yield return new WaitForSeconds(3.75f);

        yield return StartCoroutine(TypeText("THE DEMON"));

        yield return new WaitForSeconds(1f);

        conveyorBeltManager.GetComponent<ConveyorBeltManager>().SetIsRunning(true);
        EnableGameObjects();
        introCanvas.SetActive(false);
        bossCoroutine = StartCoroutine(controller.GoatBossSequence());
    }

    protected override IEnumerator DeathSequence()
    {
        StopCoroutine(bossCoroutine);

        for (int i = 0; i < 5; i++) 
        {
            yield return new WaitForSeconds(1f);

            SFXManager.Instance.PlaySound("GoatBossDead");
        }

        GameObject explosion = Instantiate(explosionPrefab, goatBoss.transform.position, transform.rotation);
        explosion.transform.localScale = new Vector3(5f, 5f, 5f);

        SFXManager.Instance.PlaySound("TurretHit");

        goatBoss.SetActive(false);
        playerObj.SetActive(false);

        Instantiate(coffinPrefab, goatBoss.transform.position, transform.rotation);
        Instantiate(coffinPrefab, playerObj.transform.position, transform.rotation);

        yield return new WaitForSeconds(5f);

        GameManager.Instance.EasterEggFound("BossFourScene");

        MenuManager.Instance.MainMenu();
    }
}
