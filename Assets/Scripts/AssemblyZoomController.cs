using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Bu script, iki belirli montaj par�as� AYNI ANDA tutuldu�unda
/// sol el joysti�i ile kamera zoom'u yapar. Par�alardan biri b�rak�ld���nda
/// zoom otomatik olarak devre d��� kal�r.
/// </summary>
public class AssemblyZoomController : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Zoom yap�lacak ana kamera.")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Sol el joysti�inin Vector2 hareketini referans alan Input Action.")]
    [SerializeField] private InputActionProperty zoomAction;

    [Header("Zoom Ayarlar�")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minFov = 20f;
    [SerializeField] private float maxFov = 60f;
    [SerializeField] private float zoomResetSpeed = 2f;

    // --- �zel De�i�kenler ---
    private bool isZoomActive = false;
    private float initialFov;
    private Coroutine resetCoroutine;

    // Durum Takip De�i�kenleri
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
            Debug.LogError("Zoom yap�lacak bir kamera bulunamad�!");
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

    // Bu metod, Kalem tutuldu�unda �a�r�lacak.
    public void OnPenGrabbed()
    {
        isPenHeld = true;
        CheckZoomStatus();
    }

    // Bu metod, Kalem b�rak�ld���nda �a�r�lacak.
    public void OnPenReleased()
    {
        isPenHeld = false;
        CheckZoomStatus();
    }

    // Bu metod, Kapak tutuldu�unda �a�r�lacak.
    public void OnCapGrabbed()
    {
        isCapHeld = true;
        CheckZoomStatus();
    }

    // Bu metod, Kapak b�rak�ld���nda �a�r�lacak.
    public void OnCapReleased()
    {
        isCapHeld = false;
        CheckZoomStatus();
    }

    /// <summary>
    /// Her iki par�an�n da tutulma durumunu kontrol eder ve zoom'u buna g�re ayarlar.
    /// </summary>
    private void CheckZoomStatus()
    {
        if (isPenHeld && isCapHeld)
        {
            // �K�S� DE TUTULUYORSA: Zoom'u aktif et.
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);
            isZoomActive = true;
            Debug.Log("Zoom AKT�F: Her iki par�a da tutuluyor.");
        }
        else
        {
            // EN AZ B�R� BIRAKILDIYSA: Zoom'u devre d��� b�rak.
            if (isZoomActive) // Sadece e�er daha �nce aktifse resetle.
            {
                isZoomActive = false;
                resetCoroutine = StartCoroutine(ResetZoom());
                Debug.Log("Zoom DEVRE DI�I: Par�alardan biri b�rak�ld�.");
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