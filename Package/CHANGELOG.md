## 2.81.2

### Changes by Kimi-Arthur:

**Improvements**

- Prevented distant moon labels from overlapping their host planet labels in space and on the starmap.
- Shortened long NiceSol star names for cleaner HUD and starmap labels.
- Unified localization through `.Translate()` and completed the Simplified Chinese (`zh-CN`) translations.

### Changes by Devious:

**Features**

- Added orbit eccentricity to the planet summary alongside the other orbital details.

**Bugfixes**

- Fixed missing atmospheres on non-vanilla sized planets.
- Fixed cloud height, thickness, and pattern scaling on non-vanilla sized planets.

## 2.81.1

### Changes by Kimi-Arthur:

**Features**

- Added procedural planetary ring system for gas giants.
- Added dynamic night-side polar planetary auroras for atmospheric planets and gas giants.
- Added elliptical orbits to comets and comet tails.



### Changes by Devious:

**Features**

- I've finally added elliptical orbits which were planned like 5 years ago, but are now finally here, yay. Elliptical orbits will only generate in new galaxies to avoid breaking saves prior to this update. Maximum Orbit Eccentricity under System Settings controls the ceilling for elliptical orbits.

**Bugfixes**

- Fixed bent spray coaters.
- Fixed small terrain vertex issue.

## 2.80.11

### Changes by Kimi-Arthur

**NiceSol bugfix and alterations**

- Changed generation to prioritize prominent constellations for smaller star counts, and changed the default star count to 500 instead of 1000.
- Fixed NiceSol small-moon radius not being rounded to a valid planet size to avoid theme lookup gap.

**Universal bugfix**

- Fixed veins from overlapping the starting landing capsule.

## 2.80.10

### Changes by Komonad

**Bugfix**

- Fixed one-shot ground laser entries accumulating when their planet factory is unavailable. Thanks Komonad for catching my mistake here.

### Changes by Devious

**Bugfixes and alterations**

- Tweaked combat skill updates during planet loading to prevent some potential null errors and redid a few changes I made previously that might've affected combat on planets you weren't on.
- Fixed Dark Fog hive orbits clustering into nearly identical planes. Previous Unsafe re-mappings and legacy near-aligned orbits are corrected when loading saves. This will alter the orbits of most Dark Fog Hives to be more varied, so just be aware of that when you load in.
- The starting planet theme selector is now disabled unless Starting Planet Unlock is enabled. This change and the batched construction one below were done previously, I just forgot to put them in previous changelogs. I really have got to organize better.

**Performance**

- Batched construction terrain flattening to reduce building and blueprint cpu usage spikes on large planets.

## 2.80.9

### Changes by Kimi-Arthur

**Features**

- Added two opt-in vein patch count controls under Universal Settings in Galactic Scale's main settings panel that work uniformly across all generators (GS2Generator3, GS2Generator2, Sol, and external generators):

  1. Scale veins by planet size (ScaleVeinsBySize, off by default): scales solid-planet vein patch counts proportionally to planet surface area (Radius / 200)^2.
  2. Vein count multiplier (VeinCountMultiplier, 0.25–4.0, step 0.25, 1.0 by default): multiplies vein patch counts across solid planets independently or on top of surface-area scaling, capped at 4.0x so continents remain buildable and below land-packing saturation.

- Added NiceSol, a new optional galaxy generator alongside Sol featuring a 1,000-star 3D catalog spanning all 88 IAU constellations and a dynamic, ephemeris-driven Solar System.

**Bugfix**

- Fixed a NullReferenceException on the planet compute thread when starting a new game after changing galaxy preview settings or exiting a previous session.

### Changes by Devious

**Bugfix**

- Fixed planet detail UI spacing that wasn't right anymore after the latest DSP updates.
- Fixed a minor invalid scanned records warning.
- Added a surface integrity check on the active planet to ensure it is rendered properly even after teleports/fast-travels that could cause an unrendered planet.
- Changed the seed randomizer again to ensure it continues randomizing after returning to the main menu.
- Slightly changed how GS2 saving works to mitigate save overwrite failures.

## 2.80.8

- Bugfix: Fixes additional null errors while loading planets and fixes a mistake I made that caused problems with SampleAndHoldSim.

## 2.80.7

**Bugfix**

- Fixes a null error when loading certain saves made prior to 2.80.6

## 2.80.6

**Features**

- Performance Settings category under GS2 settings that lets you tweak how frequently distant stars and planets update. Planets with factories and stars with Dyson Spheres will use their normal update logic. This shouldn't cause any compatibility issues with SampleAndHoldSim or Weaver.

**Bugfixes**

- Invisible solid planets shouldn't require a save and reload to fix anymore.
- Belts should now be properly aligned on large planets. Please let me know if anything else is misaligned.
- Minor fix to prevent inconsequential errors when previewing planets in GS2's galaxy preview while Orbital Ring is installed. Side note, Orbital Ring should be compatible with GS2 now.
- Minor fix to prevent a small memory leak during galaxy generation.
- Seed randomizer now properly randomizes the seed.

## 2.80.5

**Bugfixes**

- Fixed Icarus disappearing while flying away from large gas giants and not being able to warp until reaching 0.5 to 1AU away from the gas giant.
- Fixed items dropping from destroyed Dark Fog being assigned to the planet's moon instead of the planet where the Dark Fog unit was destroyed.
- Fixed size 510 planets in old saves being shrunken.
- Fixed additional issues with ILS vessels between Galactic Scale and MoreMegaStructures. Hopefully no more problems crop up.

## 2.80.4

- Bugfix: ILS vessels not returning at warp speed when station warp distance is set to 0.5AU.

## 2.80.3

**Features**

- Added a toggle in Galactic Scale quality of life options to hide the brush size buttons for those who don't want them. You can also go up to 20x20 with the regular in-game hotkeys as well.
- Expanded Binary Star settings to choose what stars are allowed as either companion or host stars.
- Added a new Dark Fog settings tab so that players who like to keep the same difficulty settings can set them once and not have to repeat them every new game.
- You can now choose whether or not a relay is allowed to spawn on your birth planet and whether or not a hive exists in your birth system, under any difficulty settings.
- Stars will now all have unique names; no more duplicate names should generate in new galaxies.

**Bugfixes**

- ILS vessels and Combat ships shouldn't get stuck around large stars anymore, but please report it in the GS2 Discord if you see your vessels or ships get stuck.
- Fixed gas giants being unlandable on if an orbiting moon has an incredibly tight orbit.
- Fix introduced for the NaN error when fast traveling in sandbox.
- Fixed telluric planet and gas giant sizes being able to generate up to 510 and 5100 respectively, causing errors on terrain generation and blueprint copying.
- Fixed planet numeral designations changing on loading a save or renaming the host star.
- Fixed an ILS vessel incompatibility issue between Galactic Scale and MoreMegaStructures.

## 2.80.2

### Changes by Devious55

**Features**

- Foundation Brush Size buttons for 20x20, 10x10, 5x5, 1x1. The last two options are mostly there to quickly switch from 20x20 back down to more precise amounts.
- Planetary Shield scaling in settings, found under Galactic Scale quality of life options on the left. Sidenote, this will turn itself off if you're using Starfish or un1eagle's Planetwide Shield Generator mods.
- Gas Giant selector for when your birth planet is the moon of one.
- By selecting the settings Birth Planet is a Moon and ...of a Gas Giant, your birth planet will now be the moon of a rocky planet orbiting a gas giant. The two settings are independent, so if you want your birth planet to only orbit a gas giant as its moon, then select ...of a Gas Giant.
- In the Gas Giant selector, you may discover an Oil Giant. I found this while looking through the themes, and it works, so I made it available as a starter giant selection.

**Bugfixes**

- New game screen Metadata UI has been moved to the top left to avoid covering Galactic Scale hotkey information and make all your available Metadata visible.
- New game ore veins shouldn't be able to generate inside other ore veins now.
- Recursive moons fixed. They should all now be solid on approach and allow you to land without flying through them.
- Dyson Sphere UI out of range fixed to account for ZordDevzz's orbit bounds change.
- Fixed Birth Planet theme selection for both standard spawn and moon spawn.
- Fixed Index out of range when Dark Fog bases were too close to the poles, and you tried to set foundations anywhere on the planet.
- Fixed being able to set your spawn on a gas giant.
- Fixed recursive moons being able to orbit into gas giants.

Thanks to Dinglebatz for helping test the recursive moon fixes.

## 2.80.1

- Devious55 Fixed several bugs

## 2.80.0

- Xabsurd updated for DSP 0.10.35

## 2.78.8

- AndrewLuebke fixed planets of overlapping systems being invisible and unlandable (#294): with very large orbits a neighboring star could hold onto you and the planet you were at could never load. Locality now hands off to the star whose planet you're approaching. Existing saves benefit immediately; no galaxy changes.

## 2.78.7

- ZordDevzz reworked Dyson sphere scaling: orbit bounds now follow system geometry (star surface to farthest planet +1 AU) instead of exploding with luminosity, with editor camera sync and sail/rocket speed scaling for large systems. AndrewLuebke fixed logistics vessels getting stuck near giant stars (steering-update throttle collateral + the missing star-radius pathing cap).

## 2.78.6

- AndrewLuebke fixed the second new game in a session generating no veins (#233) and made galaxy generation fully deterministic (#290): the same seed string now produces the same system names, themes, and vein amounts every time, across restarts. The seed-to-galaxy mapping shifts once with this update (it was never actually stable before); existing saves are unaffected.

## 2.78.5

- AndrewLuebke fixed fleet auto-targeting on large systems (#258): craft star-frame thresholds now scale to the system/hive envelope.

## 2.78.4

- Fixed the 2.78.1 vein/geothermal radius patches being applied twice (#284, thanks ZordDevzz); turret radius fix (#286); relay landing ETA on hover (#287).

## 2.78.3

- Zord fixed a bug I introduced when merging PR's without paying attention to the merge conflicts.

## 2.78.2

- AndrewLuebke fixed another bug with the planet radius.

## 2.78.1

- AndrewLuebke fixed 4 bugs with planet radius causing errors in: Beacon hover altitude, Geothermal, Blueprints and Veins.

## 2.78.0

- AndrewLuebke fixed three other issues.

## 2.77.6

- AndrewLuebke fixed a thread safety issue that probably caused crashes loading planets.

## 2.77.5

- AndrewLuebke fixed an assumption with themeProto array sizing

## 2.77.4

- AndrewLuebke fixed an error when pasting blueprints

## 2.77.3

- Update Base Game Version 0.10.34.28485

## 2.77.2

- Fix Roman Numeral Bug for outer planets

## 2.77.1

- Update for game patch 0.10.34.28347-r.0

## 2.77.0

- Update for game patch 0.10.34.28326-r.0

## 2.76.3

- Fix vegetation generation crash by ensuring vegeCursor is properly reset.

## 2.76.2

- Fix critical memory leak when mods query vein counts (was causing 128GB+ RAM usage).

## 2.76.1

- Update GSUI for Unity 2022.3

## 2.76.0

- Attempt to fix for latest multithread patch 0.10.33.26934

## 2.75.11

- Arflipe's fix for hives not attacking

## 2.75.10

- Fix Crash when saving

## 2.75.9

- Add finer galaxy control to Dev Generator. Add Logging for some save bugs, and Communicator bugs. Make sure you turn logging on! (And in BepInEx config)

## 2.75.8

- More Null Checks

## 2.75.7

- Init RawData if missing

## 2.75.6

- Add Nullcheck and wonder how I got the version numbering so hopelessly out of order

## 2.75.5

- Starfish saves the day (again). Many thanks!

## 2.74.4

- Attempt to fix memory leak

## 2.73.3

- Force Modelling of Planets instead of scanning. Prevent Automatic Scanning every 10s

## 2.73.2

- Init ModData on save to try prevent error

## 2.73.1

- Fix game loading error when you save in outer space

## 2.73.0

- Update for DSP Version 0.10.32.25779

## 2.72.5

- Add O-Type Giant

## 2.72.4

- Revert broken relay ship fix

## 2.72.3

- Fix ObserveLevel bug. Add debug option to set observe level. Attempt to fix df relays and logistics ships not pathing past large stars

## 2.72.2

- Fix DF Communicator being broken. Fix Crash with large starcount

## 2.72.1

- attempt to fix decimal numbers

## 2.72.0

- starfish fixed loading :D.

## 2.71.1

- Attempt to fix game not starting sometimes

## 2.71.0

- Fix for new Game Update

## 2.17.1

- Attempt to prevent star view distance from being reset

## 2.17.0

- Update Game Version

## 2.16.6

- Fix Dyson Sphere Radius (New Saves. Edit the GS2 File otherwise)

## 2.16.5

- Fix planet priority route function

## 2.16.4

- Fix crash on starting a new game with prologue

## 2.16.3

- Fix bug selecting starting planet star

## 2.16.2

- Fixed error on new game

## 2.16.1

- Potential Fix for Bug on New Game

## 2.16.0

- Tentative update game version 0.10.31.24646

## 2.15.4

- Default star size to 1x to try and prevent some pathing errors on df ships

## 2.15.3

- Fix Miners on small/large planets

## 2.15.2

- Enable option to ignore gameload timeout for Nebula users

## 2.15.1

- Merge Dev Generator with GS2 Generator to fix broken moon of gas giant starts.

## 2.15.0

- Update DSP Game Version 0.10.30.23292

## 2.14.3

- Fixed bug where Gas Giant Moon Chance was ignored

## 2.14.2

- Starfish fixed: Fix trash doesn't pick up by BAB soon on outer planets.  Fix throwable dealt no damage on different size planets.  Fix throwable curvature on outer planets. Fix Nebula client error when joining in space

## 2.14.1

- Fix Gas Giant Moon Start

## 2.14.0

- Update DSP Version

## 2.13.4

- Even more hopefully fix bugs with rockets and spheres. Hopefully fix but with rare veins spawning on gas giants

## 2.13.3

- Hopefully fix bugs with rockets and spheres

## 2.13.2

- Nebula Compatibility

## 2.13.1

- Hopefully fix DF Drops not relating to planet type. Fixed the planet theme descriptions for Vanilla Themes. GS Themes still use their base theme... for now.

## 2.13.0

- Recompile for DSP Update. Removed code for starmap, might break it. Let me know if it does.

## 2.12.47

- Move Darkfog Hives that are spawned outside of system limits

## 2.12.46

- Modify clearance value that logistics vessels use to avoid stars. They might fly through them now, but hopefully not. At least they should arrive instead of flying in circles

## 2.12.45

- Add debug option to suspend processing of Darkfog at a certain lightyear distance from the player (For those with major performance issues)

## 2.12.44

- Hopefully Fix loading on xbox GamePass

## 2.12.43

- Enable Darkfog Settings in JSON Galaxy Import (In Settings). Enjoy! Fixed Birth System Dark Fog Spawn

## 2.12.42

- Enable loading Splash Images from config/Splash folder. Preserve aspect ratio set for all splash images to avoid stretching on widescreens.

## 2.12.41

- Fix BP error. For real this time. Well, at least one of them. Drastically increased the size of some SectorLogic arrays, hopefully it stops the game breaking when you have a lot of stars/hives

## 2.12.40

- Fix BP error. Add Birth Planet Orbit setting to GS2 Generator. Better defaults for orbit and hz override which are enabled by default. New splashscreen.

## 2.12.39

- Remove BCE dependency

## 2.12.38

- Fix projectiles not working on small planets. Thanks starfish!

## 2.12.37

- Fixed log spam breaking new game

## 2.12.36

- Added Birth Planet Selector. Might not work if your birthplanet is a moon... Let me know if it doesn't work for you.

## 2.12.35

- Fix for error when hovering over stars and seeds simultaneously

## 2.12.34

- Fix for new DSP Update 0.10.28.21308 Starmap should be feature complete vs vanilla now.

## 2.12.33

- Fix not being able to attach belts to ILS on comets

## 2.12.32

- Fix error when due to large starcounts the game creates more idle relays than it has space for

## 2.12.31

- Fix missing text on settings buttons. Fix issue on comets placing belts under turrets. Tidy up settings. Add missing hint text. Add dev options.

## 2.12.30

- Fixed blueprinting Planetary Logistics Stations. Finally remembered to remove useless debug options from settings.

## 2.12.29

- Fix Galaxy Generation where BirthPlanet is a moon. Fix error where factories array is not set higher than planetcount (Some other Mod hijacking the prefix patch?)

## 2.12.28

- Fix Planetary Shield on small/large planets. Thanks karki!

## 2.12.27

- Fixed copy/paste not working properly. Inserters should now work properly on all planet sizes. Let me know on discord if there are any issues.

## 2.12.26

- Fix Hives so they DONT show in starmap. Fixed them so they show in normal play. Fixed Dark Fog Communicator not being labelled.

## 2.12.25

- Fix Hives not showing in starmap. Fixed not being able to reach logistics stations. Increased reach distance in general by a few meters.

## 2.12.24

- Fix some bugs introduced last patch.

## 2.12.23

- Remove force garbage collect when building. Massive performance improvement. Thanks to starfish for the code. Fixed Enemies on other planets. Fixed some errors. Fixed some logspam.

## 2.12.22

- Recompile for DSP .21219

## 2.12.21

- Fix for water on Pandora

## 2.12.20

- Prevent fix for clipping enemies turning off (Hopefully)

## 2.12.19

- Fix for gravity on moons orbiting huge gas giants. Fix can be reverted via toggle in GS Settings/Debug. Improved the code for the fix in 2.12.18 to increase performance.

## 2.12.18

- Fix for enemy units clipping through large planets. Not tested on smaller planets. Please let me know if it breaks anything.

## 2.12.17

- Revert csd fix which made things worse

## 2.12.16

- Fixed issue with resetting generator. Fixed settings not being saved. Fixed Generator Settings defaulting to GS2. Prevented a bunch of logspam.

## 2.12.15

- Fixed issue with comma seperated decimals. Fixed issue with GS2 Dev Generation. Ignored error when autosaving over locked gs2 files.

## 2.12.14

- Fixed issue loading saves

## 2.12.13

- Fixed Pandora Planet

## 2.12.12

- Update to newest DSP Version. Some new features may be bugged, only tested that the game works.

## 2.12.11

- Possibly fix broken saves. Might break unbroken saves. Needs testing!

## 2.12.10

- Fixed bug The_Iron_Angel caught when localPlanet was not present yet you killed something

## 2.12.9

- Fixed a lot more darkfog stuff - Will move hives around.

## 2.12.8

- Fixed a bunch of darkfog stuff

## 2.12.7

- Fixed Geothermal Power on DF Core Mines

## 2.12.6

- Combat working on small planets (So far as I have tested at least)

## 2.12.5

- Compatibility. Removed Replicator Alt Buttons.

## 2.12.4

- Much better combat on slightly smaller planets.

## 2.12.3

- Attempt at getting more stars to work

## 2.12.2

- Attempt at getting some combat systems working. Seems to be working on normal to large planets, but I need it tested. Small planets seem to be broken still.

## 2.12.1

- Fixed a lot of bugs in New Game screen. Dark Fog Settings work now. Only tested with 200 radius planets. Seems to be working at least for the first few mins.

## 2.12.0

- Initial Dark Fog Test Build. DONT CRY IF THIS EXPLODES YOUR PC. Very Buggy. Only for testing, would not recommend trying to actually play the game on this build. Have fun in vanilla unless you're helping fix the bugs :)

## 2.11.11

- Add Binary Star Fix from Arniox, and Soarqin's autobridging belt fix. Thanks to starfish for the transpiler, and for paying attention when I was asleep at the wheel.

## 2.11.10

- Translation additions. Thanks starfish

## 2.11.9

- Added GalacticScale Dev Generator, added Recursive Moon option. Removed Vanilla Gen due to lack of maintenance.

## 2.11.8

- Attempt to fix localization issues

## 2.11.7

- Fix Galaxy Generation when moon change is high

## 2.11.6

- 2nd Attempt

## 2.11.5

- Attempt to fix Null Reference error with Infernal Giant

## 2.11.4

- Fix Null Reference Error when quitting

## 2.11.3

- Fix errors in roman number display when more than 20 planets are used. Fix vein display problems. Fix Fast Travel button. Thanks starfish!

## 2.11.2

- Bump version number

## 2.11.1

- Fix of the fix

## 2.11.0

- Fix disappearing planets in far flung orbits. Fix some clouds. Add Gas Giant Size Settings which may or may not break the game. Revert to previous version if so and let me know.

## 2.10.3

- Fix error with birthplanet theme

## 2.10.2

- Possibly savebreaking, Vein algorithm fixes, multithreading fix. Thanks karki :)

## 2.10.1

- karki multithreaded some star stuff and got speed improvements! :D

## 2.10.0

- karki fixed Mk2 Miner Graphical Error, Sped up Starmap :D Thanks!

## 2.9.0

- karki fixed birth planet generation and an error on exit. Thanks!

## 2.8.8

- Added ability to begin on a moon to GS Generator Settings

## 2.8.7

- Incorporate 2.8.5 which was missed from .6

## 2.8.6

- Update for DSP 0.9.27.14659

## 2.8.5

- starfish's xgp fix

## 2.8.4

- Attempt at fixing mod compatibility.  Reverts 2.8.3.

## 2.8.2

- Remove non existing vegetation "Jungle Tree 3"

## 2.8.1

- Fix Logistic Bot Speed. For games created on 2.8.0 a button is added under Settings/GalacticScale/Debug that should fix the issue.

## 2.8.0

- Update for new DSP Version. Implemented starfish's warning system fix.

## 2.7.12

- Additional monitoring component null reference avoidance

## 2.7.11

- Fix very rare bug with monitoring component trying to use a speaker that doesn't exist in the pool. *shrug*

## 2.7.10

- Hopefully fix Nebula compatibility

## 2.7.9

- Fix no rares in starting system, and pregame system viewer improvements.

## 2.7.8

- Fix strange mod conflict caused by debug logging

## 2.7.7

- Another attempt to fix issues loading saves

## 2.7.6

- Fix errors when starting/ending game while still calculating veins

## 2.7.5

- Fix error when loading game while planets are calculating

## 2.7.4

- Combine Gas/Liquid Items in Star Detail again. Fix System view UI. Fix Sandbox toggle in newgame and
  load game.

## 2.7.3

- Hopefully fix errors introduced by 2.7.2

## 2.7.2

- Fixed issue with birthplanet veins not being calculated. Thanks to starfish. Fixed vein display in new
  game system viewer. Merged LittleSaya's PR with Transpiler for PlanetFactory.FlattenTerrain. Known issue: With lots of
  planets the star details will be overflowing with oceans. Will fix soon.

## 2.7.1

- Two bugfixes regarding outer planets and gas giants giving errors when viewing details

## 2.7.0

- Updated for DSP 13026. May or may not work. Need feedback in discord. They changed a bunch of stuff
  relating to veins. BACKUP YOUR SAVES

## 2.6.0

**BACKUP YOUR SAVES. PLEASE.**

- Fixed terrain generation code that has been broken since version 2.0.0. I wouldn't upgrade if you have
  an existing save. Your planets may morph into their true form. This probably isn't a good thing if you have buildings
  on them.

## 2.5.15

- Add new Planet Types (IceGelisol 2/3, FrozenTundra, PandoraSwamp 1/2, Crystal Desert, Savanna)

## 2.5.14

- Update for DSP 12900 (New Planets Not Yet Added, I'll be working on them today)

## 2.5.13

- Starfi5h's Nebula Compatibility fix

## 2.5.12

- Better Nebula Compatibility RE: Spray Coater Graphics

## 2.5.11

- Add toggle for starting system rare

## 2.5.10

- Add slider for planet rotation in Sol Generator for @Cidno

## 2.5.9

- Possible bug fix for reported veins in starmap

## 2.5.8

- Fix Scarlet Ice Lake Ice. Can now walk/build on.

## 2.5.7

- Fixed duplicate space capsule

## 2.5.6

- Improve Teleportation Functionality

## 2.5.5

- Fix Mecha Scaling when teleporting and add debug scale value for funsies.

## 2.5.4

- Fix Vanilla Generator Saves

## 2.5.3

- Added some missing code from recent updates that dpmm99 discovered

## 2.5.2

- Fixed Ice Gellisol rocks

## 2.5.1

- Fixed Autosave and LastExit

## 2.5.0

- Changed the way GS Handles Saves. No longer injects GS data into the DSP save file.

## 2.4.26

- BACKUP SAVES!!! Prevents some errors when loading saves.

## 2.4.25

- BACKUP SAVES!!! Assumed Working in latest DSP Update. Save loading fixed.

## 2.4.24

- Recompile for latest DSP Update 0.9.25 , untested. May still not work or throw errors.

## 2.4.23

- Fix Camera Shift when loading game after moving camera in system display

## 2.4.22

- Add Inner Planet Distance as Star Specific Override to GSGen

## 2.4.21

- Fix bug with Vanilla Gen

## 2.4.20

- Fix bug with comet generation

## 2.4.19

- Fix Planet Placement and Solar Values in GSGen. Fix SystemDisplay. Add "Start at PlanetName". Fix
  Nebula Compatibility (thanks starfish!)

## 2.4.18

- Fix orbit spacing :)

## 2.4.17

- Add options to change star/planet naming in GalacticScale Generator

## 2.4.16

- Add variable to increase orbit spacing to GalacticScale Generator

## 2.4.15

- Add solar power settings to GalacticScale Generator

## 2.4.14

- Improve Nebula Compatibility

## 2.4.13

- Fix another blueprint bug

## 2.4.12

- Hopefully fix blueprint error on 510 size planets.

## 2.4.11

- Hopefully fix logistic vessel pathing around huge stars. Thanks 46bit

## 2.4.10

- Hopefully more compatibility with Nebula. Fixed Spraycoater attaching to existing belt. Fixed
  spraycoater graphical glitch. Fixed a few small bugs. Thanks starfish!

## 2.4.9

- Fixed blueprints using extra space

## 2.4.8

- Fixed Lava & Hurricane Themes missing rocks. Added hint text to galaxy select. Clamped solar power to
  1-500%. Fixed GS2 Star Overrides not loading correctly. Fixed bug where planetdetail wouldnt close on game start.

## 2.4.7

- Fixed Error when saving games with short names. Added configurable mouse tolerance for systemdisplay.
  Fixed Planet name text flickering in center of screen when clicked.

## 2.4.6

- Fixed Warning System for Raptor (Hopefully). Fixed Quickstart bug from 2.4.3

## 2.4.5

- Fixed Autosave Galaxy Setting Spam. Will require generator recompile.

## 2.4.4

- Fixed error in console code. Will require generator recompile.

## 2.4.3

- Fixed space capsule doubleup on non skip gamestart

## 2.4.2

- Fixed Vanilla and Sol Generators

## 2.4.1

- Fix the icon :)

## 2.4.0

- Merge Experimental and Main Branches - Fix a bunch of bugs

## 2.3.16

- Comets added to GS2 Generator

## 2.3.15

- Test GS2 More realistic solar power. Settings update. Will require generator recompile.

## 2.3.14

- Fixed blueprints at poles. Now deletes logs older than a week

## 2.3.13

- Fixed issues in galaxy select. Fixed loading game when in space. Fixed loading game putting player
  inside planet. Fixed Obsidian theme for new games.

## 2.3.12

- Removed a lot of logspam. Fixed error on quit while planets loading

## 2.3.11

- Fixed version

## 2.3.10

- Fixed the galaxy select star/planet labels. Hit alt to get them all to show

## 2.3.9

- @starfi5h 's super fast start implemented. No more waiting for every planet to load! Fixed bug when
  quitting before all planets are generated.

## 2.3.8

- Fix gamestart - Nebula tested and working at least as far as new game is concerned

## 2.3.7

- SystemDisplay updates, Nebula Fixes. Maybe nebula works, maybe it doesn't :)

## 2.3.6

- Maybe nebula works, maybe it doesn't :)

## 2.3.5

- Fixed cargo ships not docking on outer planets in large systems

## 2.3.4

- Further refinement of system view

## 2.3.3

- Implemented System View in Galaxy Select Screen. Click a star to view the system. Currently testing
  scale of planets etc, help by trying new values under Debug Options in settings. Post screenshots to our discord if
  you find a combination that works well. I'm thinking we might need to apply some sort of logarithmic compression to
  boost tiny planets and shrink huge gas giants.
- Added CloseError as a dependency. You're probably going to get errors, why look at the red box of annoyance if you
  don't have to?

## 2.3.2

- Finally fixed North/South Pole markers on non standard planet sizes :)

## 2.3.1

- Changed Luminosity Boost. Fixed Slider rounding error. Fixed Moon size for Starter Planet.

## 2.3.0

- Experimental UI Changes to sliders. Includes button to switch between slider and text entry. Floating
  point bug not fixed. :(
