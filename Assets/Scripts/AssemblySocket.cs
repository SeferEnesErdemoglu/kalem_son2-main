using UnityEngine;


[RequireComponent(typeof(Collider))]
public class AssemblySocket : MonoBehaviour
{
    [Header("Kenetlenme Ayarları")]
    [SerializeField] private Transform snapPoint;
    [SerializeField] private string targetTag = "Pen";

    private bool isSnapped = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSnapped || !other.CompareTag(targetTag))
        {
            return;
        }

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            if (grabInteractable.isSelected)
            {
                // --- DÜZELTİLMİŞ SATIR ---
                // IXRSelectInteractable arayüzünü kullanarak seçimi iptal ediyoruz.
                grabInteractable.interactionManager.CancelInteractableSelection((UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable);
            }
            grabInteractable.enabled = false;
        }

        Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
        if (otherRigidbody != null)
        {
            otherRigidbody.isKinematic = true;
        }

        other.transform.SetParent(this.transform);
        other.transform.position = snapPoint.position;
        other.transform.rotation = snapPoint.rotation;

        isSnapped = true;

        Debug.Log(other.name + " objesi " + this.name + " objesine kenetlendi!");
    }
}