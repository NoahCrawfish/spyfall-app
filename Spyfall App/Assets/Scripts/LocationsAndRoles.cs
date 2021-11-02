using System.Collections.Generic;

public static class LocationsAndRoles {
    public static Dictionary<string, string>[] setsData = new Dictionary<string, string>[] {
        // pack 1
        new Dictionary<string, string>() {
            { "Airport", "Passenger, Pilot, Air Traffic Controller, Flight Attendant, Janitor, Hijacker, Airplane Mechanic, TSA Security" },
            { "Bar", "Bartender, 21st Birthday, Underage, Alcoholic, Party Girl, The Regular, Darts Hustler, Loner" },
            { "Beach", "Sunbather, Treasure Hunter, Drowning Child, Shark, Lifeguard, Sand Sculptor, Surfer, Fisherman" },
            { "Cemetery", "Orphan, Funeral Director, Widow, Dead Body, Grave Robber, Ghost Hunter, Beneficiary, Mourning Brother" },
            { "Church", "Priest, Satanist, Bored Kid, Youth Pastor, Crying Baby, Confessor, God, Usher" },
            { "Gas Station", "Cashier, Customer, Trucker, Mugger, Car Wash Attendant, Snack Hoarder, Gotta Pee, Road Tripper" },
            { "Gym", "Gym Bro, Beginner, Personal Trainer, Athlete, Nutritionist, Front Desk Staff, Gym Creep, Powerlifter" },
            { "Harbor", "Sailor, Yacht Salesman, Fisherman, Captain, Marine Biologist, Ship Mechanic, Wannabe Pirate, Bird Watcher" },
            { "Hospital", "Nurse, Doctor, Surgeon, Medical Student, Radiologist, Social Worker, Patient, Janitor" },
            { "Library", "Librarian, Scholar, Cram-Studier, Bookworm, Old Person, Loudmouth, Cat, Asleep" },
            { "Prison", "Guard, Warden, Murderer, Drug Dealer, Innocent, Probation Officer, Inside Trader, Escapee" },
            { "Restaurant", "Chef, Gordan Ramsay, Waiter, Food Critic, Angry Customer, Dishwasher, On A Date, Dine And Dasher" },
            { "School", "Math Teacher, Bully, Popular Kid, Jock, Nerd, Art Teacher, Principal, Lunch Lady" },
            { "Supermarket", "Cashier, Shoplifter, Baker, Thrifter, Manager, Vegan, Cart Collector, Lost Child" },
            { "Train Station", "Ticket Inspector, Conductor, Passenger, Homeless, Train Dispatcher, Hitchhiker, Cargo Loader, Busker" },
            { "Zoo", "Veterinarian, Exhibit Engineer, Gift Shop Manager, Elephant, Dolphin, Conservation Biologist, Parent, On A Field Trip" }
        },
        // pack 2
        new Dictionary<string, string>() {
            { "Amusement Park", "Ride Technician, Costume Character, Adrenaline Junkie, Nauseous Child, Security, Park Owner, Carnival Game Operator, Magician" },
            { "Art Museum", "Art Critic, Museum Director, Historian, Picasso, Auctioneer, Tour Guide, Art Thief, Visitor" },
            { "Basketball Court", "Dodgeball Player, Supportive Parent, First Time Playing, Benchwarmer, Coach, Cheerleader, Michael Jordan, Team MVP" },
            { "Circus", "Clown, Juggler, Trapeze Artist, Acrobat, Tightrope Walker, Palm Reader, Strongman, Bearded Woman" },
            { "Concert", "Drummer, Singer, Guitarist, Fan, Drug Dealer, Crowd Surfer, Tour Bus Driver, Sound Technician" },
            { "Dentist's Office", "Receptionist, Dentist, Dental Assistant, Getting Braces, Orthodontist, Oral Surgeon, Terrified Kid, Tooth Fairy" },
            { "Factory Assembly Line", "CEO, Laborer #1, Laborer #2, Laborer #3, Laborer #4, Laborer #5, Laborer #6, Laborer #7" },
            { "Greenhouse", "Tree Hugger, Harvester, Plant Waterer, Butterfly, Landscaper, Venus Fly Trap, Interior Decorator, Buying A Bouquet" },
            { "MMA Gym", "Kickboxer, Janitor, Karate Black Belt, Coach, Jujutsu Lover, Bruce Lee, White Belt, Training Dummy" },
            { "Middle School Sleepover", "Creepy Uncle, Snack Manager, Blanket Fort Builder, Movie Picker, Cool Mom, Birthday Boy, Board Game Organizer, Pillow Fighter" },
            { "Movie Theater", "Lovebird, Cashier, Janitor, Concessions Attendant, Film Buff, Movie Star, Horror Fanatic, Claps During Credits" },
            { "Hair Salon", "Actor, Apprentice, Balding Man, Colorist, Barber, Receptionist, Gossiper, Scruffy Man" },
            { "Private Yacht", "Spoiled Kid, Millionaire, Family Friend, Stewardess, Yacht Chef, Captain, Party Animal, Supermodel" },
            { "Sewer System", "Alligator, Rat, Dead Pet Fish, Sewage, Sewer Inspector, Samurai Turtle, Graffiti Artist, Runaway Criminal" },
            { "Spa", "Stressed Out, Suburban Mom, Massage Therapist, Nail Technician, Janitor, Receptionist, Spa Owner, Sauna Operator" },
            { "Track Field", "Crowd Member, Runner, Announcer, Pole Vaulter, Javelin Thrower, Last Place, Long Jumper, Gold Medalist" }
        },
        // adventure
        new Dictionary<string, string>() {
            { "Bank Heist", "Getaway Driver, Hacker, Hostage, Bank Teller, SWAT Officer, Lookout, Hostage Negotiator, Reporter" },
            { "Bermuda Triangle", "Shipwrecked Sailor, Crashed Pilot, Stranded On A Raft, Government Researcher, Kraken, Mermaid, Tropical Storm Chaser, Lost Snorkeler" },
            { "Haunted House", "Possessed Doll, Youtuber, Peer Pressured, Paranormal Investigator, Medium, Ghost, Skepticist, Vampire" },
            { "Lucid Dream", "You, Your Best Friend, Your Mom, Your Dad, Your Loved One, Your Worst Fear, Your Subconcious, Dream NPC" },
            { "Medieval Castle", "King, Queen, Peasant, Jester, Knight, Prisoner, Archer, Armorer" },
            { "Pirate Ship", "First Mate, Blackbeard, Cook, Gunner, Navigator, Lookout, Deckhand, Pet Parrot" },
            { "Wild West Town", "Outlaw, Sheriff, Miner, Bartender, Cowboy, Shopkeeper, Bounty Hunter, Mayor" },
            { "WWI Trench", "Medic, Deserter, General, Weapons Cleaner, Artillery Operator, Air Bomber, Drafted Soldier, Grave Digger" }
        },
        // fantasy
        new Dictionary<string, string>() {
            { "Dragon Lair", "Knight, Dragon, Damsel, Greedy Gold Seeker, Village Outcast, Wizard, Dragon Tamer, Knight's Horse" },
            { "Enchanted Forest", "Troll, Unicorn, Dwarf, Werewolf, Witch, Talking Tree, Fairy, Adventurer" },
            { "Flat Earth", "Ice Wall Guard, Earth Dome Cleaner, Moon Track Engineer, Star Projectionist, \"Astronaut\", Seasons Operator, Round Earth Believer" }, // 7
            { "Heaven", "God, Jesus, Angel, Gate Opener, Cloud Fluffer, Fate Decider, Throne Duster, Rejected Angel" },
            { "Hell", "Devil, Pride, Envy, Wrath, Sloth, Greed, Gluttony, Lust" },
            { "Santa's Workshop", "Santa Claus, Sleigh Loader, Naughty List Writer, Present Wrapper, Toy Painter, Lead Toy Designer, Reindeer, Ms. Claus" },
            { "Swimsuit Bottom", "SpongeBill SquareShirt, Mr. Lobsters, Plankton, Fish Resident, Pink Starfish, Octopusward, Pet Sea Snail, Squirrel Diver" },
            { "Wizard School", "Potions Teacher, Elf, Sorting Cap, He-Who-Must-Not-Be-Talked-About, Headmaster, Beast Trainer, Broom Athlete, Wandkeeper" }
        },
        // nature
        new Dictionary<string, string>() {
            { "Amazon Rainforest", "Tribe Member, Farmer, Lost Traveler, Logger, Kayaker, Mosquito, Anaconda" }, // 7
            { "Antarctica", "Researcher, Igloo Architect, Polar Bear, Penguin, Traveler, Skier, Ice Climber, Photographer" },
            { "Cave", "Route Mapper, Miner, Tour Guide, Bat, Navigator, Scout, Adrenaline Junkie, Claustrophobic" },
            { "Mount Everest", "Marathoner, Rock Climber, Physician, Sherpa Guide, Medic, Gear Manager, Underprepared, Red Bull Athlete" },
            { "Niagara Falls", "Artist, Photographer, Daredevil, Man Proposing, Kayaker, Scared Of Water, Salmon" }, // 7
            { "Sahara Desert", "Out Of Water, Sunburnt, Camel Guide, Tradesman, Genie, Nomad, Dune Surfer, Scorpion" },
            { "Tropical Island", "Resort Manager, Cliff Diver, Coconut Gatherer, Fire Juggler, Castaway, Wakeboarder, Limbo Expert, Reality TV Contestant" },
            { "Volcano", "Geologist, Rock Collector, Sacrificial Offering, Pompeii Resident, Science Fair Judge, Backpacker, Pyromaniac" } // 7
        },
        // monuments
        new Dictionary<string, string>() {
            { "Colosseum", "Gladiator, Criminal, Emperor, Chariot Racer, Executioner, Lion, Crowd Member, Animal Trainer" },
            { "Eiffel Tower", "Architect, Repairman, American Tourist, Parisian, Newlywed, Accordion Player, Street Vendor, Translator" },
            { "Great Wall of China", "Chinese General, Wall Builder, Chinese Local, Mongolian Soldier, Archer, Tourist, Guard" }, // 7
            { "Leaning Tower of Pisa", "Confused Pizza Lover, Tour Guide, Jenga Enthusiast, Italian, Photographer, Posing Tourist, Restorationist" }, // 7
            { "Pyramids", "Tomb Raider, Mummy, King Tut, Cleopatra, Embalmer, Tourist, Egyptian, Sphinx" },
            { "Statue of Liberty", "Ferry Captain, New Yorker, French Tourist, Gift Shop Cashier, Living Statue, Street Musician, Pigeon" }, // 7
            { "Great Barrier Reef", "Manta Ray, Sea Turtle, Oil Baron, Humpback Whale, Clownfish, Sea Anemone, Whale Watcher, Snorkeler" },
            { "Taj Mahal", "Landscaper, Instagram Influencer, Photographer, Tomb Maintenance, Sculptor, Indian Tourist, Window Washer, Pool Cleaner" }
        },
        // sci-fi
        new Dictionary<string, string>() {
            { "Alien Planet", "Diplomat, Biologist, Translator, Physicist, Tentacle Alien, Spaceship Pilot, Humanoid Alien, Alien Leader" },
            { "Apocalypse Shelter", "Group Leader, Raider, Scout, Guard, Zombie, Lone Wolf, Medic" }, // 7
            { "Area 51", "Conspiracy Theorist, President, UFO Designer, Alien, Gate Guard, Weapons Engineer, Alien Translator" }, // 7
            { "Death \"Planet\"", "Weapons Engineer, Stormsoldier, Janitor, Emperor, Darth Helmet, Space Princess, D-3PO, Galactic Chef" },
            { "Moon Colony", "Moon Native, Rover Driver, Vacationing, Engineer, Researcher, Alien Parasite, Military" }, // 7
            { "Robot Rights Protest", "Protest Organizer, Police Officer, Human Activist, Self-Driving Car, Sentient Toaster, Counter-Protester, Roomba, Rioter" },
            { "Time Machine", "Dinosaur, Confused Caveman, Mad Scientist, Timeline Manager, Researcher, News Reporter, Future You, Time Traveler" },
            { "Triassic Park", "T-Rex, Triceratops, Park Investor, Petting Zoo Visitor, Dinosaur Trainer, Archaeologist, Guide" } // 7
        }
    };

    public static List<string> setNames = new List<string> {
        "Pack 1",
        "Pack 2",
        "Adventure",
        "Fantasy",
        "Nature",
        "Monuments",
        "Sci-Fi"
    };

    public static string customSetName = "CUSTOM";
}