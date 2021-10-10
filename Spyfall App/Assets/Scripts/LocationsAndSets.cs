using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DataContract]
[KnownType(typeof(CustomLocation))]
public class Location {
    [DataMember]
    protected string name;
    [DataMember]
    public string Name { get => name ?? "NEW CUSTOM LOCATION"; set => name = value; }

    [DataMember]
    protected List<string> roles;
    [DataMember]
    public List<string> Roles { get => roles ?? new List<string> { "Role 1", "Role 2", "Role 3" }; set => roles = value; }

    [DataMember]
    public bool enabled;

    public SettingsUIComponent SettingsUI { get; set; }

    [DataMember]
    private static readonly System.Random rand = new System.Random();

    public Location(string location, string roles, bool enabled = true) {
        Name = location;
        if (roles != null) {
            Roles = roles.Replace(", ", ",").Split(',').ToList();
        }
        this.enabled = enabled;
    }

    // makes of shuffled list of roles (repeating shuffled when necessary) for players with a spy inserted
    public List<Player> AssignRolesToPlayers(List<Player> players) {
        List<string> repeatedRoles = Roles.Shuffle();
        while (repeatedRoles.Count < players.Count - 1) {
            repeatedRoles = repeatedRoles.Concat(Roles.Shuffle()).ToList();
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
        string searchFor = $"spr_{Name.ToLower().Replace(" ", "_").Replace("\"", "")}";
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

        public SettingsUIComponent(LocationSetController parent, bool toggleValue, Toggle toggle = null) {
            this.parent = parent;
            this.toggleValue = toggleValue;
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
    public byte[] Image { get; private set; }
    [DataMember]
    public bool JustAdded { get; set; } = true;
    [DataMember]
    public bool Deleted { get; set; }

    public CustomLocation() : base(null, null, true) { }

    public void SetImage(Texture2D image) {
        Image = ImageConversion.EncodeToPNG(image);
    }
    public void SetImage(byte[] image) {
        Image = image;
    }

    public override Texture2D GetImage() {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(Image);
        return Image == null ? Resources.Load("custom_location_default") as Texture2D : tex;
    }

    public class CustomSettingsUIComponent : SettingsUIComponent {
        public byte[] TempImage { get; protected set; }

        protected string tempName;
        public string TempName { get => tempName ?? "NEW CUSTOM LOCATION"; set => tempName = value; }

        protected List<string> tempRoles;
        public List<string> TempRoles { get => tempRoles ?? new List<string> { "Role 1", "Role 2", "Role 3" }; set => tempRoles = value; }

        public CustomSettingsUIComponent(CustomLocation location, LocationSetController parent, bool toggleValue, Toggle toggle = null) : base(parent, toggleValue, toggle) {
            SetTempImage(location.Image);

            // assign by backing field to preserve null values
            if (location.name != null) {
                TempName = string.Copy(location.name);
            }
            
            if (location.roles != null) {
                TempRoles = location.roles.Select(role => string.Copy(role)).ToList();
            }
        }

        public void SetTempImage(Texture2D image) {
            if (image != null) {
                TempImage = ImageConversion.EncodeToPNG(image);
            } else {
                TempImage = null;
            }
        }
        public void SetTempImage(byte[] image) {
            if (image != null) {
                if (TempImage == null) {
                    TempImage = new byte[image.Length];
                }
                Array.Copy(image, TempImage, image.Length);
            } else {
                TempImage = null;
            }
        }

        public Texture2D GetTempImage() {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(TempImage);
            if (TempImage == null) {
                tex = null;
            }
            return tex;
        }

        public string GetRealName() {
            return tempName;
        }
        public List<string> GetRealRoles() {
            return tempRoles;
        }
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

    public void AddLocation(CustomLocation newLocation) {
        Locations.Add(newLocation);
    }

    public void RemoveLocation(CustomLocation location) {
        Locations.Remove(location);
    }
}
