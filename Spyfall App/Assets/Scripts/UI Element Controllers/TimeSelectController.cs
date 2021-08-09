using System;
using UnityEngine;
using TMPro;

public class TimeSelectController : MonoBehaviour
{
    [SerializeField] TMP_InputField minInput;
    [SerializeField] TMP_InputField secInput;
    [SerializeField] GameObject timerModeButton;
    private const int minTimeSeconds = 30;
    private string minBeforeDisable = "01";
    private string secBeforeDisable = "00";


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
        if (int.Parse(value) == 0 && int.Parse(secInput.text) < minTimeSeconds) {
            secInput.text = minTimeSeconds.ToString("D2");
        }
    }

    private void OnSecChange(string value) {
        if (value == "") {
            value = "0";
        }
        secInput.text = Mathf.Clamp(int.Parse(value), int.Parse(minInput.text) == 0 ? minTimeSeconds : 0, 59).ToString("D2");
    }

    public void TimerModeChanged() {
        if (timerModeButton.GetComponent<TimerModeController>().Stage == ManageGame.TimerModes.disabled) {
            minBeforeDisable = minInput.text;
            secBeforeDisable = secInput.text;
            UpdateIfDisabled();
        } else if (timerModeButton.GetComponent<TimerModeController>().Stage == ManageGame.TimerModes.perPlayer) {
            minInput.text = minBeforeDisable;
            secInput.text = secBeforeDisable;
            minInput.interactable = secInput.interactable = true;
        }
    }

    public void UpdateIfDisabled() {
        if (timerModeButton.GetComponent<TimerModeController>().Stage == ManageGame.TimerModes.disabled) {
            minInput.text = secInput.text = "--";
            minInput.interactable = secInput.interactable = false;
        }
    }
}
