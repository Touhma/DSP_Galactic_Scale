# DSP Galactic Scale 2.0 Mod

# BACKUP YOUR SAVES. SERIOUSLY.

- Version 2.77.1 - Update for game patch 0.10.34.28347-r.0
- Version 2.77.0 - Update for game patch 0.10.34.28326-r.0
- Version 2.76.3 - Fix vegetation generation crash by ensuring vegeCursor is properly reset.
- Version 2.76.2 - Fix critical memory leak when mods query vein counts (was causing 128GB+ RAM usage). 
- Version 2.76.1 - Update GSUI for Unity 2022.3
- Version 2.76.0 - Attempt to fix for latest multithread patch 0.10.33.26934
- Version 2.75.11 - Arflipe's fix for hives not attacking
- Version 2.75.10 - Fix Crash when saving
- Version 2.75.9 - Add finer galaxy control to Dev Generator. Add Logging for some save bugs, and Communicator bugs. Make sure you turn logging on! (And in BepInEx config)
- Version 2.75.8 - More Null Checks
- Version 2.75.7 - Init RawData if missing
- Version 2.75.6 - Add Nullcheck and wonder how I got the version numbering so hopelessly out of order
- Version 2.75.5 - Starfish saves the day (again). Many thanks!
- Version 2.74.4 - Attempt to fix memory leak
- Version 2.73.3 - Force Modelling of Planets instead of scanning. Prevent Automatic Scanning every 10s
- Version 2.73.2 - Init ModData on save to try prevent error
- Version 2.73.1 - Fix game loading error when you save in outer space
- Version 2.73.0 - Update for DSP Version 0.10.32.25779 
- Version 2.72.5 - Add O-Type Giant
- Version 2.72.4 - Revert broken relay ship fix
- Version 2.72.3 - Fix ObserveLevel bug. Add debug option to set observe level. Attempt to fix df relays and logistics ships not pathing past large stars
- Version 2.72.2 - Fix DF Communicator being broken. Fix Crash with large starcount
- Version 2.72.1 - attempt to fix decimal numbers
- Version 2.72.0 - starfish fixed loading :D.
- Version 2.71.1 - Attempt to fix game not starting sometimes
- Version 2.71.0 - Fix for new Game Update
- Version 2.17.1 - Attempt to prevent star view distance from being reset 
- Version 2.17.0 - Update Game Version
- Version 2.16.6 - Fix Dyson Sphere Radius (New Saves. Edit the GS2 File otherwise)
- Version 2.16.5 - Fix planet priority route function
- Version 2.16.4 - Fix crash on starting a new game with prologue
- Version 2.16.3 - Fix bug selecting starting planet star
- Version 2.16.2 - Fixed error on new game
- Version 2.16.1 - Potential Fix for Bug on New Game
- Version 2.16.0 - Tentative update game version 0.10.31.24646
- Version 2.15.4 - Default star size to 1x to try and prevent some pathing errors on df ships
- Version 2.15.3 - Fix Miners on small/large planets
- Version 2.15.2 - Enable option to ignore gameload timeout for Nebula users
- Version 2.15.1 - Merge Dev Generator with GS2 Generator to fix broken moon of gas giant starts.
- Version 2.15.0 - Update DSP Game Version 0.10.30.23292
- Version 2.14.3 - Fixed bug where Gas Giant Moon Chance was ignored
- Version 2.14.2 - Starfish fixed: Fix trash doesn't pick up by BAB soon on outer planets.  Fix throwable dealt no damage on different size planets.  Fix throwable curvature on outer planets. Fix Nebula client error when joining in space
- Version 2.14.1 - Fix Gas Giant Moon Start
- Version 2.14.0 - Update DSP Version
- Version 2.13.4 - Even more hopefully fix bugs with rockets and spheres. Hopefully fix but with rare veins spawning on gas giants
- Version 2.13.3 - Hopefully fix bugs with rockets and spheres
- Version 2.13.2 - Nebula Compatibility
- Version 2.13.1 - Hopefully fix DF Drops not relating to planet type. Fixed the planet theme descriptions for Vanilla Themes. GS Themes still use their base theme... for now.
- Version 2.13.0 - Recompile for DSP Update. Removed code for starmap, might break it. Let me know if it does.
- Version 2.12.47 - Move Darkfog Hives that are spawned outside of system limits
- Version 2.12.46 - Modify clearance value that logistics vessels use to avoid stars. They might fly through them now, but hopefully not. At least they should arrive instead of flying in circles
- Version 2.12.45 - Add debug option to suspend processing of Darkfog at a certain lightyear distance from the player (For those with major performance issues)
- Version 2.12.44 - Hopefully Fix loading on xbox GamePass
- Version 2.12.43 - Enable Darkfog Settings in JSON Galaxy Import (In Settings). Enjoy! Fixed Birth System Dark Fog Spawn
- Version 2.12.42 - Enable loading Splash Images from config/Splash folder. Preserve aspect ratio set for all splash images to avoid stretching on widescreens.
- Version 2.12.41 - Fix BP error. For real this time. Well, at least one of them. Drastically increased the size of some SectorLogic arrays, hopefully it stops the game breaking when you have a lot of stars/hives
- Version 2.12.40 - Fix BP error. Add Birth Planet Orbit setting to GS2 Generator. Better defaults for orbit and hz override which are enabled by default. New splashscreen. 
- Version 2.12.39 - Remove BCE dependency
- Version 2.12.38 - Fix projectiles not working on small planets. Thanks starfish!
- Version 2.12.37 - Fixed log spam breaking new game
- Version 2.12.36 - Added Birth Planet Selector. Might not work if your birthplanet is a moon... Let me know if it doesn't work for you.
- Version 2.12.35 - Fix for error when hovering over stars and seeds simultaneously
- Version 2.12.34 - Fix for new DSP Update 0.10.28.21308 Starmap should be feature complete vs vanilla now.
- Version 2.12.33 - Fix not being able to attach belts to ILS on comets
- Version 2.12.32 - Fix error when due to large starcounts the game creates more idle relays than it has space for
- Version 2.12.31 - Fix missing text on settings buttons. Fix issue on comets placing belts under turrets. Tidy up settings. Add missing hint text. Add dev options.
- Version 2.12.30 - Fixed blueprinting Planetary Logistics Stations. Finally remembered to remove useless debug options from settings.
- Version 2.12.29 - Fix Galaxy Generation where BirthPlanet is a moon. Fix error where factories array is not set higher than planetcount (Some other Mod hijacking the prefix patch?)
- Version 2.12.28 - Fix Planetary Shield on small/large planets. Thanks karki!
- Version 2.12.27 - Fixed copy/paste not working properly. Inserters should now work properly on all planet sizes. Let me know on discord if there are any issues.
- Version 2.12.26 - Fix Hives so they DONT show in starmap. Fixed them so they show in normal play. Fixed Dark Fog Communicator not being labelled.
- Version 2.12.25 - Fix Hives not showing in starmap. Fixed not being able to reach logistics stations. Increased reach distance in general by a few meters.
- Version 2.12.24 - Fix some bugs introduced last patch.
- Version 2.12.23 - Remove force garbage collect when building. Massive performance improvement. Thanks to starfish for the code. Fixed Enemies on other planets. Fixed some errors. Fixed some logspam.
- Version 2.12.22 - Recompile for DSP .21219
- Version 2.12.21 - Fix for water on Pandora
- Version 2.12.20 - Prevent fix for clipping enemies turning off (Hopefully)
- Version 2.12.19 - Fix for gravity on moons orbiting huge gas giants. Fix can be reverted via toggle in GS Settings/Debug. Improved the code for the fix in 2.12.18 to increase performance.
- Version 2.12.18 - Fix for enemy units clipping through large planets. Not tested on smaller planets. Please let me know if it breaks anything.
- Version 2.12.17 - Revert csd fix which made things worse
- Version 2.12.16 - Fixed issue with resetting generator. Fixed settings not being saved. Fixed Generator Settings defaulting to GS2. Prevented a bunch of logspam.
- Version 2.12.15 - Fixed issue with comma seperated decimals. Fixed issue with GS2 Dev Generation. Ignored error when autosaving over locked gs2 files.
- Version 2.12.14 - Fixed issue loading saves
- Version 2.12.13 - Fixed Pandora Planet
- Version 2.12.12 - Update to newest DSP Version. Some new features may be bugged, only tested that the game works.
- Version 2.12.11 - Possibly fix broken saves. Might break unbroken saves. Needs testing!
- Version 2.12.10 - Fixed bug The_Iron_Angel caught when localPlanet was not present yet you killed something
- Version 2.12.9 - Fixed a lot more darkfog stuff - Will move hives around.
- Version 2.12.8 - Fixed a bunch of darkfog stuff
- Version 2.12.7 - Fixed Geothermal Power on DF Core Mines
- Version 2.12.6 - Combat working on small planets (So far as I have tested at least)
- Version 2.12.5 - Compatibility. Removed Replicator Alt Buttons.
- Version 2.12.4 - Much better combat on slightly smaller planets. 
- Version 2.12.3 - Attempt at getting more stars to work
- Version 2.12.2 - Attempt at getting some combat systems working. Seems to be working on normal to large planets, but I need it tested. Small planets seem to be broken still.
- Version 2.12.1 - Fixed a lot of bugs in New Game screen. Dark Fog Settings work now. Only tested with 200 radius planets. Seems to be working at least for the first few mins.
- Version 2.12.0 - Initial Dark Fog Test Build. DONT CRY IF THIS EXPLODES YOUR PC. Very Buggy. Only for testing, would not recommend trying to actually play the game on this build. Have fun in vanilla unless you're helping fix the bugs :)
- Version 2.11.11 - Add Binary Star Fix from Arniox, and Soarqin's autobridging belt fix. Thanks to starfish for the transpiler, and for paying attention when I was asleep at the wheel.
- Version 2.11.10 - Translation additions. Thanks starfish
- Version 2.11.9 - Added GalacticScale Dev Generator, added Recursive Moon option. Removed Vanilla Gen due to lack of maintenance.
- Version 2.11.8 - Attempt to fix localization issues
- Version 2.11.7 - Fix Galaxy Generation when moon change is high
- Version 2.11.6 - 2nd Attempt
- Version 2.11.5 - Attempt to fix Null Reference error with Infernal Giant
- Version 2.11.4 - Fix Null Reference Error when quitting
- Version 2.11.3 - Fix errors in roman number display when more than 20 planets are used. Fix vein display problems. Fix Fast Travel button. Thanks starfish!
- Version 2.11.2 - Bump version number
- Version 2.11.1 - Fix of the fix
- Version 2.11.0 - Fix disappearing planets in far flung orbits. Fix some clouds. Add Gas Giant Size Settings which may or may not break the game. Revert to previous version if so and let me know.
- Version 2.10.3 - Fix error with birthplanet theme
- Version 2.10.2 - Possibly savebreaking, Vein algorithm fixes, multithreading fix. Thanks karki :)
- Version 2.10.1 - karki multithreaded some star stuff and got speed improvements! :D
- Version 2.10 - karki fixed Mk2 Miner Graphical Error, Sped up Starmap :D Thanks!
- Version 2.9 - karki fixed birth planet generation and an error on exit. Thanks!
- Version 2.8.8 - Added ability to begin on a moon to GS Generator Settings
- Version 2.8.7 - Incorporate 2.8.5 which was missed from .6
- Version 2.8.6 - Update for DSP 0.9.27.14659
- Version 2.8.5 - starfish's xgp fix
- Version 2.8.4 - Attempt at fixing mod compatibility.  Reverts 2.8.3.
- Version 2.8.2 - Remove non existing vegetation "Jungle Tree 3"
- Version 2.8.1 - Fix Logistic Bot Speed. For games created on 2.8.0 a button is added under Settings/GalacticScale/Debug that should fix the issue.
- Version 2.8.0 - Update for new DSP Version. Implemented starfish's warning system fix.
- Version 2.7.12 - Additional monitoring component null reference avoidance
- Version 2.7.11 - Fix very rare bug with monitoring component trying to use a speaker that doesn't exist in the pool. *shrug*
- Version 2.7.10 - Hopefully fix Nebula compatibility
- Version 2.7.9 - Fix no rares in starting system, and pregame system viewer improvements.
- Version 2.7.8 - Fix strange mod conflict caused by debug logging
- Version 2.7.7 - Another attempt to fix issues loading saves
- Version 2.7.6 - Fix errors when starting/ending game while still calculating veins
- Version 2.7.5 - Fix error when loading game while planets are calculating
- Version 2.7.4 - Combine Gas/Liquid Items in Star Detail again. Fix System view UI. Fix Sandbox toggle in newgame and
  load game.
- Version 2.7.3 - Hopefully fix errors introduced by 2.7.2
- Version 2.7.2 - Fixed issue with birthplanet veins not being calculated. Thanks to starfish. Fixed vein display in new
  game system viewer. Merged LittleSaya's PR with Transpiler for PlanetFactory.FlattenTerrain. Known issue: With lots of
  planets the star details will be overflowing with oceans. Will fix soon.
- Version 2.7.1 - Two bugfixes regarding outer planets and gas giants giving errors when viewing details
- Version 2.7.0 - Updated for DSP 13026. May or may not work. Need feedback in discord. They changed a bunch of stuff
  relating to veins. BACKUP YOUR SAVES

# BACKUP YOUR SAVES. PLEASE.

- Version 2.6.0 - Fixed terrain generation code that has been broken since version 2.0.0. I wouldn't upgrade if you have
  an existing save. Your planets may morph into their true form. This probably isn't a good thing if you have buildings
  on them.

- Version 2.5.15 - Add new Planet Types (IceGelisol 2/3, FrozenTundra, PandoraSwamp 1/2, Crystal Desert, Savanna)
- Version 2.5.14 - Update for DSP 12900 (New Planets Not Yet Added, I'll be working on them today)
- Version 2.5.13 - Starfi5h's Nebula Compatibility fix
- Version 2.5.12 - Better Nebula Compatibility RE: Spray Coater Graphics
- Version 2.5.11 - Add toggle for starting system rare
- Version 2.5.10 - Add slider for planet rotation in Sol Generator for @Cidno
- Version 2.5.9 - Possible bug fix for reported veins in starmap
- Version 2.5.8 - Fix Scarlet Ice Lake Ice. Can now walk/build on.
- Version 2.5.7 - Fixed duplicate space capsule
- Version 2.5.6 - Improve Teleportation Functionality
- Version 2.5.5 - Fix Mecha Scaling when teleporting and add debug scale value for funsies.
- Version 2.5.4 - Fix Vanilla Generator Saves
- Version 2.5.3 - Added some missing code from recent updates that dpmm99 discovered

- Version 2.5.2 - Fixed Ice Gellisol rocks

- Version 2.5.1 - Fixed Autosave and LastExit

- Version 2.5.0 - Changed the way GS Handles Saves. No longer injects GS data into the DSP save file.

- Version 2.4.26 - BACKUP SAVES!!! Prevents some errors when loading saves.

- Version 2.4.25 - BACKUP SAVES!!! Assumed Working in latest DSP Update. Save loading fixed.

- Version 2.4.24 - Recompile for latest DSP Update 0.9.25 , untested. May still not work or throw errors.

- Version 2.4.23 - Fix Camera Shift when loading game after moving camera in system display

- Version 2.4.22 - Add Inner Planet Distance as Star Specific Override to GSGen

- Version 2.4.21 - Fix bug with Vanilla Gen

- Version 2.4.20 - Fix bug with comet generation

- Version 2.4.19 - Fix Planet Placement and Solar Values in GSGen. Fix SystemDisplay. Add "Start at PlanetName". Fix
  Nebula Compatibility (thanks starfish!)

- Version 2.4.18 - Fix orbit spacing :)

- Version 2.4.17 - Add options to change star/planet naming in GalacticScale Generator

- Version 2.4.16 - Add variable to increase orbit spacing to GalacticScale Generator

- Version 2.4.15 - Add solar power settings to GalacticScale Generator

- Version 2.4.14 - Improve Nebula Compatibility

- Version 2.4.13 - Fix another blueprint bug

- Version 2.4.12 - Hopefully fix blueprint error on 510 size planets.

- Version 2.4.11 - Hopefully fix logistic vessel pathing around huge stars. Thanks 46bit

- Version 2.4.10 - Hopefully more compatibility with Nebula. Fixed Spraycoater attaching to existing belt. Fixed
  spraycoater graphical glitch. Fixed a few small bugs. Thanks starfish!

- Version 2.4.9 - Fixed blueprints using extra space

- Version 2.4.8 - Fixed Lava & Hurricane Themes missing rocks. Added hint text to galaxy select. Clamped solar power to
  1-500%. Fixed GS2 Star Overrides not loading correctly. Fixed bug where planetdetail wouldnt close on game start.

- Version 2.4.7 - Fixed Error when saving games with short names. Added configurable mouse tolerance for systemdisplay.
  Fixed Planet name text flickering in center of screen when clicked.

- Version 2.4.6 - Fixed Warning System for Raptor (Hopefully). Fixed Quickstart bug from 2.4.3

- Version 2.4.5 - Fixed Autosave Galaxy Setting Spam. Will require generator recompile.

- Version 2.4.4 - Fixed error in console code. Will require generator recompile.

- Version 2.4.3 - Fixed space capsule doubleup on non skip gamestart

- Version 2.4.2 - Fixed Vanilla and Sol Generators

- Version 2.4.1 - Fix the icon :)

- Version 2.4 - Merge Experimental and Main Branches - Fix a bunch of bugs

- Version 2.3.16 - Comets added to GS2 Generator

- Version 2.3.15 - Test GS2 More realistic solar power. Settings update. Will require generator recompile.

- Version 2.3.14 - Fixed blueprints at poles. Now deletes logs older than a week

- Version 2.3.13 - Fixed issues in galaxy select. Fixed loading game when in space. Fixed loading game putting player
  inside planet. Fixed Obsidian theme for new games.

- Version 2.3.12 - Removed a lot of logspam. Fixed error on quit while planets loading

- Version 2.3.11 - Fixed version

- Version 2.3.10 - Fixed the galaxy select star/planet labels. Hit alt to get them all to show

- Version 2.3.9 - @starfi5h 's super fast start implemented. No more waiting for every planet to load! Fixed bug when
  quitting before all planets are generated.

- Version 2.3.8 - Fix gamestart - Nebula tested and working at least as far as new game is concerned

- Version 2.3.7 - SystemDisplay updates, Nebula Fixes. Maybe nebula works, maybe it doesn't :)

- Version 2.3.6 - Maybe nebula works, maybe it doesn't :)

- Version 2.3.5 - Fixed cargo ships not docking on outer planets in large systems

- Version 2.3.4 - Further refinement of system view

- Version 2.3.3 - Implemented System View in Galaxy Select Screen. Click a star to view the system. Currently testing
  scale of planets etc, help by trying new values under Debug Options in settings. Post screenshots to our discord if
  you find a combination that works well. I'm thinking we might need to apply some sort of logarithmic compression to
  boost tiny planets and shrink huge gas giants.
- Added CloseError as a dependency. You're probably going to get errors, why look at the red box of annoyance if you
  don't have to?

- Version 2.3.2 - Finally fixed North/South Pole markers on non standard planet sizes :)

- Version 2.3.1 - Changed Luminosity Boost. Fixed Slider rounding error. Fixed Moon size for Starter Planet.

- Version 2.3 - Experimental UI Changes to sliders. Includes button to switch between slider and text entry. Floating
  point bug not fixed. :(

[![Galactic Scale Trailer](https://github.com/Touhma/DSP_Plugins/blob/main/thumb.png?raw=true)](https://www.youtube.com/watch?v=JpdW3S73hYw "Galactic Scale Trailer - Click to Watch!")

Play with friends!
https://dsp.thunderstore.io/package/nebula/NebulaMultiplayerMod/

Remember to swing by our discord for planet themes, custom galaxies, blueprints, and help!
https://discord.gg/NbpBn6gM6d

-----------

# What Galactic Scale 2.0 is: A Framework for creating generator plugins that can completely customize everything to do with galaxy/planet generation in Dyson Sphere Program, with two different generators built in.

What can be customized?

- Planet Sizes from 5-510 tile radius (Vanilla is 200)
- Gas Giant Sizes from 500-5100
- Star Counts from 1-1024+ stars
- Planet Themes (New planet types aplenty)
- Vein Distribution
- Terrain Generation
- Vegetation Distribution
- Star location, size, luminosity etc. All exposed to configuration.
- Binary Stars.
- System generation. Everything is configurable. Planets can share orbits.
- All planets can have moons, including planets that are already moons. Moonception.
- Planet material customization. Tinting, texture replacement etc.
- Planet shader configuration. Change ocean and atmosphere params.
- Binary Planets.

And so much more that I've forgotten. Documentation might be coming soon! (If enough people bug me about writing it)

/!\ READ THIS

# Discord for Galactic Scale's team work

https://discord.gg/NbpBn6gM6d To talk about the mod and ask me questions directly :)

# Patch notes & releases :

You can find all the releases and patch notes on the discord server or https://github.com/Touhma/DSP_Plugins/releases

# Bug report :

Go on the discord or post a ticket here :
https://github.com/Touhma/DSP_Plugins/issues

## Disclaimer

This mod is pretty much incompatible with anything touching on any part of the generation, though we try and work with
other mod creators to address compatibility issues.

We need feedback, bug reports, and ideas. Plus we are always looking for dev help!.

Have fun!

## How to Install :

Use the Thunderstore Mod Manager. It's much better than manually installing.

First Install Bepinex in your game
folder : https://bepinex.github.io/bepinex_docs/master/articles/user_guide/installation/index.html?tabs=tabid-win

Then Download the latest release of the mod : https://github.com/Touhma/DSP_Plugins/releases

Add it in bepinex plugins folder in your game : %gamefolder%\Dyson Sphere Program\BepInEx\plugins

Launch the game and you should be all set !

## How to use galactic-scale ?

All settings are in the in game settings menu There are 2 generators in the beta release:

- Sol: An accurate rendition of our solar system and neighboring stars.
- GalacticScale: Customize everything!

Additional generators are available in discord, and can be created by the community! Documentation on this feature will
be written soon.

## For modders :

We spent a lot of time working on that mod so please ... don't be a thiefy mcThieferson.

If you have some features or ideas that would need our code to work, talk to the main dev first ( innominata#0454 ) on
Discord about it. GS2 is very expandable, and you can probably do it easily with a generator or other plugin.

If you wanna fix something, please go ahead and do a pull request, we'll happily credit you for your work.

Do NOT make a mod just to change the default configuration or a few parameters, create your own generator instead :) if
it's good it can make it to the official release of the mod.

We would love that mod to be a framework for community created plugins concerning everything around the generation of
the moons, planets, stars, etc

This mod is and will always be free of charge for everyone.

Old Docs:  https://centrebra.in/

Credits:

- innominata - GS1/GS2/GS3 Code/Art
-
- Touhma - GS1 Mod Developer / Advisor
-
- Shad0wlife - GS1/GS2 Code / Advisor
-
- GlitchHound - GS1 Code / Advisor
-
- sp00ktober - GS2/GS3 Code /Advisor
-
- tyukara - GS2 Japanese Translation
-
- Mushroom - GS1 Dev Env
-
- PhantomGamers - GS2 Dev Env / Advisor
-
- starfish - GS3 Code / Advisor
-
- Kremnev8 - GS2 Code / Advisor
-
- NHunter - GS2 Code / Advisor
-
- 46Bit - GS2 Code