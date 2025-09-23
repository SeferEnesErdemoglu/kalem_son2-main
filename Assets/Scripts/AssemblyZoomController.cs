using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input System için gerekli
using System.Collections;

/// <summary>
/// Bu script, belirli XR Grab Interactable nesneleri tutulduðunda
/// sol el joystiðinin dikey eksen hareketini kullanarak kamera zoom'u yapar.
/// Nesne býrakýldýðýnda zoom'u yumuþak bir þekilde eski haline getirir.
/// </summary>
public class AssemblyZoomController : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Zoom yapýlacak ana kamera.")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("Sol el joystiðinin Vector2 hareketini referans alan Input Action.")]
    [SerializeField] private InputActionProperty zoomAction;

    [Header("Zoom Ayarlarý")]
    [Tooltip("Zoom'un ne kadar hýzlý olacaðýný belirler.")]
    [SerializeField] private float zoomSpeed = 30f;

    [Tooltip("Ulaþýlabilecek en yakýn Field of View (FOV) deðeri. Daha düþük deðer = daha fazla zoom.")]
    [SerializeField] private float minFov = 20f;

    [Tooltip("Ulaþýlabilecek en uzak Field of View (FOV) deðeri. Genellikle kameranýn baþlangýç deðeri olmalýdýr.")]
    [SerializeField] private float maxFov = 60f;

    [Tooltip("Nesne býrakýldýðýnda zoom'un eski haline dönme hýzý.")]
    [SerializeField] private float zoomResetSpeed = 2f;

    // --- Özel Deðiþkenler ---
    private bool isZoomActive = false; // Zoom'un aktif olup olmadýðýný kontrol eden bayrak
    private float initialFov; // Kameranýn baþlangýçtaki FOV deðeri
    private Coroutine resetCoroutine; // Zoom resetleme coroutine'ini tutmak için

    private void Awake()
    {
        // Eðer kamera editörden atanmamýþsa, sahnedeki ana kamerayý bulmayý dene.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            // Baþlangýç FOV deðerini kaydet
            initialFov = mainCamera.fieldOfView;
            // Eðer maxFov kullanýcý tarafýndan ayarlanmadýysa, baþlangýç deðerini ata
            if (maxFov <= 0)
            {
                maxFov = initialFov;
            }
        }
        else
        {
            Debug.LogError("Zoom yapýlacak bir kamera bulunamadý! Lütfen Main Camera referansýný atayýn.");
            this.enabled = false; // Script'i devre dýþý býrak
        }
    }

    private void Update()
    {
        // Eðer zoom aktif deðilse, Update fonksiyonunda hiçbir þey yapma.
        if (!isZoomActive)
        {
            return;
        }

        // Joystick'in dikey (Y) eksenindeki deðerini oku (-1 ile 1 arasýnda)
        float joystickValue = zoomAction.action.ReadValue<Vector2>().y;

        // Mevcut FOV deðerini, joystick hareketine göre deðiþtir.
        // Joystick ileri (pozitif Y) itildiðinde FOV küçülmeli (zoom in), bu yüzden deðeri çýkartýyoruz.
        float newFov = mainCamera.fieldOfView - (joystickValue * zoomSpeed * Time.deltaTime);

        // Yeni FOV deðerini min ve max limitleri arasýnda sýkýþtýr.
        mainCamera.fieldOfView = Mathf.Clamp(newFov, minFov, maxFov);
    }

    /// <summary>
    /// Bu metod, montajlanabilir bir nesne tutulduðunda çaðrýlýr. Zoom'u aktif eder.
    /// XR Grab Interactable'ýn "Select Entered" event'inden çaðrýlmalýdýr.
    /// </summary>
    public void ActivateZoom()
    {
        // Eðer varsa, devam eden zoom resetleme iþlemini durdur.
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        isZoomActive = true;
        Debug.Log("Zoom Aktif!");
    }

    /// <summary>
    /// Bu metod, montajlanabilir nesne býrakýldýðýnda çaðrýlýr. Zoom'u devre dýþý býrakýr
    /// ve kamerayý yumuþakça baþlangýç FOV deðerine döndürür.
    /// XR Grab Interactable'ýn "Select Exited" event'inden çaðrýlmalýdýr.
    /// </summary>
    public void DeactivateZoom()
    {
        isZoomActive = false;
        // Kamerayý baþlangýç FOV deðerine yumuþakça döndürmek için Coroutine'i baþlat.
        resetCoroutine = StartCoroutine(ResetZoom());
        Debug.Log("Zoom Devre Dýþý! Resetleniyor...");
    }

    /// <summary>
    /// Kameranýn Field of View'ýný baþlangýç deðerine yumuþak bir geçiþle (Lerp) döndürür.
    /// </summary>
    private IEnumerator ResetZoom()
    {
        float currentFov = mainCamera.fieldOfView;
        float elapsedTime = 0f;

        // FOV deðeri baþlangýç deðerine çok yaklaþana kadar dön.
        while (!Mathf.Approximately(mainCamera.fieldOfView, initialFov))
        {
            elapsedTime += Time.deltaTime * zoomResetSpeed;
            mainCamera.fieldOfView = Mathf.Lerp(currentFov, initialFov, elapsedTime);
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Tam olarak baþlangýç deðerine ayarla ve coroutine'i sonlandýr.
        mainCamera.fieldOfView = initialFov;
        resetCoroutine = null;
    }
}