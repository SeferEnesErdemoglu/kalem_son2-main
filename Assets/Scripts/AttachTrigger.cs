using UnityEngine;

// Bu script, trigger'a giren AssemblingPart'ý ana scripte bildirir.
public class AttachTrigger : MonoBehaviour
{
    private AssemblingPart mainPartScript;

    void Start()
    {
        mainPartScript = GetComponentInParent<AssemblingPart>();
    }

    private void OnTriggerEnter(Collider other)
    {
        AssemblingPart enteringPart = other.GetComponent<AssemblingPart>();
        if (enteringPart != null)
        {
            // Diðer parçaya, "bana baðlanmayý dene" komutunu gönder
            enteringPart.TryToAttach(mainPartScript);
        }
    }
}