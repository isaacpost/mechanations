using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMouseTilt : MonoBehaviour
{
    private readonly float maxUpwardRotation = 3f;
    private readonly float maxDownwardRotation = 3f;
    private readonly float maxRotationY = 5f;
    private readonly float smoothSpeed = 3f;
    
    private readonly float maxPositionOffset = 1f;
    private readonly float maxZOffset = 1f;
    
    private Quaternion _startLocalRot;
    private Vector3 _startLocalPos;

    void Start()
    {
        _startLocalRot = transform.localRotation;
        _startLocalPos = transform.localPosition;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        float mouseX = (mousePos.x / Screen.width - 0.5f) * 2f;
        float mouseY = (mousePos.y / Screen.height - 0.5f) * 2f;
        mouseX = Mathf.Clamp(mouseX, -1f, 1f);
        mouseY = Mathf.Clamp(mouseY, -1f, 1f);

        float tY = (mouseY + 1f) * 0.5f;
        float pitchX = Mathf.Lerp(-maxDownwardRotation, maxUpwardRotation, tY);
        float yawY = Mathf.Clamp(mouseX * maxRotationY, -maxRotationY, maxRotationY);
        Quaternion tiltRot = Quaternion.Euler(pitchX, yawY, 0f);
        Quaternion targetLocalRot = _startLocalRot * tiltRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRot, Time.deltaTime * smoothSpeed);

        Vector3 planarOffset = new Vector3(mouseX * maxPositionOffset, -mouseY * maxPositionOffset, 0f);
        float forwardOffset = Mathf.Lerp(maxZOffset, -maxZOffset, tY);
        Vector3 zOffset = new Vector3(0f, 0f, forwardOffset);
        Vector3 targetLocalPos = _startLocalPos + planarOffset + zOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * smoothSpeed);
    }
}
