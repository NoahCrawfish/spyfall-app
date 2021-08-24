using System.Collections;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using TMPro;

public class TimeSelectController : MonoBehaviour
{
    [SerializeField] private TMP_InputField minInput;
    [SerializeField] private TMP_InputField secInput;
    [SerializeField] private GameObject timerModeButton;
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color shadowEnabledColor;
    [SerializeField] private Color shadowDisabledColor;
    [SerializeField] private RectTransform shadowRect;
    private const int minTimeSeconds = 30;
    private string minBeforeDisable = "01";
    private string secBeforeDisable = "00";
    private bool disabled;

    public bool Disabled {
        get { return disabled; }
        set {
            disabled = value;
            DisabledUpdated(disabled);
        }
    }

    private void OnEnable() {
        minInput.onDeselect.AddListener(OnMinChange);
        secInput.onDeselect.AddListener(OnSecChange);
    }
    private void OnDisable() {
        minInput.onDeselect.RemoveAllListeners();
        secInput.onDeselect.RemoveAllListeners();
    }

    private void OnMinChange(string value) {
        if (value == "") {
            value = "0";
        }
        minInput.text = Mathf.Clamp(int.Parse(value), 0, 59).ToString("D2");
        minBeforeDisable = minInput.text;
        if (int.Parse(value) == 0 && int.Parse(secInput.text) < minTimeSeconds) {
            secInput.text = minTimeSeconds.ToString("D2");
            secBeforeDisable = secInput.text;
        }
    }

    private void OnSecChange(string value) {
        if (value == "") {
            value = "0";
        }
        secInput.text = Mathf.Clamp(int.Parse(value), int.Parse(minInput.text) == 0 ? minTimeSeconds : 0, 59).ToString("D2");
        secBeforeDisable = secInput.text;
    }

    private void DisabledUpdated(bool disabled) {
        if (disabled) {
            minInput.text = secInput.text = "--";
            minInput.interactable = secInput.interactable = false;
            GetComponent<ProceduralImage>().color = disabledColor;
            shadowRect.gameObject.GetComponent<ProceduralImage>().color = shadowDisabledColor;
        } else {
            minInput.text = minBeforeDisable;
            secInput.text = secBeforeDisable;
            minInput.interactable = secInput.interactable = true;
            GetComponent<ProceduralImage>().color = enabledColor;
            shadowRect.gameObject.GetComponent<ProceduralImage>().color = shadowEnabledColor;
        }
    }
}
