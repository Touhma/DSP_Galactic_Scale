using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GalacticScale.Generators
{
    public partial class GS2Generator3 : iConfigurableGenerator
    {
        public static List<string> PlanetNames = new()
        {
            "Achelous",
            "Oceanu",
            "Siren",
            "Acheron",
            "Achilles",
            "Actaeon",
            "Admetus",
            "Adoni",
            "Aeacus",
            "Aeëtes",
            "Aegisthus",
            "Aegyptus",
            "Aeneas",
            "Aeolus",
            "Asclepiu",
            "Agamemnon",
            "Aglaia",
            "Ajax",
            "Alcestis",
            "Alcyone",
            "Pleiades",
            "Furies",
            "Althaea",
            "Amazons",
            "Ero",
            "Amphion",
            "Amphitrite",
            "Amphitryon",
            "Anchises",
            "Andromache",
            "Andromeda",
            "Anteros",
            "Antigone",
            "Antinoüs",
            "Apollo",
            "Aquilo",
            "Arachne",
            "Mars",
            "Argo",
            "Argus",
            "Ariadne",
            "Arion",
            "Artemis",
            "Asclepius",
            "Astarte",
            "Sterop",
            "Astraea",
            "Atalanta",
            "Minerva",
            "Atlas",
            "Atreus",
            "Atropos",
            "Fates",
            "Eo",
            "Auster",
            "Avernus",
            "Dionysu",
            "Bellerophon",
            "Bellona",
            "Boreas",
            "Cadmus",
            "Calliope",
            "Muses",
            "Calypso",
            "Cassandra",
            "Castor",
            "Centaurs",
            "Cephalus",
            "Cepheus",
            "Cerberus",
            "Demete",
            "Chaos",
            "Charon",
            "Charybdis",
            "Chiron",
            "Chryseis",
            "Circe",
            "Clio",
            "Clotho",
            "Clytemnestra",
            "Cyclopes",
            "Daedalus",
            "Danae",
            "Danaïdes",
            "Danaüs",
            "Daphne",
            "Graeae",
            "Ceres",
            "Artemi",
            "Dido",
            "Diomedes",
            "Dione",
            "Bacchus",
            "Dioscuri",
            "Plut",
            "Hade",
            "Dryads",
            "Echo",
            "Electra",
            "Endymion",
            "Enyo",
            "Eos",
            "Aurora",
            "Erato",
            "Erebus",
            "Eris",
            "Eros",
            "Eteocles",
            "Euphrosyne",
            "Europa",
            "Eurus",
            "Gorgons",
            "Eurydice",
            "Eurystheus",
            "Euterpe",
            "Fauns",
            "Faunus",
            "Pa",
            "Favonius",
            "Flora",
            "Fortuna",
            "Gaea",
            "Galatea",
            "Ganymede",
            "Golden Fleece",
            "Graces",
            "Hades",
            "Hamadryads",
            "Harpies",
            "Juventas",
            "Hecate",
            "Hector",
            "Hecuba",
            "Helen",
            "Helios",
            "Vulcan",
            "Hera",
            "Juno",
            "Hercules",
            "Mercury",
            "Hero",
            "Hesperus",
            "Hestia",
            "Vesta",
            "Hippolyte",
            "Amazon",
            "Hippolytus",
            "Hippomenes",
            "Hyacinthus",
            "Hydra",
            "Hyperion",
            "Hypnos",
            "Somnus",
            "Iapetus",
            "Icarus",
            "Iobates",
            "Iphigenia",
            "Iris",
            "Ismene",
            "Ixion",
            "Janus",
            "Jocasta",
            "Zeu",
            "Heb",
            "Laius",
            "Laocoön",
            "Lares",
            "Let",
            "Lavinia",
            "Leda",
            "Lethe",
            "Leto",
            "Latona",
            "Maia",
            "Manes",
            "Marsyas",
            "Medea",
            "Medusa",
            "Meleager",
            "Melpomene",
            "Memnon",
            "Menelaus",
            "Mentor",
            "Herme",
            "Merope",
            "Midas",
            "Athen",
            "Minos",
            "Minotaur",
            "Mnemosyne",
            "Muse",
            "Momus",
            "Morpheus",
            "Thanato",
            "Naiads",
            "Napaeae",
            "Narcissus",
            "Nemesis",
            "Neoptolemus",
            "Poseido",
            "Nestor",
            "Nike",
            "Niobe",
            "Notus",
            "Ny",
            "Nymphs",
            "Nyx",
            "Oceanids",
            "Oceanus",
            "Odysseus",
            "Ulysses",
            "Oedipus",
            "Oenone",
            "Rhe",
            "Orestes",
            "Furie",
            "Orion",
            "Orpheus",
            "Pan",
            "Pandora",
            "Parcae",
            "Paris",
            "Patroclus",
            "Pegasus",
            "Pelias",
            "Pelops",
            "Penates",
            "Penelope",
            "Persephone ",
            "Proserpine",
            "Perseus",
            "Phaedra",
            "Phaethon",
            "Philoctetes",
            "Phineus",
            "Phlegethon",
            "Pirithous",
            "Pollux",
            "Polyhymnia",
            "Polymnia",
            "Polynices",
            "Polyphemus",
            "Polyxena",
            "Pontus",
            "Poseidon",
            "Priam",
            "Priapus",
            "Procrustes",
            "Prometheus",
            "Persephon",
            "Proteus",
            "Psyche",
            "Pygmalion",
            "Pyramus",
            "Python",
            "Quirinus",
            "Rhadamanthus",
            "Rhea",
            "Ops",
            "Romulus",
            "Sarpedon",
            "Cronu",
            "Satyrs",
            "Scylla",
            "Selene",
            "Semele",
            "Sileni",
            "Silvanus",
            "Sirens",
            "Sisyphus",
            "Helio",
            "Hypno",
            "Sphinx",
            "Styx",
            "Symplegades",
            "Syrinx",
            "Tantalus",
            "Tartarus",
            "Telemachus",
            "Tellus",
            "Terminus",
            "Terpsichore",
            "Thalia",
            "Thanatos",
            "Mors",
            "Themis",
            "Theseus",
            "Thisbe",
            "Thyestes",
            "Tiresias",
            "Titans",
            "Tithonus",
            "Triton",
            "Turnus",
            "Odysseu",
            "Urania",
            "Uranus",
            "Aphrodit",
            "Hesti",
            "Hephaestu",
            "Zephyrus",
            "Aeternae",
            "Alcyoneus",
            "Poseidon",
            "Helle",
            "Aloadae",
            "Ares",
            "Amphisbaena",
            "Arae",
            "Argus",
            "Asterius",
            "Athos",
            "Briareus",
            "Catoblepas",
            "Centaur",
            "Centauride",
            "Agrius",
            "Heracles",
            "Amycus",
            "Centauromachy",
            "Asbolus",
            "Bienor",
            "Centaurus",
            "Chiron",
            "heroes",
            "Chthonius",
            "Nestor",
            "Pirithous",
            "Hippodamia",
            "Cyllarus",
            "Dictys",
            "Elatus",
            "Eurynomos",
            "Eurytion",
            "Eurytus",
            "Lapiths",
            "Hylaeus",
            "Atalanta",
            "Hylonome",
            "Nessus",
            "Perimedes",
            "Phólos",
            "Pholus",
            "Rhaecus",
            "Thaumas",
            "Cyprus",
            "Lamos",
            "Zeus",
            "Dionysus",
            "Hera",
            "Amphithemis",
            "Cerastes",
            "Cerberus",
            "Hades",
            "Cetus",
            "Ceuthonymus",
            "Menoetius",
            "Charon",
            "Styx",
            "Charybdis",
            "Chimera",
            "Crocotta",
            "Cyclopes",
            "Arges",
            "Gaia",
            "Uranus",
            "Tartarus",
            "Polyphemus",
            "Hephaestus",
            "Daemons",
            "Ceramici",
            "Damysus",
            "Thrace",
            "Dryad",
            "Echion",
            "Eidolon",
            "Empusa",
            "Enceladus",
            "Athena",
            "Erinyes",
            "Cronus",
            "Virgil",
            "Alecto",
            "Megaera",
            "Tisiphone",
            "Gegenees",
            "Gello",
            "Geryon",
            "Gigantes",
            "Gorgons",
            "Euryale",
            "Medusa",
            "Stheno",
            "Graeae",
            "Griffin",
            "Harpies",
            "Aello",
            "Celaeno",
            "Ocypete",
            "Hecatonchires",
            "Hippalectryon",
            "Hydra",
            "Lerna",
            "Labour",
            "Ichthyocentaurs",
            "Ipotane",
            "Keres",
            "Achlys",
            "Kobaloi",
            "Laestrygonians",
            "Antiphates",
            "Manticore",
            "Mimas",
            "Minotaur",
            "Theseus",
            "Hellhound",
            "Orthrus",
            "Odontotyrannos",
            "Onocentaur",
            "Ophiotaurus",
            "Orion",
            "Ouroboros",
            "Pallas",
            "Panes",
            "Periboea",
            "Giantess",
            "Phoenix",
            "Polybotes",
            "Porphyrion",
            "Satyrs",
            "Satyresses",
            "Pan",
            "Agreus",
            "Ampelos",
            "Marsyas",
            "Nomios",
            "Silenus",
            "Scylla",
            "nereid",
            "Circe",
            "Sirens",
            "Medea",
            "Pasiphaë",
            "Argo",
            "Sphinx",
            "Hieracosphinx",
            "Taraxippi",
            "Thoon",
            "Tityos",
            "Triton",
            "Amphitrite",
            "Typhon",
            "Monocerata",
            "Lamiai",
            "Empousa",
            "Lamia",
            "Mormo",
            "Mormolykeia",
            "Hecate",
            "Agriopas",
            "Damarchus",
            "Parrhasia",
            "Lykaia",
            "Lycaon",
            "Nyctimus",
            "Pegasus",
            "Acanthis",
            "Carduelis",
            "Alectryon",
            "Aphrodite",
            "Helios",
            "Autonous",
            "Gerana",
            "Oenoe",
            "Aethon",
            "Lark",
            "Alcyone",
            "Alkyonides",
            "Halcyon",
            "Ceyx",
            "Aëdon",
            "Procne",
            "Nyctimene",
            "Ascalaphus",
            "Philomela",
            "Cornix",
            "Coronis",
            "Corvus",
            "Apollo",
            "Cycnus",
            "Phaethon",
            "Eridanos",
            "Strix",
            "Tereus",
            "Hoopoe",
            "Ionia",
            "Crommyon",
            "Gadflies",
            "Myrmekes",
            "Menoetes",
            "Cercopes",
            "Actaeon",
            "Argos",
            "Odysseus",
            "Laelaps",
            "Maera",
            "Erigone",
            "Arion",
            "Taras",
            "Styx",
            "Achilles",
            "Balius",
            "Xanthus",
            "Ladon",
            "Nemea",
            "Mysia",
            "Trojan",
            "Laocoön",
            "Aeëtes",
            "Talos",
            "Aella"
        };

        private void RemoveRaresFromStartingSystem()
        {
            // GS2.Warn($"RemoveRares Running");
            foreach (var p in birthStar.Bodies)
            {
                // GS2.Warn($"RemoveRares {p.Name}");
                if (p.veinSettings == null || p.veinSettings == new GSVeinSettings())
                    // GS2.Warn($"RemoveRares Initializing Veins for {p.Name}");
                    p.veinSettings = p.GsTheme.VeinSettings.Clone();

                var newTypes = new GSVeinTypes();
                foreach (var v in p.veinSettings.VeinTypes)
                    switch (v.type)
                    {
                        case EVeinType.Bamboo:
                        case EVeinType.Crysrub:
                        case EVeinType.Diamond:
                        case EVeinType.Fractal:
                        case EVeinType.Grat:
                        case EVeinType.Mag:
                            GS2.Log($"RemoveRares Removing {v.type} from {p.Name}");
                            v.veins = new List<GSVein>();

                            // GS2.LogJson(v);

                            break;
                        default:
                            newTypes.Add(v);
                            break;
                    }

                p.veinSettings.VeinTypes = newTypes;
            }
        }

        private void GeneratePlanets()
        {
            foreach (var star in GSSettings.Stars)
            {
                if (!star.Decorative) GeneratePlanetsForStar(star);
                if (!star.Decorative) NamePlanets(star);
            }
        }

        private void CreateComet(GSStar star)
        {
            var comet = new GSPlanet();
            comet.Name = random.Item(PlanetNames);

            comet.Radius = Utils.ParsePlanetSize(random.NextFloat(29, 41));
            comet.Theme = random.Item(cometThemes).Name;
            comet.OrbitInclination = 66f;
            if (star.Planets.Count > 0) comet.OrbitRadius = star.Planets[star.Planets.Count - 1].OrbitRadius + 0.5f;
            else return;
            comet.Luminosity = 0f;
            comet.OrbitalPeriod = Utils.CalculateOrbitPeriod(comet.OrbitRadius);
            star.Planets.Add(comet);
        }

        private void GeneratePlanetsForStar(GSStar star)
        {
            star.Planets = new GSPlanets();
            // Warn($"Creating Planets for {star.Name}");
            var random = new GS2.Random(star.Seed);

            var starBodyCount = GetStarPlanetCount(star);
            if (starBodyCount == 0) return;
            var moonChance = GetMoonChanceForStar(star);
            if (starBodyCount == 1) moonChance = 0;
            var gasChance = GetGasChanceForStar(star);
            var subMoonChance = 0.0;
            if (preferences.GetBool("secondarySatellites"))
                subMoonChance = preferences.GetFloat("chanceMoonMoon", 5f) / 100f;
            var moonBias = preferences.GetFloat("MoonBias", 50f);
            //moonChance = moonChance - subMoonChance;

            var isBirthStar = star == birthStar;

            var startOnMoon = isBirthStar && preferences.GetBool("birthPlanetMoon");
            var startIsMoonOfGas = isBirthStar && startOnMoon && preferences.GetBool("birthPlanetGasMoon");


            var birthPlanetSize = preferences.GetInt("birthPlanetSize", 200);

            var gasCount = Math.Max(startIsMoonOfGas ? 1 : 0, Mathf.RoundToInt(starBodyCount * (float)gasChance));
            var telluricCount = Math.Max(isBirthStar ? 1 : 0, starBodyCount - gasCount);
            var moonCount = Math.Max(startOnMoon ? 1 : 0, Mathf.RoundToInt(telluricCount * (float)moonChance));
            telluricCount -= moonCount;
            var secondaryMoonCount = moonCount > 1 ? Mathf.RoundToInt((moonCount - 1) * (float)subMoonChance) : 0;
            moonCount -= secondaryMoonCount;
            if (moonCount == 0 && secondaryMoonCount > 0 && startOnMoon)
            {
                moonCount += 1;
                secondaryMoonCount -= 1;
            }
            var gasPlanets = new GSPlanets();
            var telPlanets = new GSPlanets();
            var moons = new GSPlanets();
            var singlePlanet = starBodyCount == 1;
            if (singlePlanet)
            {
                GS2.Log(
                    $"Single Planet. Ignoring Settings. Original: starBodyCount:{starBodyCount} gasCount:{gasCount} telluricCount:{telluricCount} moonCount:{moonCount} startOnMoon:{startOnMoon}");
                if (isBirthStar)
                {
                    gasCount = 0;
                    telluricCount = 1;
                }

                moonCount = 0;
                secondaryMoonCount = 0;
                startOnMoon = false;
                startIsMoonOfGas = false;
            }

            for (var i = 0; i < telluricCount - (isBirthStar && !startOnMoon ? 1 : 0); i++)
            {
                var radius = GetStarPlanetSize(star);
                var p = new GSPlanet("planet_" + i, "Barren", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                p.genData.Add("hosttype", "star");
                p.genData.Add("hostname", star.Name);
                telPlanets.Add(p);
            }

            for (var i = 0; i < gasCount; i++)
            {
                var radius = Mathf.RoundToInt(GetStarGasSize(star) / 10f);
                var p = new GSPlanet("planet_" + i, "Gas", radius, -1, -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                if (!preferences.GetBool("hugeGasGiants", true)) p.Radius = 80;
                p.Scale = 10f;
                p.genData.Add("hosttype", "star");
                p.genData.Add("hostname", star.Name);
                gasPlanets.Add(p);
            }

            for (var i = 0; i < moonCount - (isBirthStar && startOnMoon ? 1 : 0); i++)
            {
                GS2.Log(
                    $"Adding Moon {i + 1}/{moonCount} with bias {moonBias}. Available Hosts:{gasPlanets.Count} Gas / {telPlanets.Count}");
                GSPlanet randomPlanet;
                var hostGas = random.NextPick(moonBias / 100f);

                if (gasPlanets.Count > 0 && hostGas)
                {
                    GS2.Log($"Picking Host Gas Planet {gasPlanets.Count}/{telPlanets.Count}");
                    randomPlanet = random.Item(gasPlanets);
                }
                else if (telPlanets.Count > 0)
                {
                    GS2.Log("Picking Host Telluric Planet");
                    randomPlanet = random.Item(telPlanets);
                }
                else if (gasPlanets.Count > 0)
                {
                    GS2.Log("Picking Host Gas Planet Due to no Telluric Planets");
                    randomPlanet = random.Item(gasPlanets);
                }
                else
                {
                    var radius = GetStarPlanetSize(star);
                    GS2.Log("Picking No Host Planet");
                    randomPlanet = new GSPlanet("planet_" + i, "Barren", radius, -1, -1, -1, -1, -1, -1, -1, -1,
                        new GSPlanets());
                    randomPlanet.genData.Add("hosttype", "star");
                    randomPlanet.genData.Add("hostname", star.Name);
                    telPlanets.Add(randomPlanet);
                }

                var moon = new GSPlanet("Moon " + i, "Barren", GetStarMoonSize(star, randomPlanet.Radius, hostGas), -1,
                    -1, -1, -1, -1, -1, -1, -1, new GSPlanets());
                randomPlanet.Moons.Add(moon);
                moon.genData.Add("hosttype", "planet");
                moon.genData.Add("hostname", randomPlanet.Name);
                moons.Add(moon);
                GS2.Log($"Added {moon} to {randomPlanet}");
            }
            if (isBirthStar)
            {
                birthPlanet = new GSPlanet("BirthPlanet", "Mediterranean", birthPlanetSize, -1, -1, -1, -1, -1, -1, -1,
                    -1, new GSPlanets());
                if (startIsMoonOfGas)
                {
                    var gasHost = random.Item(gasPlanets);
                    gasHost.Moons.Add(birthPlanet);
                    moons.Add(birthPlanet);
                    GS2.Log($"Added Birthplanet {birthPlanet.Name} to gas host {gasHost.Name}");
                }
                else if (startOnMoon)
                {
                    var moonHost = random.Item(telPlanets);
                        moonHost.Moons.Add(birthPlanet);
                    moons.Add(birthPlanet);
                    GS2.Log($"Added Birthplanet {birthPlanet.Name} to moon host {moonHost.Name}");
                }
                else
                {
                    GS2.Log($"Added Birthplanet {birthPlanet.Name} to star host {star.Name}");
                    telPlanets.Add(birthPlanet);
                }
                birthPlanet.genData.Set("birthPlanet", true);
                GS2.Log($"Created Birth Planet in star {star.Name}: {birthPlanet}");
            }
            for (var i = 0; i < secondaryMoonCount; i++)
            {
                // GS2.Log($"Picking Moon to host Secondary {gasPlanets.Count}/{telPlanets.Count}/{moonCount}/{secondaryMoonCount}");
                var randomMoon = random.Item(moons);
                var mm = new GSPlanet("MoonMoon" + i, "Barren", GetStarMoonSize(star, randomMoon.Radius, false), -1, -1,
                    -1, -1, -1, -1, -1, -1);
                mm.genData.Add("hosttype", "moon");
                mm.genData.Add("hostname", randomMoon.Name);
                randomMoon.Moons.Add(mm);
                if (preferences.GetBool("moonCeption", false)) moons.Add(mm);
                
                // GS2.Log($"Added {mm} {mm.Radius} to {randomMoon.Name}");
            }



            foreach (var p in telPlanets)
                star.Planets.Add(p);

            foreach (var p in gasPlanets)
                star.Planets.Add(p);


            // GS2.WarnJson((from s in star.Planets select s.Name).ToList());
            // GS2.Warn($"Now Assigning Moon Orbits {(birthPlanet != null ? birthPlanet.Name : "null")}");
            AssignMoonOrbits(star);
            // GS2.Warn($"Now Assigning Planet Orbits {(birthPlanet != null ? birthPlanet.Name : "null")}");
            AssignPlanetOrbits(star);
            // GS2.Warn($"Now Assigning Themes {(birthPlanet != null ? birthPlanet.Name : "null")}");
            SelectPlanetThemes(star);
            // GS2.Warn($"Now assigning parameters {(birthPlanet != null ? birthPlanet.Name : "null")}");
            FudgeNumbersForPlanets(star);
            // GS2.Warn("Done");
            AssignVeinSettings(star);
            // GS2.Log($"Assigning Vein Settings for {star.Name}");
        }

        private void AssignVeinSettings(GSStar star)
        {
            foreach (var p in star.Bodies)
                if (p.veinSettings == null || p.veinSettings == new GSVeinSettings())
                {
                    // GS2.Log($"Vein Settings missing for planet {p.Name} with theme {p.GsTheme.Name}. Cloning...");
                    p.veinSettings = p.GsTheme.VeinSettings.Clone();
                }
        }

        private void FudgeNumbersForPlanets(GSStar star)
        {
            // GS2.Warn($"{star.displayType} {star.radius} {star.RadiusAU} {star.luminosity}");
            // GS2.Warn("Star RadiusAU, Star Luminance, HZMin, HZMax, HZ, Orbit Radius, LumInv, LumLin , intensity ");
            foreach (var body in star.Bodies)
            {
                body.RotationPhase = random.Next(360);
                if (GS2.IsPlanetOfStar(star, body))
                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = random.NextFloat() * 4 + random.NextFloat() * 5;
                if (!GS2.IsPlanetOfStar(star, body))
                    // GS2.Warn($"SETTING Orbit Inclination of {body.Name} to random");
                    body.OrbitInclination = random.NextFloat() * 50f;
                body.OrbitPhase = random.Next(360);
                body.Obliquity = random.NextFloat() * 20;
                var starInc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}inclination");
                var starLong = preferences.GetFloat($"{GetTypeLetterFromStar(star)}orbitLongitude", 0);
                if (starLong == -1)
                    body.OrbitLongitude = random.NextFloat() * 360f;
                else
                    body.OrbitLongitude = random.NextFloat() * starLong;
                // GS2.Log($"StarInc {starInc}");
                if (GS2.IsPlanetOfStar(star, body) && starInc > -1)
                {
                    // GS2.Warn($"SETTING starInc Orbit Inclination of {star.Name} to {starInc}");
                    if (starInc > 0) body.OrbitInclination = random.NextFloat(starInc);
                    else body.OrbitInclination = 0;
                }

                body.RotationPeriod = preferences.GetFloat("rotationMulti", 1f) * random.Next(60, 3600);
                if (random.NextDouble() < 0.02) body.OrbitalPeriod = -1 * body.OrbitalPeriod; // Clockwise Rotation
                var innerPlanetDistanceForStar = GetInnerPlanetDistanceForStar(star);
                // GS2.Warn($"{innerPlanetDistanceForStar} for star {star.Name} {star.displayType}");
                if (GS2.IsPlanetOfStar(star, body) && body.OrbitRadius < innerPlanetDistanceForStar &&
                    (random.NextFloat() < 0.5f || preferences.GetBool("tidalLockInnerPlanets")))
                    body.RotationPeriod = body.OrbitalPeriod; // Tidal Lock
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 1.5f &&
                         random.NextFloat() < 0.2f)
                    body.RotationPeriod = body.OrbitalPeriod / 2; // 1:2 Resonance
                else if (preferences.GetBool("allowResonances", true) && body.OrbitRadius < 2f &&
                         random.NextFloat() < 0.1f)
                    body.RotationPeriod = body.OrbitalPeriod / 4; // 1:4 Resonance
                if (random.NextDouble() < 0.05) // Crazy Obliquity
                    body.Obliquity = random.NextFloat(20f, 85f);
                if (starInc == -1 && random.NextDouble() < 0.05) // Crazy Inclination
                    // GS2.Warn("Setting Crazy Inclination for " + star.Name);
                    body.OrbitInclination = random.NextFloat(20f, 85f);

                var rc = preferences.GetFloat($"{GetTypeLetterFromStar(star)}rareChance");
                if (rc > 0f) body.rareChance = rc / 100f;
                else body.rareChance = rc;

                // Force inclinations for testing
                // body.OrbitInclination = 0f;
                // body.OrbitPhase = 0f;
                // body.OrbitalPeriod = 10000000f;
                if (body == birthPlanet)
                    if (preferences.GetBool("birthTidalLock"))
                        body.RotationPeriod = body.OrbitalPeriod;
                var oRadius = 1f;
                if (GS2.IsPlanetOfStar(star, body))
                    oRadius = body.OrbitRadius;
                else
                    foreach (var p in star.Planets)
                        if (GS2.IsMoonOfPlanet(p, body, true))
                            oRadius = p.OrbitRadius;

                // var starLum = Mathf.Pow((Mathf.Pow(star.luminosity, 0.33f)*preferences.GetFloat("luminosityBoost")),3);
                var solarRange = preferences.GetFloatFloat("solarRange", new FloatPair(1, 500));

                // var minSolar = solarRange.low / 100f;
                // var maxSolar = solarRange.high / 100f;
                // // oRadius += star.RadiusAU;
                // float minHZ = star.genData.Get("minHZ", 1);
                // float maxHZ = star.genData.Get("maxHZ", 100f);
                // var hz = (maxHZ - minHZ) / 2 + minHZ;
                // var oSquared = oRadius * oRadius;
                // var hzSquared = hz * hz;
                // var distance = hzSquared / oSquared;
                // var intensity = Mathf.Pow(distance,0.5f) * Mathf.Pow( starLum, 0.33f);
                //
                // //intensity1 x distance1squared = intensity2 x distance2squared
                var minSolar = solarRange.low / 100f;
                var maxSolar = solarRange.high / 100f;

                // oRadius += star.RadiusAU;
                float minHZ = star.genData.Get("minHZ", 1);
                float maxHZ = star.genData.Get("maxHZ", 100f);
                var hz = (maxHZ - minHZ) / 2 + minHZ;
                var oSquared = oRadius * oRadius;
                var hzSquared = hz * hz;
                var intensity = hzSquared / oSquared;
                // var intensity = Mathf.Pow(distance,0.5f) * Mathf.Pow( starLum, 0.33f);

                //1 x hzsquared = intensity2 x oRadiussquared
                //hzSquared / oRadsquared = intensity2;

                var lumInverse = Mathf.Clamp(intensity, minSolar, maxSolar);
                var lumNone = Mathf.Clamp(star.luminosity, minSolar, maxSolar);
                var lumLinear = Mathf.Clamp(1 / (Mathf.Lerp(oRadius, hz, preferences.GetFloat("solarLerp", 0.5f)) / hz),
                    minSolar, maxSolar);
                // GS2.Warn($"Lerping from {oRadius} to {hz} by {preferences.GetFloat("solarLerp",  0.5f)} = {Mathf.Lerp(oRadius, hz, preferences.GetFloat("solarLerp", 0.5f))} ");
                switch (preferences.GetString("solarScheme", "Linear"))
                {
                    case "None":
                        body.Luminosity = lumNone;
                        break;
                    case "InverseSquare":
                        body.Luminosity = lumInverse;
                        break;
                    default:
                        body.Luminosity = lumLinear;
                        break;
                }

                // GS2.Warn($"{star.RadiusAU}, {starLum}, {minHZ}, {maxHZ}, {hz}, {oRadius}, {lumInverse}, {lumLinear}, {intensity}  ");
            }
        }

        private float GetNextAvailableOrbit(GSPlanet planet, int moonIndex)
        {
            var moons = planet.Moons;
            if (moonIndex == 0) return planet.RadiusAU + moons[moonIndex].SystemRadius;
            return moons[moonIndex - 1].SystemRadius + moons[moonIndex - 1].OrbitRadius + moons[moonIndex].SystemRadius;
        }

        private void AssignMoonOrbits(GSStar star)
        {
            // Now Work Backwards from secondary Satellites to Planets, creating orbits.
            for (var planetIndex = 0; planetIndex < star.PlanetCount; planetIndex++)
            {
                var planet = star.Planets[planetIndex];
                //For each Planet
                for (var moonIndex = 0; moonIndex < planet.MoonCount; moonIndex++)
                {
                    var moon = planet.Moons[moonIndex];
                    //For Each Moon
                    for (var moon2Index = 0; moon2Index < moon.MoonCount; moon2Index++)
                    {
                        //for each subsatellite
                        var moon2 = moon.Moons[moon2Index];
                        moon2.OrbitRadius = GetMoonOrbit() / 2f + GetNextAvailableOrbit(moon, moon2Index);
                        moon2.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon2.OrbitRadius);
                    }

                    moon.OrbitRadius = GetMoonOrbit() + GetNextAvailableOrbit(planet, moonIndex);
                    moon.OrbitalPeriod = Utils.CalculateOrbitPeriod(moon.OrbitRadius);
                }
            }
            //Orbits should be set.
            // GS2.Log($"Orbits Set {(birthPlanet != null ? birthPlanet.Name : "null")}");
        }


        private float GetMoonOrbit()
        {
            return 0.01f + random.NextFloat(0f, 0.05f);
        }

        public void SelectPlanetThemes(GSStar star)
        {
            foreach (var planet in star.Planets)
            {
                var heat = CalculateThemeHeat(star, planet.OrbitRadius);
                var type = EThemeType.Planet;
                if (planet != birthPlanet)
                {
                    if (planet.Scale == 10f) type = EThemeType.Gas;
                    planet.Theme = GSSettings.ThemeLibrary.Query(random, type, heat, planet.Radius);
                }
                else
                {
                    // GS2.Warn($"Setting Theme for BirthPlanet {birthPlanet.Name}");
                    var habitableTheme = GSSettings.ThemeLibrary.Query(random, EThemeType.Telluric,
                        EThemeHeat.Temperate, preferences.GetInt("birthPlanetSize", 200), EThemeDistribute.Default,
                        true);
                    if (preferences.GetBool("birthPlanetUnlock")) planet.Theme = habitableTheme;
                    else planet.Theme = "Mediterranean";
                    planet.Scale = 1f;
                }

                //GS2.Warn($"Planet Theme Selected. {planet.Name}:{planet.Theme} Radius:{planet.Radius * planet.Scale} {((planet.Scale == 10f) ? EThemeType.Gas : EThemeType.Planet)}");
                foreach (var body in planet.Bodies)
                    if (body != planet)
                    {
                        if (body != birthPlanet)
                        {
                            body.Theme = GSSettings.ThemeLibrary.Query(random, EThemeType.Moon, heat, body.Radius);
                        }
                        else
                        {
                            var habitableTheme = GSSettings.ThemeLibrary.Query(random, EThemeType.Moon,
                                EThemeHeat.Temperate, preferences.GetInt("birthPlanetSize", 200),
                                EThemeDistribute.Default,
                                true);
                            if (preferences.GetBool("birthPlanetUnlock")) body.Theme = habitableTheme;
                            else body.Theme = "Mediterranean";
                            body.Scale = 1f;
                        }
                    }
                //Warn($"Set Theme for {body.Name} to {body.Theme}");
            }
            // GS2.Log($"Themes Set {(birthPlanet != null ? birthPlanet.Name : "null")}");
        }

        public bool CalculateIsGas(GSStar star)
        {
            var gasChance = GetGasChanceForStar(star);
            return random.NextPick(gasChance);
        }

        public static EThemeHeat CalculateThemeHeat(GSStar star, float OrbitRadius)
        {
            (float min, float max) hz = (star.genData.Get("minHZ"), star.genData.Get("maxHZ"));
            (float min, float max) orbit = (star.genData.Get("minOrbit"), star.genData.Get("maxOrbit"));
            var frozenOrbitStart = (orbit.max - hz.max) / 2 + hz.max;
            if (OrbitRadius < hz.min / 2) return EThemeHeat.Hot;
            if (OrbitRadius < hz.min) return EThemeHeat.Warm;
            if (OrbitRadius < hz.max) return EThemeHeat.Temperate;
            if (OrbitRadius < frozenOrbitStart) return EThemeHeat.Cold;
            return EThemeHeat.Frozen;
        }
    }
}