using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DrawingElementsController : MonoBehaviour
{
    [SerializeField] GameObject blurPanel;
    [SerializeField] Button saveButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button resetButton;
    [SerializeField] List<Button> paletteButtons;

    private readonly float[] blurEndpoints = { 0f, 3f };
    private readonly Color[] blurTintEndpoints = { new Color(1, 1, 1), new Color(118f / 255f, 118f / 255f, 118f / 255f) };
    private readonly float blurSpeed = 0.1f;

    private delegate void AfterEnumerator();
    private CanvasGroup canvasGroup;
    public ManageCustomizeLocationScreen Manager { get; set; }

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize() {
        if (Manager.SettingsUI.TempImage != null) {
            GetComponent<DrawingUI>().Drawing = Manager.SettingsUI.GetTempImage();
        } else {
            GetComponent<DrawingUI>().CreateBlankImage();
        }

        ShowDrawingElements();
    }

    private void ShowDrawingElements() {
        StartCoroutine(CanvasGroupFade(true, canvasGroup, blurSpeed, () => SetElementsActive(true)));
        StartCoroutine(BlurFade(true, blurSpeed));
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

    public void SaveDrawing() {
        Texture2D drawing = GetComponent<DrawingUI>().Drawing;
        Manager.SettingsUI.SetTempImage(drawing); // DONT SET TEMP IMAGE UNTIL CUSTOMIZELOCATIONSCREEN DONE BUTTON IS PRESSED
        Manager.RefreshPreviewImage();
        FadeOut();
    }

    public void FadeOut() {
        StartCoroutine(CanvasGroupFade(false, canvasGroup, blurSpeed, () => SetElementsActive(false)));
        StartCoroutine(BlurFade(false, blurSpeed));
    }


    private IEnumerator CanvasGroupFade(bool fadeIn, CanvasGroup canvasGroup, float speed, AfterEnumerator callAfter = null) {
        float alpha = fadeIn ? 0 : 1;
        while (fadeIn ? alpha < 1 : alpha > 0) {
            alpha = Mathf.Clamp(fadeIn ? alpha + speed : alpha - speed, 0, 1);
            canvasGroup.alpha = alpha;
            yield return 0;
        }
        callAfter?.Invoke();
    }

    private IEnumerator BlurFade(bool fadeIn, float speed, AfterEnumerator callAfter = null) {
        float alpha = fadeIn ? 0 : 1;
        Material blur = blurPanel.GetComponent<Image>().material;
        while (fadeIn ? alpha < 1 : alpha > 0) {
            alpha = Mathf.Clamp(fadeIn ? alpha + speed : alpha - speed, 0, 1);
            blur.SetFloat("_Size", Mathf.Lerp(blurEndpoints[0], blurEndpoints[1], alpha));
            blur.SetColor("_MultiplyColor", LerpColor(blurTintEndpoints[0], blurTintEndpoints[1], alpha));
            yield return 0;
        }
        callAfter?.Invoke();
    }

    private Color LerpColor(Color start, Color finish, float t) {
        Vector4 startRGBA = new Vector4(start.r, start.g, start.b, start.a);
        Vector4 finishRGBA = new Vector4(finish.r, finish.g, finish.b, finish.a);
        Vector4 newColor = Vector4.Lerp(startRGBA, finishRGBA, t);
        return new Color(newColor[0], newColor[1], newColor[2], newColor[3]);
    }
}
