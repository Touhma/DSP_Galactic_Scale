﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;
using static GalacticScale.GS2;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        private void AssignPlanetOrbits(GSStar star)
        {
            var r = new GS2.Random(star.Seed);
            var orbits = new List<Orbit>();
            ref var planets = ref star.Planets;
            planets.Sort(PlanetSortBySystemRadius);
            CalculateHabitableZone(star);
            var minimumOrbit = CalculateMinimumOrbit(star);
            var maximumOrbit = CalculateMaximumOrbit(star);
            var freeOrbitRanges = new List<(float inner, float outer)>();

            Warn("Orbit Count 0");
            if (star == birthStar)
            {
                var birthRadius =
                    Mathf.Clamp(r.NextFloat(star.genData.Get("minHZ").Float(0f), star.genData.Get("maxHZ").Float(100f)),
                        star.RadiusAU * 1.5f, 100f);
                Warn(
                    $"Selected Orbit {birthRadius} for planet {birthPlanet.Name}. Hz:{star.genData.Get("minHZ").Float(0f)}-{star.genData.Get("maxHZ").Float(100f)}");
                var orbit = new Orbit(birthRadius);
                orbit.planets.Add(birthPlanet);
                birthPlanet.OrbitRadius = birthRadius;
                orbits.Add(orbit);
                freeOrbitRanges.Clear();
                freeOrbitRanges.Add((minimumOrbit, birthRadius - birthPlanet.SystemRadius*2));
                freeOrbitRanges.Add((birthRadius + birthPlanet.SystemRadius*2, maximumOrbit));
            }
            else
            {
                freeOrbitRanges.Clear();
                freeOrbitRanges.Add((minimumOrbit, maximumOrbit));
            }

            for (var i = 0; i < planets.Count; i++)
            {
                Orbit orbit;
                var planet = planets[i];
                if (planet == birthPlanet)
                {
                    // planet.Name += " BIRTH";
                    continue;
                }

                // Log(planet.SystemRadius.ToString());
                planet.OrbitInclination = 0f;


                Log($"Orbit Count > 1. Free orbit range count = {freeOrbitRanges.Count}");
                var availableOrbits = new List<(float inner, float outer)>();
                foreach (var range in freeOrbitRanges)
                    if (range.outer - range.inner > 4 * planet.SystemRadius)
                        availableOrbits.Add(range);

                if (availableOrbits.Count == 0)
                {
                    Warn("Free Orbit Ranges:");
                    LogJson(freeOrbitRanges);
                    Warn($"No Orbit Ranges found for planet {planet.Name} radius:{planet.SystemRadius}");
                    var success = false;
                    foreach (var existingOrbit in orbits)
                        if (existingOrbit.hasRoom && existingOrbit.SystemRadius > planet.SystemRadius)
                        {
                            Warn($"Existing orbit {existingOrbit.radius} used for planet {planet.Name}");
                            existingOrbit.planets.Add(planet);
                            planet.OrbitRadius = existingOrbit.radius;
                            success = true;
                            break;
                        }

                    if (success) continue;

                    Warn("After all that, just couldn't find an orbit. Throwing planet into the sun.");
                    star.Planets.Remove(planet);
                    continue;
                }

                var selectedRange = r.Item(availableOrbits);
                var radius = r.NextFloat(selectedRange.inner + planet.SystemRadius*2,
                    selectedRange.outer - planet.SystemRadius*2);
                freeOrbitRanges.Remove(selectedRange);
                orbit = new Orbit(radius);
                orbit.planets.Add(planet);
                planet.OrbitRadius = radius;
                Log(
                    $"selected orbit({radius}) for {planet.Name}({planet.SystemRadius}) SelectedRange:{selectedRange.inner}, {selectedRange.outer} New Ranges: {selectedRange.inner},{radius - planet.SystemRadius}({radius - planet.SystemRadius - selectedRange.inner}) | {radius + planet.SystemRadius}, {selectedRange.outer}({selectedRange.outer - radius - planet.SystemRadius})");
                orbits.Add(orbit);
                var minGap = 0.1f;
                if (radius - planet.SystemRadius*2 - selectedRange.inner > minGap)
                    freeOrbitRanges.Add((selectedRange.inner, radius - planet.SystemRadius*2));
                if (selectedRange.outer - radius - planet.SystemRadius*2 > minGap)
                    freeOrbitRanges.Add((radius + planet.SystemRadius*2, selectedRange.outer));
            }

            starOrbits[star] = orbits;
            star.Planets.Sort(PlanetSortByOrbit);
            orbits.Sort(OrbitSort);
            for (var i = 0; i < orbits.Count; i++)
            {
                var planets2 = orbits[i].planets;
                if (planets2.Count > 1)

                    for (var j = 0; j < planets2.Count; j++)

                    {
                        var planet = planets2[j];
                        planet.Name += $" {i}{(EAlphabet) j}";
                    }
                else planets2[0].Name += $" {i}";
            }
        }

        private int OrbitSort(Orbit x, Orbit y)
        {
            if (x.radius == y.radius) return 0;
            if (x.radius > y.radius) return 1;
            return -1;
        }


        private int PlanetSortByOrbit(GSPlanet x, GSPlanet y)
        {
            if (x.OrbitRadius == y.OrbitRadius) return 0;
            if (x.OrbitRadius > y.OrbitRadius) return 1;
            return -1;
        }

        private int PlanetSortBySystemRadius(GSPlanet x, GSPlanet y)
        {
            if (x.SystemRadius == y.SystemRadius) return 0;
            if (x.SystemRadius < y.SystemRadius) return 1;
            return -1;
        }

        private void SetPlanetOrbitPhase()
        {
            Warn("Adjusting Orbits");
            var r = new GS2.Random(GSSettings.Seed);
            foreach (var star in GSSettings.Stars)
            {
                var orbits = starOrbits[star];
                foreach (var orbit in orbits)
                {
                    var planets = orbit.planets;
                    var pCount = planets.Count;
                    if (pCount == 0) continue;
                    var basePhase = r.NextFloat() * 360;
                    planets[0].OrbitPhase = basePhase;
                    for (var i = 1; i < pCount; i++) planets[i].OrbitPhase = planets[i - 1].OrbitPhase + 360f / pCount;
                }
            }
        }

        public class Orbit
        {
            public Orbit next;
            public GSPlanets planets = new GSPlanets();
            public Orbit previous;
            public float radius;

            public Orbit(float radius)
            {
                this.radius = radius;
            }

            public bool hasRoom
            {
                get
                {
                    if (planets.Count < 6) return true;
                    return false;
                }
            }

            public float SystemRadius
            {
                get
                {
                    float sr = 0;
                    foreach (var p in planets) sr = Mathf.Max(p.SystemRadius, sr);
                    return sr;
                }
            }

            public override string ToString()
            {
                return radius.ToString();
            }

            public float AvailableSpace()
            {
                if (next == null) return radius - previous.radius - previous.SystemRadius;
                if (previous == null) return next.radius - next.SystemRadius - radius;
                return Mathf.Min(next.radius - next.SystemRadius - radius,
                    radius - previous.radius - previous.SystemRadius);
            }
        }
    }

    internal enum EAlphabet
    {
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h,
        i,
        j,
        k,
        l,
        m,
        n,
        o,
        p,
        q,
        r,
        s,
        t,
        u,
        v,
        w,
        x,
        y,
        z
    }



    public static class NameGen
    {
        private static GS2.Random r;

        private static string[] cone = new[]
        {
            "rn", "st", "ll", "r", "rl", "gh", "l", "t", "d", "s"
        };

        private static string[] conb = new[]
        {
            "c", "pl", "s", "b", "d", "f", "g", "h", "j", "k", "l", "m", "n", "mn", "p", "pr", "ps", "p"
        };

        private static string[] conm = new[]
        {
            "sc", "c", "cc", "s", "ss", "b", "d", "f", "g", "h", "j", "k", "l", "m", "n", "mn", "p", "pr", "ps", "p"
        };

        private static string[] vowel = new[]
            {"o", "y", "u", "e", "ae", "i", "a"};

        public static string New(GSPlanet planet)
        {
            if (r is null) r = new GS2.Random(planet.Seed);
            int c = r.Next(1, 2);
            string output = (r.NextBool()) ? r.Item(conb) : "";
            for (var i = 0; i < c; i++)
            {
                output += r.Item(vowel) + r.Item(conm);
            }

            output += r.Item(vowel);
            output += (r.NextBool() ? "" : r.Item(cone));

            return output;
        }
// Achelous
// Oceanu
// Siren
// Acheron
// Achilles
// Actaeon
// Admetus
// Adoni
// Aeacus
// Aeëtes
// Aegisthus
// Aegyptus
// Aeneas
// Aeolus
// Asclepiu
// Agamemnon
// Aglaia
// Ajax
// Alcestis
// Alcyone
// Pleiades
// Furies
// Althaea
// Amazons
// Ero
// Amphion
// Amphitrite
// Amphitryon
// Anchises
// Andromache
// Andromeda
// Anteros
// Antigone
// Antinoüs
// Venus
// Apollo
// Aquilo
// Arachne
// Mars
// Argo
// Argus
// Ariadne
// Arion
// Artemis
// Asclepius
// Astarte
// Sterop
// Astraea
// Atalanta
// Minerva
// Atlas
// Atreus
// Atropos
// Fates
// Eo
// Auster
// Avernus
// Dionysu
// Bellerophon
// Bellona
// Boreas
// Cadmus
// Calliope
// Muses
// Calypso
// Cassandra
// Castor
// Centaurs
// Cephalus
// Cepheus
// Cerberus
// Demete
// Chaos
// Charon
// Charybdis
// Chiron
// Chryseis
// Circe
// Clio
// Clotho
// Clytemnestra
// Saturn
// Cyclopes
// Daedalus
// Danae
// Danaïdes
// Danaüs
// Daphne
// Graeae
// Ceres
// Artemi
// Dido
// Diomedes
// Dione
// Bacchus
// Dioscuri
// Plut
// Hade
// Dryads
// Echo
// Electra
// Endymion
// Enyo
// Eos
// Aurora
// Erato
// Erebus
// Eris
// Eros
// Eteocles
// Euphrosyne
// Europa
// Eurus
// Gorgons
// Eurydice
// Eurystheus
// Euterpe
// Fauns
// Faunus
// Pa
// Favonius
// Flora
// Fortuna
// Gaea
// Galatea
// Ganymede
// Golden Fleece
// Graces
// Hades
// Hamadryads
// Harpies
// Juventas
// Hecate
// Hector
// Hecuba
// Helen
// Helios
// Sol
// Vulcan
// Hera
// Juno
// Hercules
// Mercury
// Hero
// Hesperus
// Hestia
// Vesta
// Hippolyte
// Amazon
// Hippolytus
// Hippomenes
// Hyacinthus
// Hydra
// Hyperion
// Hypnos
// Somnus
// Iapetus
// Icarus
// Io
// Iobates
// Iphigenia
// Iris
// Ismene
// Ixion
// Janus
// Jocasta
// Zeu
// Heb
// Laius
// Laocoön
// Lares
// Let
// Lavinia
// Leda
// Lethe
// Leto
// Latona
// Maia
// Manes
// Marsyas
// Medea
// Medusa
// Meleager
// Melpomene
// Memnon
// Menelaus
// Mentor
// Herme
// Merope
// Midas
// Athen
// Minos
// Minotaur
// Mnemosyne
// Muse
// Momus
// Morpheus
// Thanato
// Naiads
// Napaeae
// Narcissus
// Nemesis
// Neoptolemus
// Poseido
// Nestor
// Nike
// Niobe
// Notus
// Ny
// Nymphs
// Nyx
// Oceanids
// Oceanus
// Odysseus
// Ulysses
// Oedipus
// Oenone
// Rhe
// Orestes
// Furie
// Orion
// Orpheus
// Pan
// Pandora
// Parcae
// Paris
// Patroclus
// Pegasus
// Pelias
// Pelops
// Penates
// Penelope
// Persephone 
// Proserpine
// Perseus
// Phaedra
// Phaethon
// Philoctetes
// Phineus
// Phlegethon
// Pirithous
// Pluto
// Pollux
// Polyhymnia
// Polymnia
// Polynices
// Polyphemus
// Polyxena
// Pontus
// Poseidon
// Neptune
// Priam
// Priapus
// Procrustes
// Prometheus
// Persephon
// Proteus
// Psyche
// Pygmalion
// Pyramus
// Python
// Quirinus
// Rhadamanthus
// Rhea
// Ops
// Romulus
// Sarpedon
// Cronu
// Satyrs
// Scylla
// Selene
// Semele
// Sileni
// Silvanus
// Sirens
// Sisyphus
// Helio
// Hypno
// Sphinx
// Styx
// Symplegades
// Syrinx
// Tantalus
// Tartarus
// Telemachus
// Tellus
// Terminus
// Terpsichore
// Thalia
// Thanatos
// Mors
// Themis
// Theseus
// Thisbe
// Thyestes
// Tiresias
// Titans
// Tithonus
// Triton
// Turnus
// Odysseu
// Urania
// Uranus
// Aphrodit
// Hesti
// Hephaestu
// Zephyrus
// Jupiter
// Dwarf Planets
// Aeternae
// Alcyoneus
// giant
// Almops
// Poseidon
// Helle
// Aloadae
// Ares
// Amphisbaena
// Arae
// Argus
// Asterius
// Athos
// Briareus
// Catoblepas
// Centaur
// Centauride
// Agrius
// Heracles
// Amycus
// Centauromachy
// Asbolus
// Bienor
// Centaurus
// Chiron
// heroes
// Chthonius
// Nestor
// Pirithous
// Hippodamia
// Cyllarus
// Dictys
// Elatus
// Eurynomos
// Eurytion
// Eurytus
// Lapiths
// Hylaeus
// Atalanta
// Hylonome
// Nessus
// Perimedes
// Phólos
// Pholus
// Rhaecus
// Thaumas
// Cyprus
// Lamos
// Zeus
// Dionysus
// Hera
// Amphithemis
// Cerastes
// Cerberus
// Hades
// Cetus
// Ceuthonymus
// Menoetius
// Charon
// Styx
// Charybdis
// Chimera
// Crocotta
// Cyclopes
// Arges
// Gaia
// Uranus
// Tartarus
// Polyphemus
// Hephaestus
// Daemons
//  Ceramici
// Damysus
// Thrace
// Dryad
// Echion
// Eidolon
// Empusa
// Enceladus
// Athena
// Erinyes
// Cronus
// Virgil
// Alecto
// Megaera
// Tisiphone
// Gegenees
// Gello
// Geryon
// Gigantes
// Gorgons
// Euryale
// Medusa
// Stheno
// Graeae
// Griffin
// Harpies
// Aello
// Celaeno
// Ocypete
// Hecatonchires
// Hippalectryon
// Hippocampus
// Lernaean Hydra
// Lerna
// Labour
// Ichthyocentaurs
// Ipotane
// Keres
// Achlys
// Kobaloi
// Laestrygonians
// Antiphates
// Manticore
// Mimas
// Minotaur
// Theseus
// Hellhound
// Orthrus
// Odontotyrannos
// Onocentaur
// Ophiotaurus
// Orion
// Ouroboros
// Pallas
// Panes
// Periboea
// Giantess
// Phoenix
// Polybotes
// Porphyrion
// Satyrs
// Satyresses
// Pan
// Agreus
// Ampelos
// Marsyas
// Nomios
// Silenus
// Scylla
// nereid
// Circe
// Sirens
// Medea
// Pasiphaë
// Argo
// Sphinx
// Hieracosphinx
// Taraxippi
// Thoon
// Tityos
// Triton
// Amphitrite
// Typhon
// Monocerata
// Lamiai
// Empousa
// Lamia
// Mormo
// Mormolykeia
// Hecate
// Agriopas
// Damarchus
// Parrhasia
// Lykaia
// Lycaon
// Nyctimus
// Pegasus
// Acanthis
// Carduelis
// Alectryon
// Aphrodite
// Helios
// Autonous
// Gerana
// Oenoe
// Aethon
// Lark
// Alcyone
// Alkyonides
// halcyons
// Ceyx
// Aëdon
// Procne
// Nyctimene
// Ascalaphus
// Philomela
// Cornix
// Coronis
// Corvus
// Apollo
// Cycnus
// Phaethon
// Eridanos
// Strix
// Tereus
// Hoopoe
// Ionia
// Crommyon
// Gadflies
// Myrmekes
// Menoetes
// Cercopes
// Actaeon
// panthers
// Argos
// Odysseus
// Laelaps
// Maera
// Erigone
// Arion
// Taras
// Styx
// Achilles
// Balius
// Xanthus
// Ladon
// Nemea
// Mysia
// Trojan
// Laocoön
// Aeëtes
// Talos
// Aella

    }
}