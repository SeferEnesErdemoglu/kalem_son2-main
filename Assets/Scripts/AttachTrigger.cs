using UnityEngine;

// Bu script, trigger'a giren AssemblingPart'� ana scripte bildirir.
public class AttachTrigger : MonoBehaviour
{
    private AssemblingPart mainPartScript;

    void Start()
    {
        // Kendi ebeveynindeki ana script'i bul.
        mainPartScript = GetComponentInParent<AssemblingPart>();
        if (mainPartScript == null)
        {
            Debug.LogError("AttachTrigger, ebeveyninde bir AssemblingPart script'i bulamad�!", this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mainPartScript == null) return;

        AssemblingPart enteringPart = other.GetComponent<AssemblingPart>();

        // --- YEN� G�VENL�K KONTROL� ---
        // E�er giren par�a ge�erliyse VE hen�z bir yere tak�lmam��sa devam et.
        if (enteringPart != null && !enteringPart.IsAlreadyAttached())
        {
            // Di�er par�aya, "bana ba�lanmay� dene" komutunu g�nder
            enteringPart.TryToAttach(mainPartScript);
        }
    }
}