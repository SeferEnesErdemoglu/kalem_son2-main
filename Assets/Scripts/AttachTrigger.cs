using UnityEngine;

// Bu script, trigger'a giren AssemblingPart'ý ana scripte bildirir.
public class AttachTrigger : MonoBehaviour
{
    private AssemblingPart mainPartScript;

    void Start()
    {
        // Kendi ebeveynindeki ana script'i bul.
        mainPartScript = GetComponentInParent<AssemblingPart>();
        if (mainPartScript == null)
        {
            Debug.LogError("AttachTrigger, ebeveyninde bir AssemblingPart script'i bulamadý!", this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mainPartScript == null) return;

        AssemblingPart enteringPart = other.GetComponent<AssemblingPart>();

        // --- YENÝ GÜVENLÝK KONTROLÜ ---
        // Eðer giren parça geçerliyse VE henüz bir yere takýlmamýþsa devam et.
        if (enteringPart != null && !enteringPart.IsAlreadyAttached())
        {
            // Diðer parçaya, "bana baðlanmayý dene" komutunu gönder
            enteringPart.TryToAttach(mainPartScript);
        }
    }
}