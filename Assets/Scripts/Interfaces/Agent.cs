 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract class to be inherited by controllers that
// require movement
public abstract class Agent : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rBody;

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private float wanderRadius = 5f; // How far out the agent would wander

    [SerializeField]
    private float wanderTime = 1f; // Time between each wander

    [SerializeField]
    private float targetRadius = 1f; // Target radius for arriving at a point

    [SerializeField]
    private float slowingRadius = 0.5f; // How far out it will start slowing

    // Atributes used for calculations
    protected Vector2 velocity, acceleration, steeringForce;

    private float wanderTimer; // Time since last wander

    private float orbitAngle = 0f; // Angle for orbiting

    void Update()
    {
        acceleration = Vector2.zero;
        steeringForce = CalcSteering();
        acceleration += steeringForce;
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = transform.position;

        velocity += acceleration * Time.fixedDeltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        if (velocity.magnitude > 0.5f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        }

        nextPosition += velocity * Time.fixedDeltaTime;
        rBody.MovePosition(nextPosition);

        acceleration = Vector2.zero;
    }

    protected abstract Vector2 CalcSteering();

    protected Vector2 Seek(Vector2 targetPos)
    {
        Vector2 desiredVelocity = targetPos - new Vector2(transform.position.x, transform.position.y);
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector2 seekForce = desiredVelocity - velocity;

        return seekForce;
    }

    protected Vector2 Seek(GameObject target)
    {
        return Seek(target.transform.position);
    }

    protected Vector2 Flee(Vector2 fleePos)
    {
        Vector2 desiredVelocity = -(fleePos - new Vector2(transform.position.x, transform.position.y));

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector2 fleeForce = desiredVelocity - velocity;

        return fleeForce;
    }

    protected Vector2 Flee(GameObject fleeObj)
    {
        return Flee(fleeObj.transform.position);
    }

    protected Vector2 Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            float randomAngle = Random.Range(0f, 360f);
            Vector2 newDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

            wanderTimer = wanderTime;

            return newDirection * wanderRadius;
        }

        return Vector2.zero;
    }

    protected Vector2 Arrive(Vector2 target)
    {
        Vector2 toTarget = target - (Vector2)transform.position;
        float distance = toTarget.magnitude;

        if (distance < targetRadius)
            return Vector2.zero;

        float speed = maxSpeed;
        if (distance < slowingRadius)
            speed = maxSpeed * (distance / slowingRadius);

        Vector2 desiredVelocity = toTarget.normalized * speed;
        return desiredVelocity - (Vector2)transform.up;
    }

    protected Vector2 Arrive(GameObject fleeObj)
    {
        return Arrive(fleeObj.transform.position);
    }

    protected Vector2 Orbit(GameObject target, float radius, float orbitSpeed)
    {
        // Increment orbit angle based on speed and time
        orbitAngle += orbitSpeed * Time.deltaTime;

        // Convert orbit angle to radians
        float angleInRadians = orbitAngle * Mathf.Deg2Rad;

        // Calculate the position on the circle relative to the target
        Vector2 targetPosition = target.transform.position;
        Vector2 orbitOffset = new Vector2(
            Mathf.Cos(angleInRadians) * radius,
            Mathf.Sin(angleInRadians) * radius
        );

        Vector2 orbitPosition = targetPosition + orbitOffset;

        return Seek(orbitPosition);
    }

    protected Vector2 OrbitEven(GameObject target, float radius, float orbitSpeed, int index, int totalObjects)
    {
        // Calculate the initial angle offset for this object based on its index
        float angleOffset = (360f / totalObjects) * index;

        // Increment orbit angle based on speed and time
        orbitAngle += orbitSpeed * Time.deltaTime;

        // Combine the orbit angle with the offset to spread objects equally
        float totalAngle = orbitAngle + angleOffset;

        // Convert the total angle to radians
        float angleInRadians = totalAngle * Mathf.Deg2Rad;

        // Calculate the position on the circle relative to the target
        Vector2 targetPosition = target.transform.position;
        Vector2 orbitOffset = new Vector2(
            Mathf.Cos(angleInRadians) * radius,
            Mathf.Sin(angleInRadians) * radius
        );

        Vector2 orbitPosition = targetPosition + orbitOffset;

        return Seek(orbitPosition);
    }
}
