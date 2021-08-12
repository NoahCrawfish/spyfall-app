using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageSettingsScreen : MonoBehaviour
{
    [SerializeField] private IncrementController roundsIncrement;
    [SerializeField] private GameObject timeSelect;
    [SerializeField] private TimerModeController timerModeButton;
    [SerializeField] private IncrementController civlianPointsIncrement;
    [SerializeField] private IncrementController spyPointsIncrement;
    [SerializeField] private Toggle disableScoringToggle;
    [SerializeField] private IncrementController multiplierIncrement;

    private ManageGame manageGame;

    public enum Prefs {
        Rounds,
        Time,
        TimerMode,
        CivilianPoints,
        SpyPoints,
        ScoringDisabled,
        LastRoundMultiplier
    }
    public int[] defaultValues = new int[] { 3, 60, (int)ManageGame.TimerModes.perPlayer, 2, 4, 0, 1 };

    private void Awake() {
        manageGame = FindObjectOfType<ManageGame>();
    }

    public void SaveSettings() {
        int[] settings = new int[Enum.GetNames(typeof(Prefs)).Length];
        settings[(int)Prefs.Rounds] = (int)roundsIncrement.Value;
        int minutes = int.Parse(timeSelect.transform.Find("MinInput").GetComponent<TMP_InputField>().text.Replace("--", "0"));
        int seconds = int.Parse(timeSelect.transform.Find("SecInput").GetComponent<TMP_InputField>().text.Replace("--", "0"));
        settings[(int)Prefs.Time] = minutes * 60 + seconds;
        settings[(int)Prefs.TimerMode] = (int)timerModeButton.Stage;
        settings[(int)Prefs.CivilianPoints] = (int)civlianPointsIncrement.Value;
        settings[(int)Prefs.SpyPoints] = (int)spyPointsIncrement.Value;
        settings[(int)Prefs.ScoringDisabled] = disableScoringToggle.isOn.ToInt();
        settings[(int)Prefs.LastRoundMultiplier] = (int)multiplierIncrement.Value;

        int i = 0;
        foreach (var value in settings) {
            string prefName = Enum.GetName(typeof(Prefs), (Prefs)i);
            PlayerPrefs.SetInt(prefName, value);
            i++;
        }

        manageGame.RefreshSettings();
    }

    public void LoadSettings() {
        roundsIncrement.Value = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.Rounds), defaultValues[(int)Prefs.Rounds]);
        roundsIncrement.RefreshValue();
        int minutes = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.Time), defaultValues[(int)Prefs.Time]) / 60;
        int seconds = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.Time), defaultValues[(int)Prefs.Time]) - minutes * 60;
        timeSelect.transform.Find("MinInput").GetComponent<TMP_InputField>().text = minutes.ToString("D2");
        timeSelect.transform.Find("SecInput").GetComponent<TMP_InputField>().text = seconds.ToString("D2");
        timerModeButton.Stage = (ManageGame.TimerModes)PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.TimerMode), defaultValues[(int)Prefs.TimerMode]);
        timerModeButton.SetText();
        civlianPointsIncrement.Value = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.CivilianPoints), defaultValues[(int)Prefs.CivilianPoints]);
        civlianPointsIncrement.RefreshValue();
        spyPointsIncrement.Value = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.SpyPoints), defaultValues[(int)Prefs.SpyPoints]);
        spyPointsIncrement.RefreshValue();
        disableScoringToggle.isOn = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.ScoringDisabled), defaultValues[(int)Prefs.ScoringDisabled]).ToBool();
        multiplierIncrement.Value = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.LastRoundMultiplier), defaultValues[(int)Prefs.LastRoundMultiplier]);
        multiplierIncrement.RefreshValue();
    }

    public void SetUIToDefault() {
        roundsIncrement.Value = defaultValues[(int)Prefs.Rounds];
        roundsIncrement.RefreshValue();
        int minutes = defaultValues[(int)Prefs.Time] / 60;
        int seconds = defaultValues[(int)Prefs.Time] - minutes * 60;
        timeSelect.transform.Find("MinInput").GetComponent<TMP_InputField>().text = minutes.ToString("D2");
        timeSelect.transform.Find("SecInput").GetComponent<TMP_InputField>().text = seconds.ToString("D2");
        timerModeButton.Stage = (ManageGame.TimerModes)defaultValues[(int)Prefs.TimerMode];
        timerModeButton.SetText();
        civlianPointsIncrement.Value = defaultValues[(int)Prefs.CivilianPoints];
        civlianPointsIncrement.RefreshValue();
        spyPointsIncrement.Value = defaultValues[(int)Prefs.SpyPoints];
        spyPointsIncrement.RefreshValue();
        disableScoringToggle.isOn = defaultValues[(int)Prefs.ScoringDisabled].ToBool();
        multiplierIncrement.Value = defaultValues[(int)Prefs.LastRoundMultiplier];
        multiplierIncrement.RefreshValue();
    }
}
