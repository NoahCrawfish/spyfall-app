using System.Collections.Generic;
using System.Runtime.Serialization;

public static class LocationsAndRoles {
    public static Dictionary<string, string>[] setsData = new Dictionary<string, string>[] {
        new Dictionary<string, string>() {
            { "Airplane", "First Class Passenger, Air Marshall, Mechanic, Economy Class Passenger, Stewardess, Co-Pilot, Captain" },
            { "Bank", "Armored Car Driver, Manager, Consultant, Customer, Robber, Security Guard, Teller" },
            { "Beach", "Beach Waitress, Kite Surfer, Lifeguard, Thief, Beach Goer, Beach Photographer, Ice Cream Truck Driver" }
        },
        new Dictionary<string, string>() {
            { "Broadway Theater", "Coat Check Lady, Prompter, Cashier, Visitor, Director, Actor, Crewman" },
            { "Casino", "Bartender, Head Security Guard, Bouncer, Manager, Hustler, Dealer, Gambler" }
        }
    };

    public static List<string> setNames = new List<string> {
        "Location Pack 1",
        "Location Pack 2"
    };
}

[DataContract]
public class LocationSet {
    [DataMember]
    public readonly string name;
    [DataMember]
    public readonly bool locked;
    [DataMember]
    public List<Location> Locations { get; private set; } = new List<Location>();

    public LocationSet(Dictionary<string, string> locationsAndRoles, string name, bool locked) {
        this.name = name;
        this.locked = locked;

        foreach (KeyValuePair<string, string> entry in locationsAndRoles) {
            Locations.Add(new Location(entry.Key, entry.Value));
        }
    }
}
