using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMouseTilt : MonoBehaviour
{
    [Header("Input Source")]
    [SerializeField] private PlayerController player; // Assign in Inspector

    [Header("Tilt Settings")]
    [SerializeField] private float maxUpwardRotation = 3f;
    [SerializeField] private float maxDownwardRotation = 3f;
    [SerializeField] private float maxRotationY = 5f;
    [SerializeField] private float smoothSpeed = 3f;

    [Header("Offset Settings")]
    [SerializeField] private float maxPositionOffset = 1f;
    [SerializeField] private float maxZOffset = 1f;

    private Quaternion _startLocalRot;
    private Vector3 _startLocalPos;

    void Start()
    {
        _startLocalRot = transform.localRotation;
        _startLocalPos = transform.localPosition;

        if (player == null)
            player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (player == null)
            return;

        Vector2 moveDir = player.MovementInput;
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        // --- ROTATION (flipped directions) ---
        float tY = (moveDir.y + 1f) * 0.5f;
        // Invert pitch & yaw signs to flip rotation response
        float pitchX = Mathf.Lerp(maxUpwardRotation, -maxDownwardRotation, tY);
        float yawY = Mathf.Clamp(-moveDir.x * maxRotationY, -maxRotationY, maxRotationY);

        Quaternion tiltRot = Quaternion.Euler(pitchX, -yawY, 0f);
        Quaternion targetLocalRot = moveDir == Vector2.zero ? _startLocalRot : _startLocalRot * tiltRot;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetLocalRot,
            Time.deltaTime * smoothSpeed
        );

        // --- POSITION OFFSET (flipped to match new rotation) ---
        Vector3 planarOffset = new Vector3(-moveDir.x * maxPositionOffset, -moveDir.y * maxPositionOffset, 0f);
        float forwardOffset = Mathf.Lerp(-maxZOffset, maxZOffset, tY);
        Vector3 zOffset = new Vector3(0f, 0f, forwardOffset);

        Vector3 targetLocalPos = _startLocalPos + planarOffset + zOffset;
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetLocalPos,
            Time.deltaTime * smoothSpeed
        );
    }
}
