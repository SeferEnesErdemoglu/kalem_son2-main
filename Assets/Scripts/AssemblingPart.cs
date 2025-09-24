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
    [SerializeField] private Transform selfSnapTarget; // YENİ EKLENEN ALAN

    // --- Durum Değişkenleri ---
    private bool isAttached = false;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }
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
        // Rigidbody'yi ve Collider'ı devre dışı bırakarak fiziksel çatışmayı tamamen bitiriyoruz.
        if (rb != null)
        {
            // Rigidbody'yi Kinematik yap (dış kuvvetlerden etkilenmesin).
            rb.isKinematic = true;
            // Rigidbody'nin tüm çarpışma kontrollerini durdur.
            rb.detectCollisions = false;
            // Rigidbody'yi tamamen uyku moduna al.
            rb.Sleep();
        }

        // İsteğe bağlı: Eğer hala sorun devam ederse, bu satırı da ekleyebilirsiniz.
        // GetComponent<Collider>().enabled = false;

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
        socketPart.MarkAsAttached();

        Debug.Log(this.name + ", " + socketPart.name + " yuvasına takıldı!");
    }
    // ... (kodun geri kalanı aynı) ...
    public bool CanAttach(string enteringTag)
    {
        if (snapPoint == null || isAttached || enteringTag != targetTag)
        {
            return false;
        }
        return true;
    }

    public void MarkAsAttached()
    {
        isAttached = true;
    }

    public bool IsAlreadyAttached()
    {
        return isAttached;
    }
}