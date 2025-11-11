using UnityEngine;

public class CameraFollowXY : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // The player to follow

    [Header("Follow Settings")]
    public float xFollowPadding = 0f;        // Horizontal offset from player
    public float yFollowPadding = 2f;        // Vertical offset above player
    public float followSmoothSpeed = 3f;     // Smooth speed for movement

    [Header("Clamp Settings")]
    public float minXPosition = -10f;        // Minimum X position allowed
    public float maxXPosition = 10f;         // Maximum X position allowed
    public float minYPosition = -10f;          // Minimum Y position allowed
    public float maxYPosition = 10f;         // Maximum Y position allowed

    private Vector3 _targetPos;

    void LateUpdate()
    {
        if (player == null)
            return;

        _targetPos = transform.position;

        // Calculate desired target positions
        float targetX = player.position.x + xFollowPadding;
        float targetY = player.position.y + yFollowPadding;

        // Clamp target positions to defined bounds
        targetX = Mathf.Clamp(targetX, minXPosition, maxXPosition);
        targetY = Mathf.Clamp(targetY, minYPosition, maxYPosition);

        // Smoothly interpolate toward clamped target
        _targetPos.x = Mathf.Lerp(_targetPos.x, targetX, Time.deltaTime * followSmoothSpeed);
        _targetPos.y = Mathf.Lerp(_targetPos.y, targetY, Time.deltaTime * followSmoothSpeed);

        // Apply new position
        transform.position = _targetPos;
    }
}
