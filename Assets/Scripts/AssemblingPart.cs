using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Bu script, iki par�an�n birbirine tak�l�p s�k�lebildi�i bir montaj sistemi y�netir.
/// Par�alar birle�ti�inde kilitlenir, iki elle tutuldu�unda ise ayr�l�r.
/// </summary>
public class AssemblingPart : MonoBehaviour
{
    [Header("Montaj Ayarlar�")]
    [Tooltip("Bu par�an�n ba�lanaca�� di�er par�a.")]
    public AssemblingPart otherPart;

    [Tooltip("Bu par�a, di�er par�an�n HANG� NOKTASINA kilitlenecek? (Sadece bir par�ada ayarlanmal�, �rn: Kalemde)")]
    public Transform attachPoint;

    // --- Durum De�i�kenleri ---
    private bool isAttached = false;
    private Transform originalParent;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    void Start()
    {
        // Gerekli component'leri al ve sakla
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        originalParent = transform.parent; // Ba�lang��taki parent'� kaydet

        // Grab event'ine bir listener (dinleyici) ekle
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    // Bu obje tutuldu�unda XR Grab Interactable taraf�ndan �a�r�l�r
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // E�er bu par�a bir yere tak�l�ysa VE di�er par�a da �u an tutuluyorsa...
        if (isAttached && otherPart.grabInteractable.isSelected)
        {
            // �ki elle tutma durumu alg�land�, par�alar� ay�r!
            Detach();
        }
    }

    // Bu metot, di�er par�an�n trigger'�na girildi�inde �a�r�l�r.
    public void TryToAttach(AssemblingPart partToAttachTo)
    {
        // E�er zaten tak�l� de�ilse ve do�ru par�aya temas ediyorsa
        if (!isAttached && partToAttachTo == otherPart)
        {
            Attach(partToAttachTo);
        }
    }

    // Par�alar� birbirine kilitleyen metot
    private void Attach(AssemblingPart parentPart)
    {
        Debug.Log(gameObject.name + " tak�ld�!");

        // Fiziksel �ak��malar� �nlemek i�in Rigidbody'i kinematik yap
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Bu par�ay�, di�er par�an�n �ocu�u (child) yap
        transform.SetParent(parentPart.transform);

        // Di�er par�an�n belirledi�i montaj noktas�na kendini konumland�r ve d�nd�r
        transform.position = parentPart.attachPoint.position;
        transform.rotation = parentPart.attachPoint.rotation;

        // Her iki par�an�n da "tak�l�" durumunu g�ncelle
        isAttached = true;
        parentPart.isAttached = true;
    }

    // Par�alar� birbirinden ay�ran metot
    private void Detach()
    {
        Debug.Log(gameObject.name + " ayr�ld�!");

        // Rigidbody'i tekrar normal fizik kurallar�na d�nd�r
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Parent ili�kisini kopar ve ba�lang��taki parent'a geri d�n
        transform.SetParent(originalParent);

        // Her iki par�an�n da "tak�l�" durumunu g�ncelle
        isAttached = false;
        otherPart.isAttached = false;
    }
}