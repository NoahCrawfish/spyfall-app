using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingElementsController : MonoBehaviour
{
    [SerializeField] GameObject blurPanel;
    [SerializeField] Button saveButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button resetButton;
    [SerializeField] List<Button> paletteButtons;

    public ManageCustomizeLocationScreen Manager { get; set; }

    public void Initialize() {
        if (Manager.SettingsUI.TempImage != null) {
            GetComponent<DrawingUI>().Drawing = Manager.SettingsUI.GetTempImage();
        } else {
            GetComponent<DrawingUI>().CreateBlankImage();
        }

        ManageAudio.Instance.Play("drawing", true);
        ManageAudio.Instance.Mute("drawing", false);
        ShowDrawingElements();
    }

    private void ShowDrawingElements() {
        GetComponent<CanvasGroupFade>().DoFade(CanvasGroupFade.Fade.fadeIn, () => SetElementsActive(true));
        blurPanel.GetComponent<BlurController>().DoBlur(BlurController.Fade.fadeIn);
    }

    private void SetElementsActive(bool isActive) {
        saveButton.interactable = cancelButton.interactable = resetButton.interactable = isActive;
        paletteButtons.ForEach(button => button.interactable = isActive);
        paletteButtons[0].Select();
        GetComponent<DrawingUI>().CanDraw = isActive;
        if (!isActive) {
            gameObject.SetActive(false);
        }
    }

    // called from save button onclick in inspector
    public void SaveDrawing() {
        Texture2D drawing = GetComponent<DrawingUI>().Drawing;
        Manager.SettingsUI.SetTempImage(drawing);
        Manager.RefreshPreviewImage();
        FadeOut();
    }

    public void FadeOut() {
        ManageAudio.Instance.Stop("drawing");
        GetComponent<CanvasGroupFade>().DoFade(CanvasGroupFade.Fade.fadeOut, () => SetElementsActive(false));
        blurPanel.GetComponent<BlurController>().DoBlur(BlurController.Fade.fadeOut);
    }
}
