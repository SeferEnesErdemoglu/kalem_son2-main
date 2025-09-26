using UnityEngine;
using UnityEngine.InputSystem;

public class AssemblyZoomController : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputActionProperty zoomAction;

    [Header("Zoom Ayarlar�")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minFov = 20f;
    [SerializeField] private float maxFov = 60f;

    private float initialFov;

    private void Awake()
    {
        if (mainCamera != null) initialFov = mainCamera.fieldOfView;
    }

    private void Update()
    {
        // Bu script sadece 'enabled' oldu�unda �al���r.
        float joystickValue = zoomAction.action.ReadValue<Vector2>().y;
        float newFov = mainCamera.fieldOfView - (joystickValue * zoomSpeed * Time.deltaTime);
        mainCamera.fieldOfView = Mathf.Clamp(newFov, minFov, maxFov);
    }

    public void ResetZoom()
    {
        if (mainCamera != null) mainCamera.fieldOfView = initialFov;
    }

}