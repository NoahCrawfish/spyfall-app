using UnityEngine;
using TMPro;

public class IncrementController : MonoBehaviour
{
    [SerializeField] private float value;
    public float Value {
        get { return value; }
        set { this.value = value; }
    }
    [SerializeField] private float incrementAmount;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField, Range(0, 2)] private int trailingZeros;

    private void OnEnable() {
        RefreshValue();
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
        transform.Find("Value").GetChild(0).GetComponent<TextMeshProUGUI>().text = Value.ToString($"N{trailingZeros}");
    }
}
