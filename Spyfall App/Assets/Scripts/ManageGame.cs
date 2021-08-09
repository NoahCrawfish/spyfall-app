using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ManageGame : MonoBehaviour {
    [SerializeField] GameObject addPlayersScreen;
    [SerializeField] GameObject roundScreen;
    [SerializeField] GameObject drawCardsScreen;
    [SerializeField] private GameObject playerFields;
    [SerializeField] private TextMeshProUGUI roundText;

    private const float roundScreenTime = 1f;
    public List<Player> Players { get; private set; } = new List<Player>();
    public List<List<Location>> Locations { get; private set; } = new List<List<Location>>();
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

    private static readonly System.Random rand = new System.Random();
    private UITransitions uiTransitions;
    private ManageDrawCardsScreen manageDrawCards;
    private ManageGameplayScreen manageGameplay;
    private ManageSettingsScreen manageSettings;

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
    }

    private void Start() {
        InitializeLocations();
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
    

    // creates list of Location classes from dictionary
    private void InitializeLocations() {
        Locations.Clear();
        foreach (var locationSet in LocationsAndRoles.locationSets) {
            Locations.Add(new List<Location>());
            foreach (KeyValuePair<string, string> entry in locationSet) {
                Locations[Locations.Count - 1].Add(new Location(entry.Key, entry.Value));
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
        // flattens location sets, filters out disabled locations, and assigns to a new list for the current game
        foreach (var location in Locations.SelectMany(x => x).ToList()) {
            if (location.enabled) {
                LocationsUsing.Add(location);
            }
        }
    }

    public void ResetRounds() {
        CurrentRound = 0;
    }

    public IEnumerator StartNextRound(CanvasGroup currentPanel) {
        CurrentRound += 1;
        roundText.text = (CurrentRound < MaxRounds) ? $"Round\n{CurrentRound}" : "Final\nRound";
        AssignLocationAndRoles();

        // briefly show round screen
        uiTransitions.CrossFadeBetweenPanels(currentPanel, roundScreen.GetComponent<CanvasGroup>());
        yield return new WaitForSeconds(roundScreenTime);
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

public class Location {
    public readonly string name;
    public readonly List<string> roles;
    public bool enabled;
    private static readonly System.Random rand = new System.Random();

    public Location(string location, string roles, bool enabled = true) {
        name = location;
        this.roles = roles.Replace(", ", ",").Split(',').ToList();
        this.enabled = enabled;
    }

    // shuffles roles and assigns in order to players, reshuffles when needed
    public List<Player> AssignRolesToPlayers(List<Player> players) {
        int i = 0;
        roles.Shuffle();
        roles.Insert(rand.Next(Mathf.Min(players.Count, roles.Count)), "Spy");
        foreach (var player in players) {
            player.Role = roles[i];
            if (roles[i] == "Spy") {
                roles.Remove("Spy");
                i--;
            }
            if (i < roles.Count - 1) {
                i++;
            } else {
                i = 0;
                roles.Shuffle();
            }
        }
        return players;
    }

    // gets location image from resources folder, if none is found it pulls an "image not found" placeholder
    public Texture2D GetImage() {
        string searchFor = $"spr_{name.ToLower().Replace(" ", "_")}";
        Texture2D image = Resources.Load(searchFor) as Texture2D;
        if (image == null) {
            image = Resources.Load("image_not_found") as Texture2D;
        }
        return image;
    }
}

public static class Extensions {
    private static readonly System.Random rand = new System.Random();

    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
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
}