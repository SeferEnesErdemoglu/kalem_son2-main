using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class MontajInputManager : MonoBehaviour
{
    [Header("Kontrol Edilecek Sistemler")]
    [Tooltip("Tüm hareket component'lerini içeren ana GameObject.")]
    [SerializeField] private GameObject locomotionSystem;
    [Tooltip("Zoom mantığını içeren script.")]
    [SerializeField] private AssemblyZoomController zoomController;

    [Header("El Etkileşimcileri (Interactors)")]
    [SerializeField] private XRBaseInteractor leftHand;
    [SerializeField] private XRBaseInteractor rightHand;

    [Header("Montaj Eylemleri")]
    [SerializeField] private InputActionProperty detachAction;

    private int montajParcasiSayaci = 0;

    private void Awake()
    {
        if (zoomController != null) zoomController.enabled = false;
        if (locomotionSystem != null) locomotionSystem.SetActive(true);
        detachAction.action.performed += _ => AyirmaIsleminiDene();
    }

    private void OnEnable()
    {
        leftHand.selectEntered.AddListener(OnParcaSecildi);
        rightHand.selectEntered.AddListener(OnParcaSecildi);
        leftHand.selectExited.AddListener(OnParcaBirakildi);
        rightHand.selectExited.AddListener(OnParcaBirakildi);
        detachAction.action.Enable();
    }

    private void OnDisable()
    {
        leftHand.selectEntered.RemoveListener(OnParcaSecildi);
        rightHand.selectEntered.RemoveListener(OnParcaSecildi);
        leftHand.selectExited.RemoveListener(OnParcaBirakildi);
        rightHand.selectExited.RemoveListener(OnParcaBirakildi);
        detachAction.action.Disable();
    }

    private void OnParcaSecildi(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.GetComponent<AssemblingPart>() != null)
        {
            montajParcasiSayaci++;
            GuncelleSistemDurumlarini();
        }
    }

    private void OnParcaBirakildi(SelectExitEventArgs args)
    {
        if (args.interactableObject != null && args.interactableObject.transform.GetComponent<AssemblingPart>() != null)
        {
            montajParcasiSayaci--;
            GuncelleSistemDurumlarini();
        }
    }

    private void GuncelleSistemDurumlarini()
    {
        bool hareketAktif = (montajParcasiSayaci == 0);
        if (locomotionSystem != null) locomotionSystem.SetActive(hareketAktif);

        bool zoomAktif = (montajParcasiSayaci == 2);
        if (zoomController != null)
        {
            // --- DOĞRU YÖNTEM BURASI ---
            // 'SetZoomActive' yerine component'i direkt açıp kapatıyoruz.
            zoomController.enabled = zoomAktif;

            if (!zoomAktif)
            {
                zoomController.ResetZoom();
            }
        }
    }

    private void AyirmaIsleminiDene()
    {
        XRBaseInteractor aktifEl = null;
        if (rightHand.hasSelection) aktifEl = rightHand;
        else if (leftHand.hasSelection) aktifEl = leftHand;

        if (aktifEl == null) return;

        var interactable = aktifEl.interactablesSelected.Count > 0 ? aktifEl.interactablesSelected[0] : null;
        if (interactable == null) return;

        AssemblingPart part = (interactable as MonoBehaviour)?.GetComponent<AssemblingPart>();
        if (part != null)
        {
            part.TryDetachFromParent();
        }
    }
}