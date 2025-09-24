using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Bu script, iki belirli montaj parçasý AYNI ANDA tutulduðunda
/// sol el joystiði ile kamera zoom'u yapar. Parçalardan biri býrakýldýðýnda
/// zoom otomatik olarak devre dýþý kalýr.
/// </summary>
public class AssemblyZoomController : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Zoom yapýlacak ana kamera.")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Sol el joystiðinin Vector2 hareketini referans alan Input Action.")]
    [SerializeField] private InputActionProperty zoomAction;

    [Header("Zoom Ayarlarý")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minFov = 20f;
    [SerializeField] private float maxFov = 60f;
    [SerializeField] private float zoomResetSpeed = 2f;

    // --- Özel Deðiþkenler ---
    private bool isZoomActive = false;
    private float initialFov;
    private Coroutine resetCoroutine;

    // Durum Takip Deðiþkenleri
    private bool isPenHeld = false;
    private bool isCapHeld = false;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        if (mainCamera != null)
        {
            initialFov = mainCamera.fieldOfView;
            if (maxFov <= 0) maxFov = initialFov;
        }
        else
        {
            Debug.LogError("Zoom yapýlacak bir kamera bulunamadý!");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (!isZoomActive)
        {
            return;
        }

        float joystickValue = zoomAction.action.ReadValue<Vector2>().y;
        float newFov = mainCamera.fieldOfView - (joystickValue * zoomSpeed * Time.deltaTime);
        mainCamera.fieldOfView = Mathf.Clamp(newFov, minFov, maxFov);
    }

    // --- Yeni Fonksiyonlar ---

    // Bu metod, Kalem tutulduðunda çaðrýlacak.
    public void OnPenGrabbed()
    {
        isPenHeld = true;
        CheckZoomStatus();
    }

    // Bu metod, Kalem býrakýldýðýnda çaðrýlacak.
    public void OnPenReleased()
    {
        isPenHeld = false;
        CheckZoomStatus();
    }

    // Bu metod, Kapak tutulduðunda çaðrýlacak.
    public void OnCapGrabbed()
    {
        isCapHeld = true;
        CheckZoomStatus();
    }

    // Bu metod, Kapak býrakýldýðýnda çaðrýlacak.
    public void OnCapReleased()
    {
        isCapHeld = false;
        CheckZoomStatus();
    }

    /// <summary>
    /// Her iki parçanýn da tutulma durumunu kontrol eder ve zoom'u buna göre ayarlar.
    /// </summary>
    private void CheckZoomStatus()
    {
        if (isPenHeld && isCapHeld)
        {
            // ÝKÝSÝ DE TUTULUYORSA: Zoom'u aktif et.
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);
            isZoomActive = true;
            Debug.Log("Zoom AKTÝF: Her iki parça da tutuluyor.");
        }
        else
        {
            // EN AZ BÝRÝ BIRAKILDIYSA: Zoom'u devre dýþý býrak.
            if (isZoomActive) // Sadece eðer daha önce aktifse resetle.
            {
                isZoomActive = false;
                resetCoroutine = StartCoroutine(ResetZoom());
                Debug.Log("Zoom DEVRE DIÞI: Parçalardan biri býrakýldý.");
            }
        }
    }

    private IEnumerator ResetZoom()
    {
        float currentFov = mainCamera.fieldOfView;
        float elapsedTime = 0f;
        while (!Mathf.Approximately(mainCamera.fieldOfView, initialFov))
        {
            elapsedTime += Time.deltaTime * zoomResetSpeed;
            mainCamera.fieldOfView = Mathf.Lerp(currentFov, initialFov, elapsedTime);
            yield return null;
        }
        mainCamera.fieldOfView = initialFov;
        resetCoroutine = null;
    }
}