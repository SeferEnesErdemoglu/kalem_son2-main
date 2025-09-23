using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input System i�in gerekli
using System.Collections;

/// <summary>
/// Bu script, belirli XR Grab Interactable nesneleri tutuldu�unda
/// sol el joysti�inin dikey eksen hareketini kullanarak kamera zoom'u yapar.
/// Nesne b�rak�ld���nda zoom'u yumu�ak bir �ekilde eski haline getirir.
/// </summary>
public class AssemblyZoomController : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Zoom yap�lacak ana kamera.")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Sol el joysti�inin Vector2 hareketini referans alan Input Action.")]
    [SerializeField] private InputActionProperty zoomAction;

    [Header("Zoom Ayarlar�")]
    [Tooltip("Zoom'un ne kadar h�zl� olaca��n� belirler.")]
    [SerializeField] private float zoomSpeed = 30f;

    [Tooltip("Ula��labilecek en yak�n Field of View (FOV) de�eri. Daha d���k de�er = daha fazla zoom.")]
    [SerializeField] private float minFov = 20f;

    [Tooltip("Ula��labilecek en uzak Field of View (FOV) de�eri. Genellikle kameran�n ba�lang�� de�eri olmal�d�r.")]
    [SerializeField] private float maxFov = 60f;

    [Tooltip("Nesne b�rak�ld���nda zoom'un eski haline d�nme h�z�.")]
    [SerializeField] private float zoomResetSpeed = 2f;

    // --- �zel De�i�kenler ---
    private bool isZoomActive = false; // Zoom'un aktif olup olmad���n� kontrol eden bayrak
    private float initialFov; // Kameran�n ba�lang��taki FOV de�eri
    private Coroutine resetCoroutine; // Zoom resetleme coroutine'ini tutmak i�in

    private void Awake()
    {
        // E�er kamera edit�rden atanmam��sa, sahnedeki ana kameray� bulmay� dene.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            // Ba�lang�� FOV de�erini kaydet
            initialFov = mainCamera.fieldOfView;
            // E�er maxFov kullan�c� taraf�ndan ayarlanmad�ysa, ba�lang�� de�erini ata
            if (maxFov <= 0)
            {
                maxFov = initialFov;
            }
        }
        else
        {
            Debug.LogError("Zoom yap�lacak bir kamera bulunamad�! L�tfen Main Camera referans�n� atay�n.");
            this.enabled = false; // Script'i devre d��� b�rak
        }
    }

    private void Update()
    {
        // E�er zoom aktif de�ilse, Update fonksiyonunda hi�bir �ey yapma.
        if (!isZoomActive)
        {
            return;
        }

        // Joystick'in dikey (Y) eksenindeki de�erini oku (-1 ile 1 aras�nda)
        float joystickValue = zoomAction.action.ReadValue<Vector2>().y;

        // Mevcut FOV de�erini, joystick hareketine g�re de�i�tir.
        // Joystick ileri (pozitif Y) itildi�inde FOV k���lmeli (zoom in), bu y�zden de�eri ��kart�yoruz.
        float newFov = mainCamera.fieldOfView - (joystickValue * zoomSpeed * Time.deltaTime);

        // Yeni FOV de�erini min ve max limitleri aras�nda s�k��t�r.
        mainCamera.fieldOfView = Mathf.Clamp(newFov, minFov, maxFov);
    }

    /// <summary>
    /// Bu metod, montajlanabilir bir nesne tutuldu�unda �a�r�l�r. Zoom'u aktif eder.
    /// XR Grab Interactable'�n "Select Entered" event'inden �a�r�lmal�d�r.
    /// </summary>
    public void ActivateZoom()
    {
        // E�er varsa, devam eden zoom resetleme i�lemini durdur.
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        isZoomActive = true;
        Debug.Log("Zoom Aktif!");
    }

    /// <summary>
    /// Bu metod, montajlanabilir nesne b�rak�ld���nda �a�r�l�r. Zoom'u devre d��� b�rak�r
    /// ve kameray� yumu�ak�a ba�lang�� FOV de�erine d�nd�r�r.
    /// XR Grab Interactable'�n "Select Exited" event'inden �a�r�lmal�d�r.
    /// </summary>
    public void DeactivateZoom()
    {
        isZoomActive = false;
        // Kameray� ba�lang�� FOV de�erine yumu�ak�a d�nd�rmek i�in Coroutine'i ba�lat.
        resetCoroutine = StartCoroutine(ResetZoom());
        Debug.Log("Zoom Devre D���! Resetleniyor...");
    }

    /// <summary>
    /// Kameran�n Field of View'�n� ba�lang�� de�erine yumu�ak bir ge�i�le (Lerp) d�nd�r�r.
    /// </summary>
    private IEnumerator ResetZoom()
    {
        float currentFov = mainCamera.fieldOfView;
        float elapsedTime = 0f;

        // FOV de�eri ba�lang�� de�erine �ok yakla�ana kadar d�n.
        while (!Mathf.Approximately(mainCamera.fieldOfView, initialFov))
        {
            elapsedTime += Time.deltaTime * zoomResetSpeed;
            mainCamera.fieldOfView = Mathf.Lerp(currentFov, initialFov, elapsedTime);
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Tam olarak ba�lang�� de�erine ayarla ve coroutine'i sonland�r.
        mainCamera.fieldOfView = initialFov;
        resetCoroutine = null;
    }
}