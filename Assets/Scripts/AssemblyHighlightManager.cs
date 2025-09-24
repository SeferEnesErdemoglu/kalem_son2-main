using UnityEngine;

/// <summary>
/// Bu script, montajlanabilir parçalarýn tutulup tutulmadýðýný takip eder
/// ve durumlarýna göre onlara görsel bir highlight (parlama) efekti uygular.
/// </summary>
public class AssemblyHighlightManager : MonoBehaviour
{
    [Header("Montajlanabilir Parçalar")]
    [Tooltip("Sahnedeki Kalem objesinin referansý.")]
    [SerializeField] private Renderer penRenderer;

    [Tooltip("Sahnedeki Kapak objesinin referansý.")]
    [SerializeField] private Renderer capRenderer;

    [Header("Highlight Renkleri")]
    [Tooltip("Tek bir parça tutulduðunda uygulanacak ýþýma rengi.")]
    [SerializeField] private Color singleHoldColor = Color.yellow;

    [Tooltip("Ýki parça ayný anda tutulduðunda uygulanacak ýþýma rengi.")]
    [SerializeField] private Color bothHoldColor = new Color(1.0f, 0.5f, 0.0f); // Turuncu

    // --- Özel Deðiþkenler ---
    private Material penMaterial;
    private Material capMaterial;
    private Color defaultEmissionColor = Color.black; // Iþýmanýn kapalý olduðu varsayýlan renk

    private bool isPenHeld = false;
    private bool isCapHeld = false;

    private void Start()
    {
        // Null kontrolü
        if (penRenderer == null || capRenderer == null)
        {
            Debug.LogError("Lütfen Kalem ve Kapak Renderer referanslarýný Inspector'dan atayýn!");
            this.enabled = false;
            return;
        }

        // Materyallerin birer kopyasýný oluþturarak orijinal materyal asset'lerini bozmuyoruz.
        penMaterial = penRenderer.material;
        capMaterial = capRenderer.material;

        // Materyallerin Emission özelliðini aktif hale getiriyoruz.
        penMaterial.EnableKeyword("_EMISSION");
        capMaterial.EnableKeyword("_EMISSION");

        // Baþlangýçta highlight'larý kapat.
        UpdateHighlights();
    }

    // Bu metod, Kalem tutulduðunda çaðrýlacak.
    public void OnPenGrabbed()
    {
        isPenHeld = true;
        UpdateHighlights();
    }

    // Bu metod, Kalem býrakýldýðýnda çaðrýlacak.
    public void OnPenReleased()
    {
        isPenHeld = false;
        UpdateHighlights();
    }

    // Bu metod, Kapak tutulduðunda çaðrýlacak.
    public void OnCapGrabbed()
    {
        isCapHeld = true;
        UpdateHighlights();
    }

    // Bu metod, Kapak býrakýldýðýnda çaðrýlacak.
    public void OnCapReleased()
    {
        isCapHeld = false;
        UpdateHighlights();
    }

    /// <summary>
    /// Tutulma durumlarýna göre her iki objenin de highlight rengini günceller.
    /// </summary>
    private void UpdateHighlights()
    {
        if (isPenHeld && isCapHeld) // Ýkisi de tutuluyorsa
        {
            SetHighlight(penMaterial, bothHoldColor);
            SetHighlight(capMaterial, bothHoldColor);
        }
        else if (isPenHeld) // Sadece kalem tutuluyorsa
        {
            SetHighlight(penMaterial, singleHoldColor);
            SetHighlight(capMaterial, defaultEmissionColor); // Diðerini kapat
        }
        else if (isCapHeld) // Sadece kapak tutuluyorsa
        {
            SetHighlight(penMaterial, defaultEmissionColor); // Diðerini kapat
            SetHighlight(capMaterial, singleHoldColor);
        }
        else // Hiçbiri tutulmuyorsa
        {
            SetHighlight(penMaterial, defaultEmissionColor);
            SetHighlight(capMaterial, defaultEmissionColor);
        }
    }

    /// <summary>
    /// Verilen materyalin ýþýma rengini ayarlar.
    /// </summary>
    private void SetHighlight(Material mat, Color color)
    {
        mat.SetColor("_EmissionColor", color);
    }
}