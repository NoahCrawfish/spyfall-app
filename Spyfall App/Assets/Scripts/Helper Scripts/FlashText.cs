using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float t;
    private Color startingColor;
    public float flashSpeed = 1f;

    public StartingAlpha startingPoint = StartingAlpha.opaque;
    public enum StartingAlpha {
        opaque,
        transparent
    }

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        startingColor = text.color;
    }

    private void OnDisable() {
        ResetT();
    }

    public void ResetT() {
        t = 0f;
    }

    private void Update() {
        if (gameObject.activeSelf) {
            t += Time.deltaTime;
            switch (startingPoint) {
                case StartingAlpha.opaque:
                    text.color = new Color(startingColor.r, startingColor.g, startingColor.b, (Mathf.Cos(t * flashSpeed) + 1) * 0.5f);
                    break;
                case StartingAlpha.transparent:
                    text.color = new Color(startingColor.r, startingColor.g, startingColor.b, (Mathf.Cos(t * flashSpeed + Mathf.PI) + 1) * 0.5f);
                    break;
            }
            
        }
    }
}
