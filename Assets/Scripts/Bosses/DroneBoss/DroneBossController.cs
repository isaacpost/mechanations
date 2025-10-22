using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for the second boss, The Swarm
public class DroneBossController : MonoBehaviour, IDamagable
{
    [SerializeField]
    private GameObject objectToSpawn; // Prefab to instantiate

    [SerializeField]
    private float spawnRadius = 5f; // Radius of the circle

    [SerializeField]
    private float phaseTwoThreshold = 67f; // Health threshold to go to phase two

    [SerializeField]
    private float phaseThreeThreshold = 33f; // health threshold to go to phase three

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Health health; // Controls health, which controls healthbar display
    private bool goneToPhaseTwo = false; // If the boss has gone to phase 2
    private bool goneToPhaseThree = false; // If the boss has gone to phase 3
    private GameObject signal; // Signal sprite that appears when the boss sends a signal

    void Start()
    {
        health = GetComponent<Health>();
        signal = transform.Find("Signal").gameObject;
    }

    public void SpawnDronesInCircle(int numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Calculate the angle for this object
            float angle = i * (360f / numberOfObjects);
            float angleInRadians = angle * Mathf.Deg2Rad;

            // Calculate the position on the circle
            Vector2 spawnPosition = new Vector2(
                Mathf.Cos(angleInRadians) * spawnRadius,
                Mathf.Sin(angleInRadians) * spawnRadius
            );

            // Instantiate the object at the calculated position
            GameObject spawnedObject = Instantiate(
                objectToSpawn,
                (Vector2)transform.position + spawnPosition,
                Quaternion.identity
            );

            spawnedObject.gameObject.GetComponent<DroneController>().SetTower(gameObject);

            spawnedObject.transform.SetParent(transform);

            spawnedObjects.Add(spawnedObject);

            UpdateDroneStates(DroneState.Waiting);
        }
    }

    public void UpdateDroneStates(DroneState droneState)
    {
        spawnedObjects.RemoveAll(item => item == null);

        int listSize = spawnedObjects.Count;

        for (int i = 0; i < listSize; i++)
        {
            spawnedObjects[i].GetComponent<DroneController>().SetAttributes(droneState, i, listSize);
        }
    }

    public IEnumerator DroneBossSequence()
    {
        while (true)
        {
            yield return StartCoroutine(DroneBossSignal());

            UpdateDroneStates(DroneState.Shoot);

            yield return new WaitForSeconds(5.0f);

            yield return StartCoroutine(DroneBossSignal());

            UpdateDroneStates(DroneState.Protect);

            yield return new WaitForSeconds(5.0f);
        }
    }

    public IEnumerator DroneBossSignal()
    {
        signal.SetActive(true);

        SFXManager.Instance.PlaySound("DroneBossSignal");

        yield return new WaitForSeconds(1.75f);

        signal.SetActive(false);
    }

    public void TakeDamage(float dmg)
    {
        health.TakeDamage(dmg);

        if (spawnedObjects.Count == 0) 
        {
            SFXManager.Instance.PlaySound("DroneBossSignal");
            DroneBossSignal();
            SpawnDronesInCircle(5);
        }

        if (health.GetCurrentHealth() <= phaseTwoThreshold && !goneToPhaseTwo)
        {
            SFXManager.Instance.PlaySound("DroneBossSignal");
            DroneBossSignal();
            SpawnDronesInCircle(25);
            goneToPhaseTwo = true;
        }
        else if (health.GetCurrentHealth() <= phaseThreeThreshold && !goneToPhaseThree)
        {
            SFXManager.Instance.PlaySound("DroneBossSignal");
            DroneBossSignal();
            SpawnDronesInCircle(50);
            goneToPhaseThree = true;
        }
    }

    public void Died()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;

        LifecycleManager.Instance.BossDied();
    }
}
