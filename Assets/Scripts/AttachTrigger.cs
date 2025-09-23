using UnityEngine;

// Bu script, trigger'a giren AssemblingPart'� ana scripte bildirir.
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
            // Di�er par�aya, "bana ba�lanmay� dene" komutunu g�nder
            enteringPart.TryToAttach(mainPartScript);
        }
    }
}