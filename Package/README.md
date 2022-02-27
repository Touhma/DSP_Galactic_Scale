# DSP Galactic Scale 2.0 Mod

- Version 2.3.13 - Fixed issues in galaxy select. Fixed loading game when in space. Fixed loading game putting player inside planet. Fixed Obsidian theme for new games.

- Version 2.3.12 - Removed a lot of logspam. Fixed error on quit while planets loading

- Version 2.3.11 - Fixed version

- Version 2.3.10 - Fixed the galaxy select star/planet labels. Hit alt to get them all to show

- Version 2.3.9 - @starfi5h 's super fast start implemented. No more waiting for every planet to load! Fixed bug when quitting before all planets are generated.

- Version 2.3.8 - Fix gamestart - Nebula tested and working at least as far as new game is concerned

- Version 2.3.7 - SystemDisplay updates, Nebula Fixes. Maybe nebula works, maybe it doesn't :)

- Version 2.3.6 - Maybe nebula works, maybe it doesn't :)

- Version 2.3.5 - Fixed cargo ships not docking on outer planets in large systems

- Version 2.3.4 - Further refinement of system view

- Version 2.3.3 - Implemented System View in Galaxy Select Screen. Click a star to view the system. Currently testing scale of planets etc, help by trying new values under Debug Options in settings. Post screenshots to our discord if you find a combination that works well. I'm thinking we might need to apply some sort of logarithmic compression to boost tiny planets and shrink huge gas giants. 
- Added CloseError as a dependency. You're probably going to get errors, why look at the red box of annoyance if you don't have to?

- Version 2.3.2 - Finally fixed North/South Pole markers on non standard planet sizes :)

- Version 2.3.1 - Changed Luminosity Boost. Fixed Slider rounding error. Fixed Moon size for Starter Planet.

- Version 2.3 - Experimental UI Changes to sliders. Includes button to switch between slider and text entry. Floating point bug not fixed. :(


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

If you really want to support us, there's a donation link at the bottom of the about page on our website http://customizing.space

Credits:

innominata - GS1/GS2/GS3 Code/Art
Touhma - GS1 Mod Developer / Advisor
Shad0wlife - GS1/GS2 Code / Advisor
GlitchHound - GS1 Code / Advisor
sp00ktober - GS2/GS3 Code /Advisor
tyukara - GS2 Japanese Translation
Mushroom - GS1 Dev Env
PhantomGamers - GS2 Dev Env / Advisor
starfish - GS3 Code / Advisor
Kremnev8 - GS2 Code / Advisor
NHunter - GS2 Code / Advisor
