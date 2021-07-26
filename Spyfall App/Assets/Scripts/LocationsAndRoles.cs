using System.Collections.Generic;

public static class LocationsAndRoles {
    public static Dictionary<string, string>[] locationSets = new Dictionary<string, string>[] {
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
}
