using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;
using TMPro;

public class ManageGame : MonoBehaviour {
    [SerializeField] GameObject addPlayersScreen;
    [SerializeField] GameObject roundScreen;
    [SerializeField] GameObject drawCardsScreen;
    [SerializeField] private GameObject playerFields;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private GameObject multiplierText;

    private const float roundScreenTime = 1f;
    public List<Player> Players { get; private set; } = new List<Player>();
    public List<LocationSet> LocationSets { get; private set; } = new List<LocationSet>();
    public CustomLocationSet CustomSet { get; private set; }
    public bool Paused { get; private set; }
    public DateTime PauseTime { get; private set; }
    public DateTime UnpauseTime { get; private set; }
    
    // current round
    public int CurrentRound { get; private set; } = 0;
    public Location CurrentLocation { get; private set; }
    public List<Location> LocationsUsing { get; private set; } = new List<Location>();

    // current settings
    public int MaxRounds { get; private set; }
    public int TimerSeconds { get; private set; }
    public TimerModes TimerMode { get; private set; }
    public int[] MaxPoints { get; private set; } = new int[2];
    public bool ScoringDisabled { get; private set; }
    public int LastRoundMultiplier { get; private set; }
    public bool PaidUnlocked { get; private set; } // need to load this data directly from the app store on launch, to retain purchases in case of uninstalling

    private string PlatformPathModifier {
        get {
            #if UNITY_EDITOR
                return "Editor-";
            #elif UNITY_IOS
                return "iOS-";
            #endif
        }
    }

    private string savePath;

    private static readonly System.Random rand = new System.Random();
    private UITransitions uiTransitions;
    private ManageDrawCardsScreen manageDrawCards;
    private ManageGameplayScreen manageGameplay;
    private ManageSettingsScreen manageSettings;
    private ManageAddPlayersScreen manageAddPlayers;

    public enum TimerModes { 
        disabled,
        perPlayer,
        setAmount
    }

    public enum PlayerTypes {
        civilian,
        spy
    }

    private void Awake() {
        uiTransitions = FindObjectOfType<UITransitions>();
        manageDrawCards = FindObjectOfType<ManageDrawCardsScreen>();
        manageGameplay = FindObjectOfType<ManageGameplayScreen>();
        manageSettings = FindObjectOfType<ManageSettingsScreen>();
        manageAddPlayers = FindObjectOfType<ManageAddPlayersScreen>();
    }

    private void Start() {
        savePath = $"{Application.persistentDataPath}/{PlatformPathModifier}";
        if (!Directory.Exists(savePath)) {
            Directory.CreateDirectory(savePath);
        }

        DeleteDepreciatedSets();
        LoadSets();
        InitializeLocationsUsing();
        RefreshSettings();
    }

    private void OnApplicationFocus(bool focus) {
        if (Paused == focus) {
            if (focus) {
                UnpauseTime = DateTime.Now;
                manageGameplay.UpdateTimerFromPause();
            } else {
                PauseTime = DateTime.Now;
            }
        }
        Paused = !focus;
    }
    private void OnApplicationPause(bool pause) {
        if (Paused != pause) {
            if (pause) {
                PauseTime = DateTime.Now;
            } else {
                UnpauseTime = DateTime.Now;
                manageGameplay.UpdateTimerFromPause();
            }
        }
        Paused = pause;
    }

    // dev debug
    private void Update() {
        if (Input.GetKeyDown(KeyCode.P) && !PaidUnlocked) {
            UnlockFullVersion();
        }
    }


    private string StringToPath(string input) {
        return input.ToLower().Replace(' ', '_');
    }

    private void LoadSets() {
        string filePath;
        foreach (string setName in LocationsAndRoles.setNames) {
            filePath = $"{savePath}/{StringToPath(setName)}";
            if (File.Exists(filePath)) {
                LocationSet locationSet = Serialization.LoadViaDataContractSerialization<LocationSet>(filePath);
                LocationSets.Add(locationSet);
            } else {
                // only first two sets are available, unless the paid version has been unlocked
                int setIndex = LocationsAndRoles.setNames.IndexOf(setName);
                LocationSet locationSet = new LocationSet(LocationsAndRoles.setsData[setIndex], setName, setIndex >= 2 && !PaidUnlocked);
                LocationSets.Add(locationSet);
            }
        }

        // load custom set
        filePath = $"{savePath}/{StringToPath(LocationsAndRoles.customSetName)}";
        CustomSet = File.Exists(filePath) ?
            Serialization.LoadViaDataContractSerialization<CustomLocationSet>(filePath) : new CustomLocationSet(!PaidUnlocked);

        SaveSets();
    }

    public void SaveSets() {
        string filePath;
        foreach (var locationSet in LocationSets) {
            filePath = $"{savePath}/{StringToPath(locationSet.name)}";
            Serialization.SaveViaDataContractSerialization(locationSet, filePath);
        }

        SaveCustomSet();
    }

    public void SaveCustomSet() {
        string filePath = $"{savePath}/{StringToPath(LocationsAndRoles.customSetName)}";
        Serialization.SaveViaDataContractSerialization(CustomSet, filePath);
    }

    private void DeleteDepreciatedSets() {
        List<string> pathNames = LocationsAndRoles.setNames.Select(x => StringToPath(x)).ToList();

        string file = "";
        foreach (string filePath in Directory.GetFiles(savePath)) {
            file = Path.GetFileNameWithoutExtension(filePath);
            if (!pathNames.Contains(file) && file != StringToPath(LocationsAndRoles.customSetName)) {
                File.Delete(filePath);
            }
        }
    }


    public void CreatePlayerList() {
        Players.Clear();
        // creates list of Player classes from fields' text/placeholder values
        foreach (Transform child in playerFields.transform) {
            if (child.gameObject.name.Split('_')[0] == "Player") {
                string text = child.GetChild(0).GetComponent<TMP_InputField>().text;
                if (text == "") {
                    text = ((TextMeshProUGUI)child.GetChild(0).GetComponent<TMP_InputField>().placeholder).text;
                }
                Players.Add(new Player(text));
            }
        }
    }

    public void InitializeLocationsUsing() {
        LocationsUsing.Clear();
        // filters out disabled locations, and assigns to a new list for the current game
        foreach (var locationSet in LocationSets.Concat(new List<CustomLocationSet> { CustomSet })) {
            foreach (var location in locationSet.Locations) {
                if (location.enabled) {
                    LocationsUsing.Add(location);
                }
            }
        }

        // update begin button status based on whether no locations are enabled
        if (LocationsUsing.Count > 0) {
            manageAddPlayers.ResetBeginButton();
        } else {
            manageAddPlayers.NoLocationsEnabled();
        }
    }


    public void ResetRounds() {
        CurrentRound = 0;
    }

    public IEnumerator StartNextRound(CanvasGroup currentPanel) {
        CurrentRound += 1;
        TextMeshProUGUI roundTextShadow = roundText.gameObject.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI roundTextShadow2 = roundText.gameObject.transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>();
        roundText.text = roundTextShadow.text = roundTextShadow2.text = (CurrentRound < MaxRounds) ? $"Round\n{CurrentRound}" : "Final\nRound";
        AssignLocationAndRoles();

        // briefly show round screen
        uiTransitions.CrossFadeBetweenPanels(currentPanel, roundScreen.GetComponent<CanvasGroup>());
        if (CurrentRound == MaxRounds && LastRoundMultiplier != 1 && !ScoringDisabled) {
            multiplierText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{LastRoundMultiplier}X SCORE MULTIPLIER!";
            multiplierText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{LastRoundMultiplier}X SCORE MULTIPLIER!";
            multiplierText.SetActive(true);
            yield return new WaitForSeconds(roundScreenTime * 1.5f);
        } else {
            multiplierText.SetActive(false);
            yield return new WaitForSeconds(roundScreenTime);
        }
        uiTransitions.CrossFadeBetweenPanels(roundScreen.GetComponent<CanvasGroup>(), drawCardsScreen.GetComponent<CanvasGroup>());

        yield return 0; // wait till next frame so that gameobject is active
        manageDrawCards.InitializeScreen();
    }

    public void AssignLocationAndRoles() {
        CurrentLocation = LocationsUsing[rand.Next(LocationsUsing.Count)];
        Players = CurrentLocation.AssignRolesToPlayers(Players);

        // ensures unique locations are picked until it is no longer possible
        LocationsUsing.Remove(CurrentLocation);
        if (LocationsUsing.Count == 0) {
            InitializeLocationsUsing();
        }
    }


    // load values from playerPrefs and apply them to settings fields
    public void RefreshSettings() {
        foreach (int pref in Enum.GetValues(typeof(ManageSettingsScreen.Prefs))) {
            string prefName = Enum.GetName(typeof(ManageSettingsScreen.Prefs), (ManageSettingsScreen.Prefs)pref);
            if (PlayerPrefs.HasKey(prefName)) {
                SetPref((ManageSettingsScreen.Prefs)pref, PlayerPrefs.GetInt(prefName));
            } else {
                SetPref((ManageSettingsScreen.Prefs)pref, manageSettings.defaultValues[pref]);
            }
        }
    }

    private void SetPref(ManageSettingsScreen.Prefs pref, int value) {
        switch (pref) {
            case ManageSettingsScreen.Prefs.Rounds:
                MaxRounds = value;
                break;
            case ManageSettingsScreen.Prefs.Time:
                TimerSeconds = value;
                break;
            case ManageSettingsScreen.Prefs.TimerMode:
                TimerMode = (TimerModes)value;
                break;
            case ManageSettingsScreen.Prefs.CivilianPoints:
                MaxPoints[(int)PlayerTypes.civilian] = value;
                break;
            case ManageSettingsScreen.Prefs.SpyPoints:
                MaxPoints[(int)PlayerTypes.spy] = value;
                break;
            case ManageSettingsScreen.Prefs.ScoringDisabled:
                ScoringDisabled = value.ToBool();
                break;
            case ManageSettingsScreen.Prefs.LastRoundMultiplier:
                LastRoundMultiplier = value;
                break;
        }
    }

    public void UnlockFullVersion() {
        PaidUnlocked = true;
        foreach (var locationSet in LocationSets) {
            locationSet.locked = false;
        }
        CustomSet.locked = false;
        manageSettings.SaveLocationStatesAndSet();
        StartCoroutine(manageSettings.RefreshAllSets());
    }
}

public class Player {
    public string Name { get; private set; }
    public string Role { get; set; }
    public float Score { get; set; } = 0f;
    public float PreviousScore { get; set; } = 0f;

    public Player(string name) {
        Name = name;
    }
}

public static class Serialization {
    public static void SaveViaDataContractSerialization<T>(T serializableObject, string filepath) {
        var serializer = new DataContractSerializer(typeof(T));
        var settings = new XmlWriterSettings() {
            Indent = true,
            IndentChars = "\t",
        };
        var writer = XmlWriter.Create(filepath, settings);
        serializer.WriteObject(writer, serializableObject);
        writer.Close();
    }

    public static T LoadViaDataContractSerialization<T>(string filepath) {
        var fileStream = new FileStream(filepath, FileMode.Open);
        var reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas { MaxArrayLength = int.MaxValue });
        var serilizer = new DataContractSerializer(typeof(T));
        T serializableObject = (T)serilizer.ReadObject(reader, true);
        reader.Close();
        fileStream.Close();
        return serializableObject;
    }
}

public static class Extensions {
    private static readonly System.Random rand = new System.Random();

    public static List<T> Shuffle<T>(this IList<T> list) {
        List<T> newList = new List<T>(list);
        int n = newList.Count;
        while (n > 1) {
            n--;
            int k = rand.Next(n + 1);
            T value = newList[k];
            newList[k] = newList[n];
            newList[n] = value;
        }
        return newList;
    }

    public static bool ToBool(this int i) {
        return i switch {
            0 => false,
            1 => true,
            _ => throw new ArgumentOutOfRangeException("integer", "int value must be 0 or 1 to convert to a bool"),
        };
    }

    public static int ToInt(this bool i) {
        if (i) {
            return 1;
        }
        return 0;
    }

    public static Texture2D TextureFromSprite(this Sprite sprite) {
        if (sprite.rect.width != sprite.texture.width) {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        } else
            return sprite.texture;
    }
}