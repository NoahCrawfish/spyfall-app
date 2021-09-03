using System.Collections.Generic;
using System.Runtime.Serialization;

public static class LocationsAndRoles {
    public static Dictionary<string, string>[] setsData = new Dictionary<string, string>[] {
        new Dictionary<string, string>() {
            { "Airport", "Passenger, Pilot, Air Traffic Controller, Flight Attendant, Janitor, Co-Pilot, Aeronautical Engineer, TSA Security" },
            { "Bar", "Bartender, 21st Birthday, 18-Year-Old, Alcoholic, Party Girl, The Regular, Trivia Team, Lonely Dude" },
            { "Beach", "Sunbather, Treasure Hunter, Drowning Child, Shark, Lifeguard, Coast Guard, Surfer, Fisherman" },
            { "Cemetery", "Great-Great Grandfather, Great-Great Grandmother, Funeral Director, Widow, Widower, Skeleton, Grave Robber, Ghost Hunter" },
            { "Church", "Priest, Pope, Bored Kid, Youth Pastor, Crying Baby, Worship Leader, God, Communion Bread Lover" },
            { "Gas Station", "Cashier, Customer, Manager, Trucker, Thief, Car Wash Attendant, Snack Hoarder, Gotta Pee" },
            { "Gym", "Gym Bro, Newbie, Personal Trainer, Athlete, Nutritionist, Front Desk Staff, Karen, Bodybuilder" },
            { "Harbor", "Sailor, Yacht Salesman, Fisherman, Oil Driller, Captain, Marine Biologist, Mechanic, Lost Child" },
            { "Hospital", "Nurse, Doctor, Surgeon, Medical Student, Physical Therapist, Social Worker, Patient, Diabetic" },
            { "Library", "Librarian, Student, Fell Asleep Studying, Bookworm, Old Lady, Old Man, Loudmouth, Cat" },
            { "Prison", "Guard, Warden, Murderer, Drug Dealer, Innocent, Probation Officer, Inside Trader, Escapee" },
            { "Restaurant", "Chef, Gordan Ramsay, Waiter, Pastry Chef, Food Critic, Angry Customer, Dishwasher, Musician" },
            { "School", "Math Teacher, Bully, Popular Kid, Jock, Nerd, History Teacher, Principal, Lunch Lady" },
            { "Supermarket", "Cashier, Security, Baker, Butcher, Manager, Customer, Delivery Driver, Kid Looking For Mom" },
            { "Train Station", "Ticket Inspector, Conductor, Passenger, Dispatcher, Homeless Person, Locomotive Engineer, Brake Operator, Rail Car Loader" },
            { "Zoo", "Veterinarian, Exhibit Curator, Gift Shop Manager, Zebra, Elephant, Conservation Biologist, Child, Alligator Wrestler" }
        },
        new Dictionary<string, string>() {
            { "Amusement Park", "Ride Technician, Costume Character, Adrenaline Junkie, Nauseous Child, Security, Park Owner, Carnival Game Operator, Magician" },
            { "Art Museum", "Curator, Museum Director, Exhibit Designer, Picasso, Van Gogh, Historian, Art Thief, Visitor" },
            { "Basketball Court", "Videographer, Supportive Parent, Hoop Installer, Point Guard, Shooting Guard, Water Boy, Coach, Cheerleader" },
            { "Circus", "Clown, Juggler, Trapeze Artist, Acrobat, Tightrope Walker, Magician, Strongman, Audience Member" },
            { "Concert", "Drummer, Singer, Guitarist, Fangirl, Fanboy, Crowd Surfer, Tour Bus Driver, Sound Technician" },
            { "Dentist's Office", "Receptionist, Dentist, Dental Assistant, Laboratory Technician, Orthodontist, Oral Surgeon, Tooth Brush, Floss" },
            { "Factory Assembly Line", "CEO, Laborer #1, Laborer #2, Laborer #3, Laborer #4, Laborer #5, Laborer #6, Laborer #7" },
            { "Greenhouse", "Plant Enthusiast, Christmas Tree Trimmer, Hand Sprayer, Hydroponics Worker, Plantscaper, Venus Fly Trap, Tomato, Lettuce" },
            { "MMA Gym", "Kickboxer, Taekwondo Black Belt, Karate Expert, Coach, Jujutsu Lover, Bruce Lee, Mike Tyson, Training Dummy" },
            { "Middle School Sleepover", "Creepy Uncle, Snack Manager, Blanket Fort Builder, Movie Picker, Cool Mom, Sleepover Host, Board Game Organizer, Pillow Fighter" },
            { "Movie Theater", "Visitor, Cashier, Usher, Concessions Attendant, Projectionist, Tom Cruise, Scarlett Johansson, Will Smith" },
            { "Hair Salon", "Shampooist, Apprentice, Stylist, Colorist, Salon Manager, Barber, Receptionist, Waxing Specialist" },
            { "Private Yacht", "Spoiled Kid, Millionaire, Billionaire, Deckhand, Stewardess, Yacht Chef, Captain, Yacht Engineer" },
            { "Sewer System", "Civil Engineer, Alligator, Rat, Dead Pet Fish, Beetle, Sewage, Sewer Inspector, Teenager" },
            { "Spa", "Business Man, Suburban Mom, Massage Therapist, Nail Technician, Spa Manager, Aesthetician, Hot Tub Technician, Receptionist" },
            { "Track Field", "Crowd Member, Sprinter, Announcer, High Jumper, Javelin Thrower, Long Distance Runner, Long Jumper, Gold Medalist" }
        },
        new Dictionary<string, string>() {
            { "Blockcraft", "Miner, Head Builder, Villager, Server Owner, Moderator, Griefer, Farmer, Armorer" },
            { "Triassic Park", "Bait Handler, T-Rex, Velociraptor, Helicopter Pilot, Petting Zoo Attendant, Dinosaur Trainer, Visitor, Guide" },
            { "Death \"Planet\"", "Weapons Engineer, Stormsoldier, Janitor, Emperor, Intelligence Analyst, Hologram Technician, Plumber, Galactic Chef" },
            { "Grand Theft Vehicle", "Bank Robber, Street Racer, Car Salesman, Carjacker, Gas Station Robber, Drug Dealer, Gambler, Gun Salesman" },
            { "Princess Nectarine's Castle", "Main Hero, Main Hero's Brother, Tortoise Villain, Mushroom Guard, Star, Coin, Groundskeeper, Jester" },
            { "Swimsuit Bottom", "SpongeBill SquareShirt, Mr. Lobsters, Plankton, Fish Resident, Pink Starfish, Octopusward, Pet Sea Snail, Squirrel Diver" },
            { "Wacky Chocolate Factory", "Loopa Oopa, Billy Bonka, Blueberry Girl, Silver Ticket Printer, Chocolate Pond Operator, Product Tester, Investor, Fizzy Drink Mixer" },
            { "Wizard School", "Potions Teacher, Scarhead Boy, Sorting Cap, [REDACTED], Headmaster, Beast Trainer, Broom Maker, Wandkeeper" }
        },
        new Dictionary<string, string>() {
            { "Amazon Rainforest", "Tribe Member, Farmer, Cattle Rancher, Logger, Miner, Kayaker, Jaguar, Anaconda" },
            { "Antarctica", "Researcher, Igloo Architect, Snowboarder, Polar Bear, Penguin, Seal, Glaciologist, Traveler" },
            { "Cave", "Scout, Cave Sketcher, Surveyor, Headlamp Manufacturer, Tour Guide, Bat, Navigator, Spelunker" },
            { "Mount Everest", "Marathoner, Rock Climber, Physician, Sherpa Guide, Medic, Gear Manager, Underprepared, Red Bull Athlete" },
            { "Niagara Falls", "Plein Air Artist, Photographer, Daredevil, Man Proposing, Kayaker, Scared Of Water, Videographer, Salmon" },
            { "Sahara Desert", "Dehydrated Traveler, Date Farmer, Camel Guide, Tradesman, Genie, Nomad, Dune Surfer, Scorpion" },
            { "Tropical Island", "Resort Manager, Cliff Diver, Coconut Gatherer, Fire Juggler, Machete Salesman, Wakeboarder, Limbo Expert, Tourist" },
            { "Volcano", "Geologist, Rock Collector, Lava Dragon, Sacrificial Offering, Pompeii Resident, Science Fair Judge, Fossil, Backpacker" }
        },
        new Dictionary<string, string>() {
            { "Colosseum", "Gladiator, Criminal, Emperor, Chariot Racer, Executioner, Spearman, Lion, Crowd Member" },
            { "Eiffel Tower", "Aspiring Architect, Repairman, American Tourist, Parisian, Newlywed, Maintenance, Street Vendor, Translator" },
            { "Great Wall of China", "Chinese General, Long Distance Runner, Chinese Local, Mongolian Soldier, Archer, Swordsman, Tourist, Guard" },
            { "Leaning Tower of Pisa", "Confused Pizza Lover, Tour Guide, Jenga Enthusiast, Italian, Photographer, Unreliable Architect, Bell Ringer, Restorationist" },
            { "Pyramids", "Tomb Raider, Mummy, King Tut, Cleopatra, Embalmer, Pyramid Scheme Scammer, Illuminati Member, Laborer" },
            { "Statue of Liberty", "Ferry Captain, New Yorker, French Tourist, Gift Shop Cashier, Living Statue, Ice Cream Vendor, Street Musician, Pigeon" },
            { "Great Barrier Reef", "Manta Ray, Sea Turtle, Oil Baron, Humpback Whale, Clownfish, Sea Anemone, Whale Watcher, Snorkeler" },
            { "Taj Mahal", "Landscaper, Instagram Influencer, Can't Find Parking, Tomb Maintenance, Sculptor, Indian Tourist, Window Washer, Pool Cleaner" }
        },
        new Dictionary<string, string>() {
            { "Area 51", "Conspiracy Theorist, Military Sniper, President, UFO Designer, Alien, Gate Guard, Alien Weapons Engineer, Extraterrestrial Communications" },
            { "Bermuda Triangle", "Shipwrecked Sailor, Crashed Pilot, Stranded On A Raft, Cthulhu, Kraken, Mermaid, Tropical Storm Chaser, Lost Snorkeler" },
            { "Flat Earth", "Ice Wall Guard, Earth Dome Cleaner, Moon Track Engineer, Star Projectionist, Broke Globe Craftsman, \"Astronaut\", Sun Track Engineer, Round Earth Believer" },
            { "Heaven", "God, Jesus, Angel, Gate Opener, Cloud Fluffer, Earth Watcher, Throne Duster, Rejected Angel" },
            { "Moon Colony", "Solar Panel Installer, Rover Driver, On Vacation, Engineer, Diplomat, Jeff Bezos, Researcher, Alien Parasite" },
            { "Pirate Ship", "First Mate, Blackbeard, Cook, Gunner, Navigator, Lookout, Prisoner, Pet Parrot" },
            { "Santa's Workshop", "Santa Claus, Sleigh Loader, Elf Outcast, Present Wrapper, Toy Painter, Lead Toy Designer, Reindeer, Ms. Claus" },
            { "Time Machine", "Hitchhiking Dinosaur, Confused Caveman, Lead Designer, Timeline Manager, Researcher, DeLorean Engineer, Switch Operator, Time Traveler" }
        }
    };

    public static List<string> setNames = new List<string> {
        "Pack 1",
        "Pack 2",
        "NOT COPYRIGHTED",
        "Nature",
        "Monuments",
        "Fantasy"
    };

    public static string customSetName = "CUSTOM LOCATIONS";
}