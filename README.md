# DSP Touhma GalacticScale Plugin


## Disclaimer

This is a Beta Release version of the mod. Expects bugs , crash etc ...

Your save WILL break !

I need some feedback & some bug report to fix it. Plus i'm opening the mod for people who think can help me fixing it.

Have fun with it and good testing !

## How to Install :

First Install Bepinex in your game
folder : https://bepinex.github.io/bepinex_docs/master/articles/user_guide/installation/index.html?tabs=tabid-win

Then Download the latest DLL of the mod : https://github.com/Touhma/DSP_Plugins/releases

And add it in depinex plugins folder in your game : %gamefolder%\Dyson Sphere Program\BepInEx\plugins

Launch the game and you should be all set !

## How to use galactic-scale ?

### Map Size :

You can slide the number of system up to 1024 ( by default ) instead of 64. Be carefull if your pc is slow ^^ ---> everything past 64 is experimental ! Test it at your own risk :)

#### Config Available :

Edit the file here : %gamefolder%\Dyson Sphere Program\BepInEx\config\touhma.dsp.galactic-scale.galaxy-size-select.cfg

#### Limitations :

Although you can put more than 1024 stars in the config file the slider will get stuck on a number between 1024 & 1300 depending on the seed you are using.

## How to use the custom system Generation ?

#### Planet Sizes :
One of the main feature of this mod is to allow for more variety for planets & moons. So now you will be able to find different size planets & moons all around the universe ! Good luck with that limited space :D
Plus Gas Giant have been reworked too, their size is variable too.

#### Moon Changes :
This mod allow for telluric moons around gas giant BUT you can have telluric moon around telluric planets too !

#### System Generation :
A first version of the reworked algorithm is available. It allow you to :
- Customize how each system type is generated,
- Customize for each how much : Planets , Moons are available
- Customize the chance for a planet in the habitable radius of the system to be ... well .. habitable
- Customize how large the habitable radius is
- Customize the orbits of the planets
- Customize the orbits of the moons around their host planets
- Customize the density of the system & how dense moons systems are
- Customize the min or max size for telluric & GasGiant planet

#### Config Available :

Edit the file here :
%gamefolder%\Dyson Sphere Program\BepInEx\config\touhma.dsp.galactic-scale.star-system-generation.cfg

A more complete documentation will come later :)

# How to use the QOL mod ?

### Dismantling :

Everything is available from the dismantle tool ingame :
<ul>
    <li>Use the tool normally for vanilla usage</li>
    <li>Use the LeftControl key to enter in Sphere mode, use the reformPlusKey  or reformMinusKey ( + & - on NUMPAD by default ) to increase or decrease the size of the sphere</li>
    <li>The sphere start either from the ground or from the first building touched</li>
    <li>Use the LeftShift key to enter in General mode, it allow you to select all the conveyor belt of a specific node</li>
    <li>Use the LeftShift key to enter in General - Sphere mode, it allow you to select all the specific Building in a specific Area. ex : all conveyors in a specific sphere. use the reformPlusKey  or reformMinusKey ( + & - on NUMPAD by default ) to increase or decrease the size of the sphere</li>
</ul>

#### Quick Key map :

<ul>
    <li>LeftControl : Sphere mode</li>
    <li>reformPlusKey  or reformMinusKey ( + & - on NUMPAD by default ) :  increase or decrease the size of the sphere</li>
    <li>LeftShift key : General mode </li>
    <li>LeftShift key + Left Control key to enter in General - Sphere mode</li>
</ul>

#### Config Available :

Edit the file here : %gamefolder%\Dyson Sphere Program\BepInEx\config\touhma.dsp.plugins.qol-features.cfg


## For modders :

I spent a shit ton of time working on that mod so please ... don't be a dick.
If you have some features ideas that would need my code to work, talk to me first ( Touhma#1599 ) on Discord about it then we shall decide how we can proceed.
If you can : make your own work using this mod as a dependency if you don't wanna improve this mod.
If you wanna fix something in it or improve on it --> please go ahead and do a pull request, i'll hapilly credit you for your work.

Do NOT make a mod just to change the default configuration or a few parameters, DO share your configs files with me :) if it's giving good result it can make it to the official release of the mod.

I would love that mod to be a reference in the comunity concerning everything around the generation of the moons, planets, stars, etc ... I have a shit tons of ideas for it and i'll probably not be able to take car e of everything by myself as fast as I wish for. So consider helping me improving it instead of copy pasting the code & releasing it under a new name.

This mod is and will always be free of charge for everyone.

I'll see in the future if people wanna give me a pint of beer if they wannna support me but again : it will stay open & free of charge for everyone.

If you wanna reuse some piece of code : make your own work free of charge & open source too .