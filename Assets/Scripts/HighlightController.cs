using UnityEngine;

/// <summary>
/// Bu script, bir objenin materyalini belirlenen bir highlight materyali ile de�i�tirmeyi
/// ve sonra tekrar orijinal haline d�nd�rmeyi sa�lar.
/// XR Interactable events (Hover veya Select) taraf�ndan tetiklenmek i�in tasarlanm��t�r.
/// </summary>
public class HighlightController : MonoBehaviour
{
    [Tooltip("Obje highlight oldu�unda kullan�lacak materyal.")]
    [SerializeField] private Material highlightMaterial;

    // --- �zel De�i�kenler ---
    private Renderer objectRenderer; // Objenin Mesh Renderer'�n� tutar
    private Material originalMaterial; // Objenin ba�lang��taki orijinal materyalini saklar

    private void Awake()
    {
        // Objenin Renderer component'ini bul.
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("Bu objede bir Renderer component'i bulunamad�!", this);
            this.enabled = false;
            return;
        }

        // Ba�lang��taki materyali haf�zaya al.
        originalMaterial = objectRenderer.material;

        // E�er highlight materyali atanmam��sa hata ver.
        if (highlightMaterial == null)
        {
            Debug.LogError("Highlight Materyali atanmam��!", this);
            this.enabled = false;
        }
    }

    /// <summary>
    /// Highlight'� A�AR. Objenin materyalini highlight materyali ile de�i�tirir.
    /// XR Grab Interactable'�n "Select Entered" event'inden �a�r�labilir.
    /// </summary>
    public void ActivateHighlight()
    {
        if (highlightMaterial != null && objectRenderer != null)
        {
            objectRenderer.material = highlightMaterial;
        }
    }

    /// <summary>
    /// Highlight'� KAPATIR. Objenin materyalini orijinal materyaline d�nd�r�r.
    /// XR Grab Interactable'�n "Select Exited" event'inden �a�r�labilir.
    /// </summary>
    public void DeactivateHighlight()
    {
        if (originalMaterial != null && objectRenderer != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }
}