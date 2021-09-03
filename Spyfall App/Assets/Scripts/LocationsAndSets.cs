using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DataContract]
[KnownType(typeof(CustomLocation))]
public class Location {
    [DataMember]
    public readonly string name;
    [DataMember]
    public readonly List<string> roles;
    [DataMember]
    public bool enabled;

    public SettingsUIComponent SettingsUI { get; set; }

    private static readonly System.Random rand = new System.Random();

    public Location(string location, string roles, bool enabled = true) {
        name = location;
        this.roles = roles.Replace(", ", ",").Split(',').ToList();
        this.enabled = enabled;
    }

    // makes of shuffled list of roles (repeating shuffled when necessary) for players with a spy inserted
    public List<Player> AssignRolesToPlayers(List<Player> players) {
        List<string> repeatedRoles = roles.Shuffle();
        while (repeatedRoles.Count < players.Count - 1) {
            repeatedRoles = repeatedRoles.Concat(roles.Shuffle()).ToList();
        }
        repeatedRoles = repeatedRoles.GetRange(0, players.Count - 1);
        repeatedRoles.Insert(rand.Next(repeatedRoles.Count + 1), "Spy");

        int i = 0;
        foreach (var player in players) {
            player.Role = repeatedRoles[i];
            i++;
        }

        return players;
    }

    // gets location image from resources folder, if none is found it pulls an "image not found" placeholder
    public virtual Texture2D GetImage() {
        string searchFor = $"spr_{name.ToLower().Replace(" ", "_")}";
        Texture2D image = Resources.Load(searchFor) as Texture2D;
        if (image == null) {
            image = Resources.Load("image_not_found") as Texture2D;
        }
        return image;
    }

    public class SettingsUIComponent {
        protected Toggle toggle;
        public bool toggleValue;
        protected readonly LocationSetController parent;

        public SettingsUIComponent(LocationSetController parent, Toggle toggle = null) {
            this.parent = parent;
            if (toggle != null) {
                AssignToggle(toggle);
            }
        }

        public virtual void AssignToggle(Toggle toggle) {
            this.toggle = toggle;
        }

        public virtual void OnToggleChanged() {
            toggleValue = toggle.isOn;
            parent.RefreshSetToggle();
        }

        public virtual void RefreshToggle(bool supressTween = false) {
            toggle.isOn = toggleValue;
            if (supressTween) {
                toggle.GetComponent<ToggleSwitchController>().SupressTween(toggleValue);
            }
        }
    }
}

[DataContract]
public class CustomLocation : Location {
    [DataMember]
    public readonly byte[] image;

    public CustomLocation(string location, string roles, Texture2D image, bool enabled = true) : base(location, roles, enabled) {
        this.image = ImageConversion.EncodeToPNG(image);
    }

    public override Texture2D GetImage() {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(image);
        return image == null ? Resources.Load("custom_location_default") as Texture2D : tex;
    }
}


[DataContract]
[KnownType(typeof(CustomLocationSet))]
public class LocationSet {
    [DataMember]
    public readonly string name;
    [DataMember]
    public bool locked;
    [DataMember]
    public List<Location> Locations { get; protected set; } = new List<Location>();

    public LocationSet(Dictionary<string, string> locationsAndRoles, string name, bool locked) {
        this.name = name;
        this.locked = locked;

        if (locationsAndRoles != null) {
            foreach (KeyValuePair<string, string> entry in locationsAndRoles) {
                Locations.Add(new Location(entry.Key, entry.Value, !locked));
            }
        }
    }
}

[DataContract]
public class CustomLocationSet : LocationSet {
    public CustomLocationSet(bool locked) : base(null, LocationsAndRoles.customSetName, locked) { }

    public void AddLocation(CustomLocation location) {
        Locations.Add(location);
    }

    public void RemoveLocation(CustomLocation location) {
        Locations.Remove(location);
    }
}
