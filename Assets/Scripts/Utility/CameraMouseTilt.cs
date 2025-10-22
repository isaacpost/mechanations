using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMouseTilt : MonoBehaviour
{
    [Header("Settings")]
    public float maxUpwardRotation = 15f;    // Max upward tilt (degrees)
    public float maxDownwardRotation = 15f;  // Max downward tilt (degrees)
    public float maxRotationY = 15f;         // Side tilt (yaw)
    public float maxPositionOffset = 0.5f;   // XY movement offset
    public float maxZOffset = 0.5f;          // Forward/back offset
    public float smoothSpeed = 5f;           // Smooth interpolation speed

    private Quaternion _initialRotation;
    private Vector3 _initialPosition;

    void Start()
    {
        _initialRotation = transform.localRotation;
        _initialPosition = transform.localPosition;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Normalize mouse position around screen center (-1 to +1)
        float mouseX = (mousePos.x / Screen.width - 0.5f) * 2f;
        float mouseY = (mousePos.y / Screen.height - 0.5f) * 2f;

        // --- ROTATION ---
        // Mouse up (positive Y) → tilt UP
        // Mouse down (negative Y) → tilt DOWN
        float targetPitchX = Mathf.Lerp(-maxDownwardRotation, maxUpwardRotation, (mouseY + 1f) / 2f);
        float targetYawY = Mathf.Clamp(mouseX * maxRotationY, -maxRotationY, maxRotationY);

        Quaternion targetRot = Quaternion.Euler(targetPitchX, targetYawY, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * smoothSpeed);

        // --- POSITION ---
        // Mouse down → move forward (closer)
        // Mouse up → move backward (further)
        Vector3 xyOffset = new Vector3(mouseX, -mouseY, 0f) * maxPositionOffset;
        float forwardOffset = Mathf.Lerp(maxZOffset, -maxZOffset, (mouseY + 1f) / 2f); // bottom=in, top=out
        Vector3 zOffset = transform.forward * forwardOffset;

        Vector3 targetPos = _initialPosition + xyOffset + zOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothSpeed);
    }
}
