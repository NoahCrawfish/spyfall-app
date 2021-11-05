using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasePopupController : MonoBehaviour
{
    private ManageGame manageGame;
    [SerializeField] private GameObject blurPanel;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private List<Selectable> selectables = new List<Selectable>();
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI removeAdsText;
    [SerializeField] private GameObject restorePurchasesButton;

    public enum ButtonMessage {
        unlock,
        unavailable,
        whoops
    }

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
    }

    private void OnEnable() {
        SetButtonText(ButtonMessage.unlock);
        ShowPopup();

        #if UNITY_IOS
            restorePurchasesButton.SetActive(true);
        #endif
    }

    private void ShowPopup() {
        blurPanel.GetComponent<BlurController>().DoBlur(BlurController.Fade.fadeIn);
        popupPanel.GetComponent<CanvasGroupFade>().DoFade(CanvasGroupFade.Fade.fadeIn, () => selectables.ForEach(selectable => selectable.interactable = true));

        if (removeAdsText != null) {
            SetButtonFlash(removeAdsText, false);
        }
    }

    // called from close button in inspector
    public void ClosePopup() {
        selectables.ForEach(selectable => selectable.interactable = false);
        blurPanel.GetComponent<BlurController>().DoBlur(BlurController.Fade.fadeOut);
        popupPanel.GetComponent<CanvasGroupFade>().DoFade(CanvasGroupFade.Fade.fadeOut, () => gameObject.SetActive(false));

        if (removeAdsText != null) {
            SetButtonFlash(removeAdsText, true);
        }
    }

    // called from purchase button in inspector
    public void MakePurchase() {
        manageGame.IapManager.PurchaseProVersion();
    }

    public void SetButtonText(ButtonMessage message) {
        buttonText.text = message switch {
            ButtonMessage.unlock => "UNLOCK",
            ButtonMessage.unavailable => "Purchasing currently unavailable. Check your internet connection and try again.",
            ButtonMessage.whoops => "Whoops, something went wrong. Try again later.",
            _ => "UNLOCK"
        };

        SetButtonFlash(buttonText, message != ButtonMessage.unavailable && message != ButtonMessage.whoops);
    }

    private void SetButtonFlash(TextMeshProUGUI text, bool flashing) {
        FlashText flash = text.GetComponent<FlashText>();

        if (flashing) {
            flash.enabled = true;
        } else {
            flash.ResetT();
            text.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 1);
            flash.enabled = false;
        }
    }
}
