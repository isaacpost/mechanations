using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoatState
{
    Stationary,
    HoveringAtPoint
}

public class GoatBossController : Agent, IDamagable
{

    [SerializeField]
    private List<GameObject> hoverPoints;

    [SerializeField]
    private RotateOverTime gridRotate; // Prefab to instantiate

    [Header("Cone Projectile Attribtues")]
    public GameObject projectilePrefab;
    public Transform target;
    public int projectileCount = 10;
    public float coneAngle = 45f;  // Total spread angle

    private GoatState goatState;
    private GameObject currentHoverPoint;
    private Health health;

    private void Start()
    {
        currentHoverPoint = hoverPoints[0];
        goatState = GoatState.Stationary;

        health = GetComponent<Health>();
    }

    protected override Vector2 CalcSteering()
    {
        Vector2 totalForce = Vector2.zero;

        switch (goatState)
        {
            case GoatState.Stationary:
                break;

            case GoatState.HoveringAtPoint:
                Vector2 orbitForce = Orbit(currentHoverPoint, 0.25f, 100.0f) * 2;
                totalForce += orbitForce;
                break;
        }

        return totalForce;
    }

    public IEnumerator GoatBossSequence()
    {
        goatState = GoatState.HoveringAtPoint;
        int pointIndex = 1;

        while (true)
        {
            yield return StartCoroutine(gridRotate.RotateObject(-90f, 0.5f));

            yield return new WaitForSeconds(1.0f + (1 - health.CalculateScaleFactor()) * 4f);

            SpawnCone();

            currentHoverPoint = hoverPoints[pointIndex];
            pointIndex++;

            if (pointIndex > hoverPoints.Count - 1)
            {
                pointIndex = 0;
            }
        }
    }

    void SpawnCone()
    {
        if (target == null) return;

        // Get base angle towards target
        Vector2 dirToTarget = (target.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg; // Convert to degrees

        for (int i = 0; i < projectileCount; i++)
        {
            float angleStep = coneAngle / (projectileCount - 1);
            float spawnAngle = baseAngle - (coneAngle / 2) + (angleStep * i); // Spread around base angle

            // Apply rotation to projectile
            Quaternion rotation = Quaternion.Euler(0, 0, spawnAngle + 90);
            Instantiate(projectilePrefab, transform.position, rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            // Only deals damage if player is damageable
            if (!player.IsInvinsible())
            {
                player.TakeDamage(1f);
                SFXManager.Instance.PlaySound("PlayerHurt");
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        SFXManager.Instance.PlaySound("GoatBossDead");
        health.TakeDamage(dmg);
    }

    public void Died()
    {
        currentHoverPoint = target.gameObject;

        LifecycleManager.Instance.BossDied();
    }
}
