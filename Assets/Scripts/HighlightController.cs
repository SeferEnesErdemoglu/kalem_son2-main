using UnityEngine;

/// <summary>
/// Bu script, bir objenin materyalini belirlenen bir highlight materyali ile deðiþtirmeyi
/// ve sonra tekrar orijinal haline döndürmeyi saðlar.
/// XR Interactable events (Hover veya Select) tarafýndan tetiklenmek için tasarlanmýþtýr.
/// </summary>
public class HighlightController : MonoBehaviour
{
    [Tooltip("Obje highlight olduðunda kullanýlacak materyal.")]
    [SerializeField] private Material highlightMaterial;

    // --- Özel Deðiþkenler ---
    private Renderer objectRenderer; // Objenin Mesh Renderer'ýný tutar
    private Material originalMaterial; // Objenin baþlangýçtaki orijinal materyalini saklar

    private void Awake()
    {
        // Objenin Renderer component'ini bul.
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("Bu objede bir Renderer component'i bulunamadý!", this);
            this.enabled = false;
            return;
        }

        // Baþlangýçtaki materyali hafýzaya al.
        originalMaterial = objectRenderer.material;

        // Eðer highlight materyali atanmamýþsa hata ver.
        if (highlightMaterial == null)
        {
            Debug.LogError("Highlight Materyali atanmamýþ!", this);
            this.enabled = false;
        }
    }

    /// <summary>
    /// Highlight'ý AÇAR. Objenin materyalini highlight materyali ile deðiþtirir.
    /// XR Grab Interactable'ýn "Select Entered" event'inden çaðrýlabilir.
    /// </summary>
    public void ActivateHighlight()
    {
        if (highlightMaterial != null && objectRenderer != null)
        {
            objectRenderer.material = highlightMaterial;
        }
    }

    /// <summary>
    /// Highlight'ý KAPATIR. Objenin materyalini orijinal materyaline döndürür.
    /// XR Grab Interactable'ýn "Select Exited" event'inden çaðrýlabilir.
    /// </summary>
    public void DeactivateHighlight()
    {
        if (originalMaterial != null && objectRenderer != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }
}