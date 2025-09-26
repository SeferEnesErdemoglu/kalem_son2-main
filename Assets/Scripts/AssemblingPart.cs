using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class AssemblingPart : MonoBehaviour
{
    [Header("Yuva Ayarları (Sadece Yuva Olan Parçalar İçin)")]
    [Tooltip("Bu parçaya başka bir parçanın kenetleneceği hedef nokta.")]
    [SerializeField] private Transform snapPoint;
    [Tooltip("Bu yuvaya sadece bu etikete sahip objeler kenetlenebilir.")]
    [SerializeField] private string targetTag = "Pen";

    [Header("Takılma Ayarları (Sadece Takılan Parçalar İçin)")]
    [Tooltip("Bu parçanın, bir yuvaya hizalanacak olan kendi referans noktası. Boş bırakılırsa, objenin merkezi kullanılır.")]
    [SerializeField] private Transform selfSnapTarget;

    // --- Durum Değişkenleri ---
    private bool isAttached = false;
    private AssemblingPart attachedPart = null; // Takılı olan parçanın referansını tutar
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// AttachTrigger tarafından çağrılır. Bu parçanın, verilen yuvaya kenetlenmesini dener.
    /// </summary>
    public void TryToAttach(AssemblingPart socketPart)
    {
        if (isAttached || socketPart == null || !socketPart.CanAttach(this.tag))
        {
            return;
        }

        if (grabInteractable.isSelected)
        {
            grabInteractable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grabInteractable);
        }
        grabInteractable.enabled = false;

        // --- FİZİKSEL TİTREME DÜZELTMESİ ---
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.Sleep();
        }

        // --- HASSAS HİZALAMA MANTIĞI ---
        Transform targetSnapPoint = socketPart.snapPoint;
        if (selfSnapTarget != null)
        {
            Quaternion rotationDifference = Quaternion.Inverse(selfSnapTarget.localRotation);
            transform.rotation = targetSnapPoint.rotation * rotationDifference;

            Vector3 positionDifference = transform.position - selfSnapTarget.position;
            transform.position = targetSnapPoint.position + positionDifference;
        }
        else
        {
            transform.position = targetSnapPoint.position;
            transform.rotation = targetSnapPoint.rotation;
        }

        transform.SetParent(socketPart.transform);
        isAttached = true;
        socketPart.MarkAsAttached(this); // Yuvanın dolduğunu ve hangi parçanın takıldığını bildir.

        Debug.Log(this.name + ", " + socketPart.name + " yuvasına takıldı!");
    }

    /// <summary>
    /// MontajInputManager tarafından çağrılır. Bu parça bir yuvaysa, takılı olan parçayı ayırır.
    /// </summary>
    public void Detach()
    {
        if (snapPoint == null || !isAttached || attachedPart == null)
        {
            return;
        }

        Debug.Log(attachedPart.name + " ayrılıyor...");

        AssemblingPart partToDetach = attachedPart;
        XRGrabInteractable partGrabInteractable = partToDetach.GetComponent<XRGrabInteractable>();
        Rigidbody partRigidbody = partToDetach.GetComponent<Rigidbody>();

        partToDetach.transform.SetParent(null);

        if (partRigidbody != null)
        {
            partRigidbody.isKinematic = false;
            partRigidbody.detectCollisions = true;
            partRigidbody.WakeUp();
            partRigidbody.AddForce(transform.up * 0.5f, ForceMode.Impulse);
        }

        if (partGrabInteractable != null)
        {
            partGrabInteractable.enabled = true;
        }

        partToDetach.ResetAttachmentState();
        this.ResetAttachmentState();
    }

    /// <summary>
    /// MontajInputManager tarafından çağrılır.
    /// Eğer bu parça bir yuvaya takılıysa, kendisini ebeveyninden (yuvadan) ayırır.
    /// </summary>
    public void TryDetachFromParent()
    {
        // Eğer bir yere takılı değilsem veya bir ebeveynim yoksa, hiçbir şey yapma.
        if (!isAttached || transform.parent == null) return;

        // Ebeveynimdeki (yuva olan parça) AssemblingPart script'ini bul.
        AssemblingPart socketPart = transform.parent.GetComponent<AssemblingPart>();
        if (socketPart != null)
        {
            // Ayırma işlemini ebeveyn (yuva) üzerinden başlat.
            socketPart.Detach();
        }
    }

    /// <summary>
    /// Bu yuvanın, belirtilen etikete sahip bir parçayı kabul edip edemeyeceğini kontrol eder.
    /// </summary>
    public bool CanAttach(string enteringTag)
    {
        if (snapPoint == null || isAttached || enteringTag != targetTag)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Bu yuvanın artık dolu olduğunu ve hangi parçanın takıldığını işaretler.
    /// </summary>
    public void MarkAsAttached(AssemblingPart part)
    {
        isAttached = true;
        attachedPart = part;
    }

    /// <summary>
    /// Hem yuvanın hem de ayrılan parçanın takılma durumunu sıfırlar.
    /// </summary>
    public void ResetAttachmentState()
    {
        isAttached = false;
        attachedPart = null;
    }

    /// <summary>
    /// Bu parçanın zaten bir yuvaya takılı olup olmadığını döndürür.
    /// </summary>
    public bool IsAlreadyAttached()
    {
        return isAttached;
    }
}