using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum to track the state of the drone from 1 of 4 options
public enum DroneState
{
    Protect,
    Shoot,
    Waiting,
    Scatter
}

// Controller to indicate the logic of a drone a part of the boss, The Swarm
public class DroneController : Agent, IDamagable
{
    [SerializeField]
    private GameObject tower; // The boss itself

    [SerializeField]
    private float towerProtectWeight; // Weight of drone logic to surround tower

    [SerializeField]
    private float towerShootWeight; // Weight of drone logic to orbit in circle and shoot

    [SerializeField]
    private float droneWeight; // Weight to move away from other drones

    [SerializeField]
    private float wanderWeight; // Weight of drone wandering

    [SerializeField]
    private GameObject explosionPrefab; // Prefab to instantiate if destroyed

    [SerializeField]
    private GameObject projectilePrefab; // The projectile the drone shoots

    [SerializeField]
    private float shootCooldown = 1f; // Time between shots

    // Attributes used to keep track of movement forces
    private Vector2 towerForce;
    private Vector2 droneForce;
    private Vector2 wanderForce;

    private float lastShootTime = 0f; // Timer to know when to shoot next
    private int health = 2;
    private int droneIndex; // Drone identifier
    private int numDrones; // Number of total drones in the scene
    private DroneState droneState; // Uses above enum to keep track of movements state
    private Animator animator;
    private Transform parentObject; // The tower of The Swarm

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        parentObject = transform.parent;
    }

    protected override Vector2 CalcSteering()
    {
        Vector2 totalForce = Vector2.zero;

        switch (droneState) 
        { 
            case DroneState.Protect:
                lastShootTime = Time.time;

                towerForce = Orbit(tower, 0.75f, 100.0f) * towerProtectWeight;
                totalForce += towerForce;

                wanderForce = Wander() * wanderWeight;
                totalForce += wanderForce;

                break;

            case DroneState.Shoot:
                towerForce = OrbitEven(tower, 1.75f, 50.0f, droneIndex, numDrones) * towerShootWeight;
                totalForce += towerForce;

                // Check if enough time has passed since the last shot
                if (Time.time >= lastShootTime + shootCooldown)
                {
                    Shoot();
                    lastShootTime = Time.time; // Update last shoot time
                }

                break;

            case DroneState.Waiting:
                towerForce = OrbitEven(tower, 1.75f, 50.0f, droneIndex, numDrones) * towerShootWeight;
                totalForce += towerForce;

                break;

            case DroneState.Scatter:
                droneForce = Flee(FindClosestObject(tower)) * droneWeight;
                totalForce += droneForce;

                break;
        }

        return totalForce;
    }

    public void Shoot()
    {
        if (projectilePrefab == null || parentObject == null) return;

        // Calculate the direction from the parent's center to the drone
        Vector2 shootDirection = (transform.position - parentObject.position).normalized;

        // Calculate the rotation angle
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // Instantiate projectile and apply rotation
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
    }

    public void SetAttributes(DroneState droneState, int index, int totalNum)
    {
        this.droneState = droneState;
        this.droneIndex = index;
        this.numDrones = totalNum;
    }

    public void SetTower(GameObject tower)
    {
        this.tower = tower;
    }

    GameObject FindClosestObject(GameObject prefab)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(prefab.tag))
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < closestDistance && distance != 0f)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }

        return closestObject;
    }

    public void TakeDamage(float dmg)
    {
        health = health - 1;

        if (health == 0)
        {
            Died();
        }
        else
        {
            animator.SetTrigger("TookDamage");
        }

    }

    public void Died()
    {
        Instantiate(explosionPrefab, gameObject.transform.position, transform.rotation);
        SFXManager.Instance.PlaySound("TurretHit");
        Destroy(gameObject); // Destroy the projectile
    }
}
