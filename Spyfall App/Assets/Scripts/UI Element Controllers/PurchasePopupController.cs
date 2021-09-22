using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePopupController : MonoBehaviour
{
    private ManageGame manageGame;
    [SerializeField] private GameObject blurPanel;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private List<Selectable> selectables = new List<Selectable>();

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
    }

    private void OnEnable() {
        ShowPopup();
    }

    private void ShowPopup() {
        blurPanel.GetComponent<BlurController>().DoBlur(BlurController.Fade.fadeIn);
        popupPanel.GetComponent<CanvasGroupFade>().DoFade(CanvasGroupFade.Fade.fadeIn, () => selectables.ForEach(selectable => selectable.interactable = true));
    }

    // called from close button in inspector
    public void ClosePopup() {
        selectables.ForEach(selectable => selectable.interactable = false);
        blurPanel.GetComponent<BlurController>().DoBlur(BlurController.Fade.fadeOut);
        popupPanel.GetComponent<CanvasGroupFade>().DoFade(CanvasGroupFade.Fade.fadeOut, () => gameObject.SetActive(false));
    }

    // called from purchase button in inspector
    public void MakePurchase() {
        manageGame.IapManager.PurchaseProVersion();
    }
}
