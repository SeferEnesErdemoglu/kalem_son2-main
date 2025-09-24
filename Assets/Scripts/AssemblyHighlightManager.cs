using UnityEngine;

/// <summary>
/// Bu script, montajlanabilir par�alar�n tutulup tutulmad���n� takip eder
/// ve durumlar�na g�re onlara g�rsel bir highlight (parlama) efekti uygular.
/// </summary>
public class AssemblyHighlightManager : MonoBehaviour
{
    [Header("Montajlanabilir Par�alar")]
    [Tooltip("Sahnedeki Kalem objesinin referans�.")]
    [SerializeField] private Renderer penRenderer;

    [Tooltip("Sahnedeki Kapak objesinin referans�.")]
    [SerializeField] private Renderer capRenderer;

    [Header("Highlight Renkleri")]
    [Tooltip("Tek bir par�a tutuldu�unda uygulanacak ���ma rengi.")]
    [SerializeField] private Color singleHoldColor = Color.yellow;

    [Tooltip("�ki par�a ayn� anda tutuldu�unda uygulanacak ���ma rengi.")]
    [SerializeField] private Color bothHoldColor = new Color(1.0f, 0.5f, 0.0f); // Turuncu

    // --- �zel De�i�kenler ---
    private Material penMaterial;
    private Material capMaterial;
    private Color defaultEmissionColor = Color.black; // I��man�n kapal� oldu�u varsay�lan renk

    private bool isPenHeld = false;
    private bool isCapHeld = false;

    private void Start()
    {
        // Null kontrol�
        if (penRenderer == null || capRenderer == null)
        {
            Debug.LogError("L�tfen Kalem ve Kapak Renderer referanslar�n� Inspector'dan atay�n!");
            this.enabled = false;
            return;
        }

        // Materyallerin birer kopyas�n� olu�turarak orijinal materyal asset'lerini bozmuyoruz.
        penMaterial = penRenderer.material;
        capMaterial = capRenderer.material;

        // Materyallerin Emission �zelli�ini aktif hale getiriyoruz.
        penMaterial.EnableKeyword("_EMISSION");
        capMaterial.EnableKeyword("_EMISSION");

        // Ba�lang��ta highlight'lar� kapat.
        UpdateHighlights();
    }

    // Bu metod, Kalem tutuldu�unda �a�r�lacak.
    public void OnPenGrabbed()
    {
        isPenHeld = true;
        UpdateHighlights();
    }

    // Bu metod, Kalem b�rak�ld���nda �a�r�lacak.
    public void OnPenReleased()
    {
        isPenHeld = false;
        UpdateHighlights();
    }

    // Bu metod, Kapak tutuldu�unda �a�r�lacak.
    public void OnCapGrabbed()
    {
        isCapHeld = true;
        UpdateHighlights();
    }

    // Bu metod, Kapak b�rak�ld���nda �a�r�lacak.
    public void OnCapReleased()
    {
        isCapHeld = false;
        UpdateHighlights();
    }

    /// <summary>
    /// Tutulma durumlar�na g�re her iki objenin de highlight rengini g�nceller.
    /// </summary>
    private void UpdateHighlights()
    {
        if (isPenHeld && isCapHeld) // �kisi de tutuluyorsa
        {
            SetHighlight(penMaterial, bothHoldColor);
            SetHighlight(capMaterial, bothHoldColor);
        }
        else if (isPenHeld) // Sadece kalem tutuluyorsa
        {
            SetHighlight(penMaterial, singleHoldColor);
            SetHighlight(capMaterial, defaultEmissionColor); // Di�erini kapat
        }
        else if (isCapHeld) // Sadece kapak tutuluyorsa
        {
            SetHighlight(penMaterial, defaultEmissionColor); // Di�erini kapat
            SetHighlight(capMaterial, singleHoldColor);
        }
        else // Hi�biri tutulmuyorsa
        {
            SetHighlight(penMaterial, defaultEmissionColor);
            SetHighlight(capMaterial, defaultEmissionColor);
        }
    }

    /// <summary>
    /// Verilen materyalin ���ma rengini ayarlar.
    /// </summary>
    private void SetHighlight(Material mat, Color color)
    {
        mat.SetColor("_EmissionColor", color);
    }
}