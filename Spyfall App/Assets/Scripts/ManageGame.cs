using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ManageGame : MonoBehaviour {
    [SerializeField] private GameObject playerFields;
    [SerializeField] private TextMeshProUGUI roundText;
    
    public List<Player> Players { get; private set; } = new List<Player>();
    public List<List<Location>> Locations { get; private set; } = new List<List<Location>>();
    
    // current round
    public int CurrentRound { get; private set; } = 0;
    public Location CurrentLocation { get; private set; }
    public List<Location> LocationsUsing { get; private set; } = new List<Location>();

    // settings
    public int MaxRounds { get; private set; } = 3;
    public int TimerSeconds { get; private set; } = 60;
    public TimerModes TimerMode { get; private set; } = TimerModes.perPlayer;
    public int[] MaxPoints { get; private set; } = new int[2];
    public int LastRoundMultiplier { get; private set; } = 1;

    private static readonly System.Random rand = new System.Random();

    public enum TimerModes { 
        disabled,
        perPlayer,
        setAmount
    }

    public enum PlayerTypes {
        civilian,
        spy
    }

    private void Start() {
        MaxPoints[(int)PlayerTypes.civilian] = 2;
        MaxPoints[(int)PlayerTypes.spy] = 4;
        InitializeLocations();
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

    public void ResetRounds() {
        CurrentRound = 0;
    }

    public void UpdateRound() {
        CurrentRound += 1;
        roundText.text = $"Round\n{CurrentRound}";
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

    public void AssignLocationAndRoles() {
        CurrentLocation = LocationsUsing[rand.Next(LocationsUsing.Count)];
        Players = CurrentLocation.AssignRolesToPlayers(Players);

        // ensures unique locations are picked until it is no longer possible
        LocationsUsing.Remove(CurrentLocation);
        if (LocationsUsing.Count == 0) {
            InitializeLocationsUsing();
        }
    }

    public void UpdateSettings() {

    }
}

public class Player {
    public string Name { get; private set; }
    public string Role { get; set; }
    public float Score { get; private set; } = 0f;

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
}