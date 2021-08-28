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
    [SerializeField] private DisableScoringIncrements disableScoringIncrements;

    [SerializeField] private GameObject locationSetPrefab;
    [SerializeField] private GameObject locationTogglePrefab;
    [SerializeField] private GameObject locationSetsFrame;

    [SerializeField] private ScrollRect scrollRect;

    private ManageGame manageGame;
    public CanvasGroup PreviousScreen { get; set; }

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

    public void Initalize() {
        LoadSettings();
        CreateLocationSets();
        scrollRect.verticalNormalizedPosition = 1;
    }

    public void SaveSettings() {
        // create array of ints to write to playerPrefs
        int[] settings = new int[Enum.GetNames(typeof(Prefs)).Length];

        // set settings array values
        settings[(int)Prefs.Rounds] = (int)roundsIncrement.Value;
        int minutes = int.Parse(timeSelect.transform.GetChild(0).Find("MinInput").GetComponent<TMP_InputField>().text.Replace("--", "0"));
        int seconds = int.Parse(timeSelect.transform.GetChild(0).Find("SecInput").GetComponent<TMP_InputField>().text.Replace("--", "0"));
        settings[(int)Prefs.Time] = minutes * 60 + seconds;
        settings[(int)Prefs.TimerMode] = (int)timerModeButton.Stage;
        settings[(int)Prefs.CivilianPoints] = (int)civlianPointsIncrement.Value;
        settings[(int)Prefs.SpyPoints] = (int)spyPointsIncrement.Value;
        settings[(int)Prefs.ScoringDisabled] = disableScoringToggle.isOn.ToInt();
        settings[(int)Prefs.LastRoundMultiplier] = (int)multiplierIncrement.Value;

        // write to playerPrefs
        int i = 0;
        foreach (var value in settings) {
            string prefName = Enum.GetName(typeof(Prefs), (Prefs)i);
            PlayerPrefs.SetInt(prefName, value);
            i++;
        }

        // update "enabled" field for Location classes
        foreach (Transform locationSet in locationSetsFrame.transform) {
            locationSet.GetComponent<LocationSetController>().SaveLocationStates();
        }

        manageGame.RefreshSettings();
        manageGame.SaveSets();
        manageGame.InitializeLocationsUsing();
    }

    public void LoadSettings() {
        roundsIncrement.Value = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.Rounds), defaultValues[(int)Prefs.Rounds]);
        roundsIncrement.RefreshValue();

        int minutes = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.Time), defaultValues[(int)Prefs.Time]) / 60;
        int seconds = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.Time), defaultValues[(int)Prefs.Time]) - minutes * 60;
        timeSelect.transform.GetChild(0).Find("MinInput").GetComponent<TMP_InputField>().text = minutes.ToString("D2");
        timeSelect.transform.GetChild(0).Find("SecInput").GetComponent<TMP_InputField>().text = seconds.ToString("D2");

        timerModeButton.Stage = (ManageGame.TimerModes)PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.TimerMode), defaultValues[(int)Prefs.TimerMode]);
        timerModeButton.SetText();

        civlianPointsIncrement.Value = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.CivilianPoints), defaultValues[(int)Prefs.CivilianPoints]);
        civlianPointsIncrement.RefreshValue();

        spyPointsIncrement.Value = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.SpyPoints), defaultValues[(int)Prefs.SpyPoints]);
        spyPointsIncrement.RefreshValue();

        disableScoringToggle.isOn = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.ScoringDisabled), defaultValues[(int)Prefs.ScoringDisabled]).ToBool();
        disableScoringIncrements.OnToggleChanged(disableScoringToggle.isOn);

        multiplierIncrement.Value = PlayerPrefs.GetInt(Enum.GetName(typeof(Prefs), Prefs.LastRoundMultiplier), defaultValues[(int)Prefs.LastRoundMultiplier]);
        multiplierIncrement.RefreshValue();
    }

    public void SetUIToDefault() {
        roundsIncrement.Value = defaultValues[(int)Prefs.Rounds];
        roundsIncrement.RefreshValue();

        int minutes = defaultValues[(int)Prefs.Time] / 60;
        int seconds = defaultValues[(int)Prefs.Time] - minutes * 60;
        timeSelect.transform.GetChild(0).Find("MinInput").GetComponent<TMP_InputField>().text = minutes.ToString("D2");
        timeSelect.transform.GetChild(0).Find("SecInput").GetComponent<TMP_InputField>().text = seconds.ToString("D2");

        timerModeButton.Stage = (ManageGame.TimerModes)defaultValues[(int)Prefs.TimerMode];
        timerModeButton.SetText();

        civlianPointsIncrement.Value = defaultValues[(int)Prefs.CivilianPoints];
        civlianPointsIncrement.RefreshValue();

        spyPointsIncrement.Value = defaultValues[(int)Prefs.SpyPoints];
        spyPointsIncrement.RefreshValue();

        disableScoringToggle.isOn = defaultValues[(int)Prefs.ScoringDisabled].ToBool();
        disableScoringIncrements.OnToggleChanged(disableScoringToggle.isOn);

        multiplierIncrement.Value = defaultValues[(int)Prefs.LastRoundMultiplier];
        multiplierIncrement.RefreshValue();

        // by default only first two sets are enabled
        int i = 0;
        foreach (Transform locationSet in locationSetsFrame.transform) {
            Toggle setToggle = locationSet.Find("SetFrame/ToggleFrame/SetToggle").GetComponent<Toggle>();
            setToggle.isOn = i == 0 || i == 1;
            locationSet.GetComponent<LocationSetController>().OnSetToggle(setToggle);
            i++;
        }
    }

    public void CreateLocationSets() {
        if (locationSetsFrame.transform.childCount == 0) {
            foreach (var locationSet in manageGame.LocationSets) {
                // create locationSet
                GameObject set = Instantiate(locationSetPrefab);
                set.transform.SetParent(locationSetsFrame.transform, false);
                set.transform.Find("SetFrame/SetButton/Renderer").GetChild(0).GetComponent<TextMeshProUGUI>().text = locationSet.name;
                set.name = $"Set_{set.transform.GetSiblingIndex() + 1}";
                // create reference to the locationSet stored in ManageGame
                set.GetComponent<LocationSetController>().ThisSet = locationSet;
            }
        }
    }
}
