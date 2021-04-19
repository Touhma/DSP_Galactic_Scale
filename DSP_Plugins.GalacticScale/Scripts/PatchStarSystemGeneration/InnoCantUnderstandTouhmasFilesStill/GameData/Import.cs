//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

//namespace GalacticScale
//{
//    public static class PatchOnGameData
//    {
//        [HarmonyPatch(typeof(GameData))]
//[HarmonyPrefix, HarmonyPatch("Import")]
//public static bool Import(BinaryReader r, ref GameData __instance)
//{
//    Patch.Debug("Begin Import");
//    int num1 = r.ReadInt32();
//    __instance.patch = 0;
//    if (num1 >= 4)
//        __instance.patch = r.ReadInt32();
//    __instance.account = AccountData.NULL;
//    if (num1 >= 3)
//        __instance.account.Import(r);
//    if (__instance.account.isNull)
//        __instance.account = AccountData.me;
//    __instance.gameName = r.ReadString();
//    __instance.gameDesc = new GameDesc();
//    __instance.gameDesc.Import(r);
//    DSPGame.GameDesc = __instance.gameDesc;
//    DSPGame.LoadFile = string.Empty;
//    DSPGame.LoadDemoIndex = 0;
//    GameMain.gameTick = r.ReadInt64();
//    Patch.Debug("Beginning to Create Galaxy");
//    __instance.galaxy = UniverseGen.CreateGalaxy(__instance.gameDesc);
//    Patch.Debug("Updating Poses");

//    __instance.galaxy.UpdatePoses(GameMain.gameTime);
//    Patch.Debug("Creating Factories");

//    __instance.factories = new PlanetFactory[__instance.gameDesc.starCount * 6];
//    __instance.factoryCount = 0;
//    Patch.Debug("Setting Preferences");

//    __instance.preferences = new GamePrefsData();
//    __instance.preferences.Init(__instance);
//    if (num1 >= 1)
//        __instance.preferences.Import(r);
//    else
//        __instance.preferences.SetForNewGame();
//    __instance.preferences.EarlyRestore();
//    Patch.Debug("Regenerating Names");

//    __instance.galaxy.RegeneratePlanetNames();
//    __instance.history = new GameHistoryData();
//    __instance.history.Init(__instance);
//    Patch.Debug("Importing History");

//    __instance.history.Import(r);
//    if (num1 >= 2)
//    {
//        __instance.hidePlayerModel = r.ReadBoolean();
//        __instance.disableController = r.ReadBoolean();
//    }
//    else
//    {
//        __instance.hidePlayerModel = false;
//        __instance.disableController = false;
//    }
//    Patch.Debug("Initializing Statistics");

//    __instance.statistics = new GameStatData();
//    __instance.statistics.Init(__instance);
//    __instance.statistics.Import(r);
//    Patch.Debug("Creating Planet");

//    PlanetData planet = __instance.galaxy.PlanetById(r.ReadInt32());
//    Patch.Debug("Spawing Player");
//    //Player np = Player.Create(__instance, __instance.gameDesc.playerProto);
//    //MethodInfo f = AccessTools.PropertySetter(typeof(GameData), "mainPlayer");
//    //MethodInvoker.GetHandler(AccessTools.PropertySetter(typeof(GameData), "mainPlayer")).Invoke();
//    //AccessTools.Property(typeof(Player), "planetData").SetValue(GameMain.mainPlayer, tmpData, null); //GameMain.mainPlayer.planetData = tmpData;
//    Player np = Player.Create(__instance, __instance.gameDesc.playerProto);
//    AccessTools.Property(typeof(GameData), "mainPlayer").SetValue(__instance, np, null);
//    //Traverse.Create(__instance).Property("mainPlayer").SetValue(np);
//    // AccessTools.Property(typeof(GameData), "mainPlayer").SetValue(GameMain.data, np, null);
//    Patch.Debug("Importing Player Data");

//    __instance.mainPlayer.Import(r);
//    __instance.factoryCount = r.ReadInt32();
//    Patch.Debug("Initializing Transport System");

//    __instance.galacticTransport = new GalacticTransport();
//    __instance.galacticTransport.Init(__instance);
//    __instance.galacticTransport.Import(r);
//    Patch.Debug("Creating Trash");

//    __instance.trashSystem = new TrashSystem(__instance);
//    if (num1 >= 5)
//        __instance.trashSystem.Import(r);
//    else
//        __instance.trashSystem.SetForNewGame();
//    for (int _index = 0; _index < __instance.factoryCount; ++_index)
//    {
//        __instance.factories[_index] = new PlanetFactory();
//        __instance.factories[_index].Import(_index, __instance, r);
//    }
//    __instance.galacticTransport.Arragement();
//    __instance.galacticTransport.RefreshTraffic();
//    int length = r.ReadInt32();
//    Assert.True(length == __instance.galaxy.starCount);
//    if (length < __instance.galaxy.starCount)
//        length = __instance.galaxy.starCount;
//    Patch.Debug("Rolling Spheres");

//    __instance.dysonSpheres = new DysonSphere[length];
//    for (int index = 0; index < length; ++index)
//    {
//        Patch.Debug("Loading Dyson Sphere index " + index);
//        int num2 = r.ReadInt32();
//        Patch.Debug("num2 = " + num2);
//        if (num2 == 1)
//        {
//            Patch.Debug("num2 == 1");
//            __instance.dysonSpheres[index] = new DysonSphere();
//            __instance.dysonSpheres[index].Init(__instance, __instance.galaxy.stars[index % __instance.galaxy.starCount]);
//            __instance.dysonSpheres[index].Import(r);
//        }
//        Assert.True(num2 == 1 || num2 == 0);
//    }
//    Patch.Debug("Arriving on Planet");

//    if (planet != null)
//        __instance.ArrivePlanet(planet);
//    else
//        __instance.DetermineLocalPlanet();
//    __instance.DetermineRelative();
//    __instance.warningSystem = new WarningSystem(__instance);
//    Patch.Debug("Finished Import");

//    return false;
//}