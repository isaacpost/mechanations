using UnityEngine;

public class CameraMouseTilt : MonoBehaviour
{
    [Header("Input Source")]
    [SerializeField] private PlayerController player;

    [Header("Tilt Settings")]
    [SerializeField] private float maxUpwardRotation = 3f;     // when pressing DOWN
    [SerializeField] private float maxDownwardRotation = 3f;   // when pressing UP
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

        bool hasInput = moveDir.sqrMagnitude > 0.0001f;

        float tY = (moveDir.y + 1f) * 0.5f;

        // ---- ROTATION ----
        float pitchX = Mathf.Lerp(maxUpwardRotation, maxDownwardRotation, tY);

        // INVERT X ROTATION HERE
        pitchX = -pitchX;

        float yawY = moveDir.x * maxRotationY;

        Quaternion targetLocalRot = hasInput
            ? _startLocalRot * Quaternion.Euler(pitchX, yawY, 0f)
            : _startLocalRot;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetLocalRot,
            Time.deltaTime * smoothSpeed
        );

        // ---- POSITION OFFSET ----
        Vector3 offset = new Vector3(
            -moveDir.x * maxPositionOffset,
            -moveDir.y * maxPositionOffset,
            Mathf.Lerp(-maxZOffset, maxZOffset, tY)
        );

        Vector3 targetLocalPos = _startLocalPos + offset;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetLocalPos,
            Time.deltaTime * smoothSpeed
        );
    }
}
