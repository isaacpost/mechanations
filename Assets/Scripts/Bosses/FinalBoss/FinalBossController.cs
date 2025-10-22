using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D.IK;

// Main logic controller for the final boss. Keeps track of attributes
// and movement/attack sequences.
public class FinalBossController: MonoBehaviour, IDamagable
{
    [SerializeField] private Transform target; // Right hand movement target
    [SerializeField] private Transform player; // Player object
    [SerializeField] private Animator clawAnimator;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private float horizontalSpeed; // Speed at which boss moves back and forth
    [SerializeField] private float clawHandSpeed; // Speed at which hand moves to target
    [SerializeField] private float centerX;
    [SerializeField] private IKManager2D ikManager; // Kinematic manager for right hand
    [SerializeField] private float partClawDistance = 2.5f; // Distance to grab parts using claw
    [SerializeField] private float laserShootTime;
    [SerializeField] private Transform shootFromLocation;
    [SerializeField] private float phaseTwoThreshold; // Threshold to move into phase two

    [Header("Move Limits")]
    [SerializeField] private float maxMoveX;
    [SerializeField] private float minMoveX;

    [Header("Body Segments")]
    [SerializeField] private Transform rightLowerArm;
    [SerializeField] private Transform rightUpperArm;
    [SerializeField] private Transform leftLowerArm;
    [SerializeField] private Transform leftUpperArm;

    [Header("Effect Objects")]
    [SerializeField] private GameObject sparks;
    [SerializeField] private GameObject tears;

    [Header("Prefabs")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject smallProjectilePrefab;
    [SerializeField] private GameObject laserSegmentPrefab;
    [SerializeField] private GameObject explosionPrefab;

    private Coroutine mainSequenceCoroutine; // Keeps sequence coroutine to end on death
    private readonly float laserSegmentSpacing = 0.2f; // Effect spacing when destroying grid
    private Coroutine moveCoroutine; // Keeps move coroutine
    private Health health; // Health script on object
    private Vector2 originalTargetPos = new Vector2(0.5f, 2f); // Original position of claw

    void Start()
    {
        health = GetComponent<Health>();
    }

    public void SetMainSequenceCoroutine(Coroutine mainSequence)
    {
        mainSequenceCoroutine = mainSequence;
    }

    public IEnumerator FinalBossSequence()
    {
        while (true)
        {
            GameObject foundPart = FindRandomPart();
            if (foundPart != null)
            {
                Vector3 desiredPartLocation = foundPart.transform.position;

                yield return StartCoroutine(MoveTargetToPosition(foundPart.transform.position, clawHandSpeed));

                float partDistance = Vector3.Distance(foundPart.transform.position, transform.position);

                if ((foundPart.transform.position == desiredPartLocation) && partDistance <= partClawDistance)
                {
                    foundPart = gridManager.PickUpItem(desiredPartLocation);

                    clawAnimator.SetTrigger("CloseClaw");

                    foundPart.transform.SetParent(rightLowerArm);

                    yield return StartCoroutine(MoveTargetToPosition(transform.TransformPoint(originalTargetPos), clawHandSpeed));
                    ikManager.enabled = false;

                    SFXManager.Instance.PlaySound("FinalBossCharge");

                    yield return StartCoroutine(RotateOverTime(rightLowerArm, 1755f, 3f));

                    clawAnimator.SetTrigger("OpenClaw");

                    GameObject partProj = Instantiate(projectilePrefab, foundPart.transform.position, Quaternion.Euler(0, 0, -45));

                    partProj.GetComponent<FinalBossPartProjectile>().SetSprite(
                        foundPart.GetComponent<SpriteRenderer>().sprite
                    );

                    Destroy(foundPart);

                    SFXManager.Instance.PlaySound("TurretShoot");

                    yield return new WaitForSeconds(1f);
                    yield return StartCoroutine(RotateOverTime(rightLowerArm, 45f, 1f));
                    ikManager.enabled = true;
                }
                else
                {
                    yield return StartCoroutine(MoveTargetToPosition(transform.TransformPoint(originalTargetPos), clawHandSpeed));
                }
            }
            else
            {
                yield return StartCoroutine(MoveTargetToPosition(transform.TransformPoint(originalTargetPos), clawHandSpeed));
            }

            if (health.GetCurrentHealth() <= phaseTwoThreshold)
            {
                moveCoroutine = StartCoroutine(MoveByOffset(maxMoveX - transform.position.x));
                StartCoroutine(Shoot(10));
                yield return StartCoroutine(DestroyGrid());
            }
            else
            {
                StartCoroutine(Shoot(3));
            }

            Transform rightmostPart = gridManager.FindRightmostPart();
            if (rightmostPart != null)
            {
                moveCoroutine = StartCoroutine(MoveByOffset(rightmostPart.position.x - transform.position.x));
            }
            else
            {
                moveCoroutine = StartCoroutine(MoveByOffset(minMoveX - transform.position.x));
            }

            yield return StartCoroutine(RotateOverTime(leftLowerArm, 90f, 0.5f));

            SFXManager.Instance.PlaySound("FinalBossSaw");

            yield return StartCoroutine(RotateOverTime(leftUpperArm, -360f, 3f));

            yield return StartCoroutine(RotateOverTime(leftLowerArm, -90f, 0.5f));
        }
    }

    public IEnumerator Shoot(int numShots)
    {
        // Calculate the rotation angle
        float angle = Random.Range(60f, 120f);

        for (int i = 0; i < numShots; i++)
        {
            // Instantiate projectile and apply rotation
            Instantiate(smallProjectilePrefab, shootFromLocation.position, Quaternion.Euler(0, 0, angle));
            SFXManager.Instance.PlaySound("SmallProjectile");
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator DeathAnimation()
    {
        StopCoroutine(mainSequenceCoroutine);

        Instantiate(explosionPrefab, leftUpperArm.position, Quaternion.identity);
        Instantiate(explosionPrefab, rightUpperArm.position, Quaternion.identity);

        StartCoroutine(RotateOverTime(leftUpperArm, 3600f, 5f));
        yield return StartCoroutine(RotateOverTime(rightUpperArm, 3600f, 5f));

        tears.SetActive(true);

        yield return new WaitForSeconds(5f);

        GameObject explosion = Instantiate(explosionPrefab, tears.transform.position, transform.rotation);
        explosion.transform.localScale = new Vector3(10f, 10f, 10f);
        SFXManager.Instance.PlaySound("TurretHit");
    }

    private IEnumerator DestroyGrid()
    {
        sparks.SetActive(true);

        yield return new WaitForSeconds(1f);

        GameObject gridToDestroy = gridManager.GetAndRemoveRandomGrid();

        SFXManager.Instance.PlaySound("FinalBossLaser");

        yield return StartCoroutine(CreateLaser(shootFromLocation, gridToDestroy.transform));

        sparks.SetActive(false);

        Instantiate(explosionPrefab, gridToDestroy.transform.position, Quaternion.identity);

        SFXManager.Instance.PlaySound("TurretHit");

        Transform tileHighlight = GetChildWithName("TileHighlight", gridToDestroy);
        if (tileHighlight)
        {
            tileHighlight.SetParent(null);
            tileHighlight.gameObject.SetActive(false);
        }

        Destroy(gridToDestroy);
    }

    public IEnumerator MoveArmsIntoPlace()
    {
        StartCoroutine(MoveObjectToZeroY(rightUpperArm.gameObject, 3f));
        yield return StartCoroutine(MoveObjectToZeroY(leftUpperArm.gameObject, 3f));
        SFXManager.Instance.PlaySound("FinalBossIntro");
    }

    private IEnumerator MoveObjectToZeroY(GameObject objectToMove, float duration)
    {
        Vector3 startPosition = objectToMove.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, 0f, startPosition.z);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            objectToMove.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToMove.transform.position = targetPosition;
    }

    private Transform GetChildWithName(string name, GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            if (child.name == name)
            {
                return child;
            }
        }

        return null;
    }

    private IEnumerator RotateOverTime(Transform objectTransform, float degrees, float duration)
    {
        float elapsedTime = 0f;
        float startingRotation = objectTransform.eulerAngles.z;

        float targetRotation = startingRotation + degrees;

        while (elapsedTime < duration)
        {
            float easedTime = Mathf.Pow(elapsedTime / duration, 2);

            float currentRotation = Mathf.Lerp(startingRotation, targetRotation, easedTime);

            objectTransform.eulerAngles = new Vector3(0, 0, currentRotation);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        objectTransform.eulerAngles = new Vector3(0, 0, targetRotation);
    }

    private IEnumerator MoveByOffset(float offsetX)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        Vector3 startPosition = transform.position;
        float targetX = startPosition.x + offsetX;

        float clampedTargetX = Mathf.Clamp(targetX, minMoveX, maxMoveX);
        Vector3 endPosition = new Vector3(clampedTargetX, startPosition.y, startPosition.z);

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * horizontalSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return null;
        }

        transform.position = endPosition;
    }

    private IEnumerator MoveTargetToPosition(Vector2 final, float duration)
    {
        ikManager.enabled = true;

        Vector3 startPos = target.transform.position;
        Vector3 endPos = final;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.transform.position = endPos;

        ikManager.enabled = false;
    }

    private GameObject FindRandomPart()
    {
        float radius = partClawDistance;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        var matchingObjects = colliders
            .Select(c => c.gameObject)
            .Where(go => go.CompareTag("Part"))
            .Where(go => go.GetComponent<Part>().GetIsPlaced())
            .ToList();

        if (matchingObjects.Count > 0)
        {
            GameObject randomObject = matchingObjects[Random.Range(0, matchingObjects.Count)];
            return randomObject;
        }
        else
        {
            return null;
        }
    }

    private IEnumerator CreateLaser(Transform startPoint, Transform endPoint)
    {
        List<GameObject> segments = new List<GameObject>();

        if (startPoint && endPoint && laserSegmentPrefab)
        {
            Vector3 direction = (endPoint.position - startPoint.position).normalized;
            float distance = Vector3.Distance(startPoint.position, endPoint.position);
            int segmentCount = Mathf.CeilToInt(distance / laserSegmentSpacing);

            float creationDuration = laserShootTime * 0.5f;
            float delayPerSegment = creationDuration / segmentCount;

            for (int i = 0; i < segmentCount; i++)
            {
                Vector3 segmentPos = startPoint.position + direction * (i * laserSegmentSpacing);
                GameObject segment = Instantiate(laserSegmentPrefab, segmentPos, Quaternion.identity, transform);
                segments.Add(segment);

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                segment.transform.rotation = Quaternion.Euler(0, 0, angle);

                yield return new WaitForSeconds(delayPerSegment);
            }

            yield return new WaitForSeconds(laserShootTime - creationDuration);

            foreach (GameObject segment in segments)
            {
                Destroy(segment);
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        health.TakeDamage(dmg);

        moveCoroutine = StartCoroutine(MoveByOffset(2f));
    }

    public void Died()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;

        LifecycleManager.Instance.BossDied();
    }
}
