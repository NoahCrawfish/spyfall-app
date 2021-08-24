using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IncrementController : MonoBehaviour
{
    [SerializeField] private GameObject valueRenderer;
    [SerializeField] private GameObject plusButtonRenderer;
    [SerializeField] private GameObject minusButtonRenderer;
    [SerializeField] private float value;
    public float Value {
        get { return value; }
        set { this.value = value; }
    }
    [SerializeField] private bool disabled;
    public bool Disabled {
        get { return disabled; }
        set {
            disabled = value;
            RefreshValue();
            RefreshColor();
        }
    }
    [SerializeField] private float incrementAmount;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField, Range(0, 2)] private int trailingZeros;
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;

    private void OnEnable() {
        RefreshValue();
        RefreshColor();
    }

    public void Add() {
        Value = Mathf.Clamp(Value + incrementAmount, minValue, maxValue);
        RefreshValue();
    }

    public void Subtract() {
        Value = Mathf.Clamp(Value - incrementAmount, minValue, maxValue);
        RefreshValue();
    }

    public void RefreshValue() {
        if (!disabled) {
            valueRenderer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Value.ToString($"N{trailingZeros}");
        } else {
            valueRenderer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "-";
        }
    }

    private void RefreshColor() {
        if (disabled) {
            valueRenderer.GetComponent<Image>().color = disabledColor;
            transform.Find("MinusButton").GetComponent<Button>().interactable = false;
            minusButtonRenderer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = disabledColor;
            transform.Find("PlusButton").GetComponent<Button>().interactable = false;
            plusButtonRenderer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = disabledColor;
        } else {
            valueRenderer.GetComponent<Image>().color = enabledColor;
            transform.Find("MinusButton").GetComponent<Button>().interactable = true;
            minusButtonRenderer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = enabledColor;
            transform.Find("PlusButton").GetComponent<Button>().interactable = true;
            plusButtonRenderer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = enabledColor;
        }
    }
}
