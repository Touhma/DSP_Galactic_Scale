using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

// Updated with communicator position fixes and removed problematic Harmony patch

namespace GalacticScale
{
    public partial class PatchOnUISpaceGuide
    {
        ////Strategy: Replace ldc.i4.s 10 instructions with a dynamic addition equal to the current system's planet count
        //// Get the local system:
        ///* 0x000E0746 02           */// IL_034A: ldarg.0
        ///* 0x000E0747 7B0E190004   */// IL_034B: ldfld class GameData UISpaceGuide::gameData
        ///* 0x000E074C 6F4C090006   */// IL_0350: callvirt instance class StarData GameData::get_localStar()
        //// Get the planet count
        ///* 0x000E0751 6F970A0006   */// IL_0355: ldfld instance int StarData::planetCount
        ////
        ////
        //[HarmonyTranspiler, HarmonyPatch(typeof(UISpaceGuide), "_OnLateUpdate")]
        //public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) => ReplaceLd10(instructions);
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISpaceGuide), "_OnLateUpdate")]
        public static bool _OnLateUpdate(ref UISpaceGuide __instance)
        {
            var sailing = __instance.player.sailing;
            var _guidecnt = 0;
            var flag = __instance.gameData.guideComplete || __instance.gameData.guideMission.elapseTime > 36f;
            if (!VFInput.inFullscreenGUI && flag)
            {
                var pId0 = __instance.gameData.localStar == null ? 0 : __instance.gameData.localStar.id * 100;
                __instance.relPos = __instance.gameData.relativePos;
                __instance.relRot = __instance.gameData.relativeRot;
                var position = __instance.gameCamera.transform.position;
                var camUPos = __instance.relPos + Maths.QRotateLF(__instance.relRot, position);
                var directionOfView = Maths.QRotateLF(__instance.relRot, __instance.gameCamera.ScreenPointToRay(Input.mousePosition).direction);
                var flag2 = VFInput.onGUI || VFInput.onGUIOperate;
                var num = 0;
                for (var index = 1; index <= __instance.galaxy.starCount; index++) //For each star in the galaxy
                {
                    var starData = __instance.galaxy.StarById(index);
                    if (starData != null) //If there is a star
                    {
                        var _rpos = Vector3.zero;
                        var showStarLabel = false;
                        if (__instance.uiGame.dfSpaceGuideOn)
                        {
                            if (__instance.gameData.localStar == starData) showStarLabel = true;
                            if (!showStarLabel && (__instance.history.GetStarPin(index) == EPin.Show || (starData.uPosition - __instance.player.uPosition).sqrMagnitude < 92160000000000.0)) //If the current star isnt local, and it's pinned, or its closer than 9216....
                            {
                                var starContainsPinnedPlanet = false;
                                var planetId = index * 100;
                                while (planetId <= index * 100 + 99) //Edited here
                                {
                                    if (__instance.astroPoses[planetId].uRadius <= 1f)
                                    {
                                        planetId++;
                                        continue;
                                    }

                                    if (__instance.history.GetPlanetPin(planetId) == EPin.Show) //Edited here
                                    {
                                        starContainsPinnedPlanet = true;
                                        break;
                                    }

                                    planetId++;
                                }

                                showStarLabel = !starContainsPinnedPlanet;
                            }

                            if (showStarLabel && __instance.history.GetStarPin(index) == EPin.Hide) showStarLabel = false; //If the star is hidden
                        }

                        if (!showStarLabel && !flag2) //if we dont want to show the star, and we are not ?in a star gui?
                        {
                            var starRelativePosition = starData.uPosition - camUPos;
                            var num2 = directionOfView.x * starRelativePosition.x + directionOfView.y * starRelativePosition.y + directionOfView.z * starRelativePosition.z;
                            if (num2 > 0.0)
                            {
                                num2 /= starRelativePosition.magnitude;
                                if (num2 > 0.99994)
                                {
                                    showStarLabel = true;
                                    num = index;
                                }
                            }
                        }

                        if (__instance.mouseInStar == index) showStarLabel = true;
                        if (showStarLabel)
                            _rpos = Maths.QInvRotateLF(__instance.relRot, starData.uPosition - __instance.relPos);
                        if (showStarLabel) showStarLabel = __instance.CheckVisible(pId0, index * 100, starData.uPosition, camUPos);
                        if (showStarLabel)
                            __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Star, index, 0, _rpos, starData.viewRadius - 120f);
                    }
                }

                if (__instance.gameData.localStar != null)
                {
                    var planetID = pId0 + 1;
                    while (planetID <= pId0 + 99 && __instance.astroPoses[planetID].uRadius > 0f) //1f to 0f
                    {
                        //GS2.Warn((index2).ToString() + " " + __instance.astroPoses[index2].uRadius.ToString() + " " + GameMain.galaxy.PlanetById(index2).name);
                        if (__instance.astroPoses[planetID].uRadius > 1f) //added conditional
                        {
                            var _rpos2 = Vector3.zero;
                            var showPlanetLabel = __instance.uiGame.dfSpaceGuideOn;
                            if (__instance.uiGame.dfSpaceGuideOn && !showPlanetLabel && __instance.history.GetPlanetPin(planetID) == EPin.Show) showPlanetLabel = true;
                            if (showPlanetLabel && __instance.history.GetPlanetPin(planetID) == EPin.Hide) showPlanetLabel = false;
                            if (!showPlanetLabel && !flag2)
                            {
                                var vectorLf3_3 = __instance.astroPoses[planetID].uPos - camUPos;
                                var num3 = directionOfView.x * vectorLf3_3.x + directionOfView.y * vectorLf3_3.y + directionOfView.z * vectorLf3_3.z;
                                if (num3 > 0.0)
                                {
                                    num3 /= vectorLf3_3.magnitude;
                                    if (num3 > 0.9999) showPlanetLabel = true;
                                }
                            }

                            if (__instance.mouseInPlanet == planetID) showPlanetLabel = true;
                            if (showPlanetLabel)
                            {
                                _rpos2 = Maths.QInvRotateLF(__instance.relRot, __instance.astroPoses[planetID].uPos - __instance.relPos);
                                if (_rpos2.magnitude > __instance.gameData.localStar.systemRadius * 6f * 40000.0)
                                    showPlanetLabel = false;
                                if (!sailing && __instance.gameData.localPlanet != null && __instance.gameData.localPlanet.id == planetID) showPlanetLabel = false;
                            }

                            if (showPlanetLabel)
                                showPlanetLabel = __instance.CheckVisible(pId0, planetID, __instance.astroPoses[planetID].uPos, camUPos);
                            if (showPlanetLabel)
                                __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Planet, planetID, 0, _rpos2, __instance.astroPoses[planetID].uRadius);
                        }

                        planetID++;
                    }
                }

                if (__instance.uiGame.dfSpaceGuideOn)
                {
                    var num4 = 0;
                    foreach (var pinnedPlanet in __instance.history.pinnedPlanets)
                    {
                        var key = pinnedPlanet.Key;
                        if (key >= num4)
                        {
                            var planetData = __instance.galaxy.PlanetById(key);
                            if (planetData != null && planetData.star != __instance.gameData.localStar)
                            {
                                var star = planetData.star;
                                if (__instance.gameData.localStar != star && __instance.mouseInStar != star.id && num != star.id)
                                {
                                    Vector3 _rpos3 = Maths.QInvRotateLF(__instance.relRot, __instance.astroPoses[key].uPos - __instance.relPos);
                                    __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Planet, key, 0, _rpos3, 0f);
                                    num4 = key + 100;
                                    num4 /= 100;
                                    num4 *= 100;
                                }
                            }
                        }
                    }
                }
                // Safer handling of hive processing with better null checks
                if (__instance.gameData.localStar != null && __instance.gameData.gameDesc.isCombatMode && __instance.gameData.guideComplete)
                {
                    // First check sector exists
                    if (__instance.sector == null)
                    {
                        // GS2.Warn("Sector is null - cannot process hives");
                    }
                    // Then check dfHives exists
                    else if (__instance.sector.dfHives == null)
                    {
                        // GS2.Warn("dfHives array is null - cannot process hives");
                    }
                    // Then check array bounds
                    else if (__instance.sector.dfHives.Length <= __instance.gameData.localStar.index || __instance.gameData.localStar.index < 0)
                    {
                        // GS2.Warn($"DFHives length ({__instance.sector.dfHives.Length}) is insufficient for localStar index ({__instance.gameData.localStar.index})");
                    }
                    // Finally check the specific array element isn't null
                    else if (__instance.sector.dfHives[__instance.gameData.localStar.index] == null)
                    {
                        // GS2.Warn($"DFHives at index {__instance.gameData.localStar.index} is null");
                    }
                    else
                    {
                        try
                        {
                            ProcessHiveStuff(ref __instance, ref _guidecnt);
                        }
                        catch (System.Exception ex)
                        {
                            // Catch any remaining exceptions in the method
                            GS2.Error($"Error processing hive stuff: {ex.Message}");
                        }
                    }
                }
                Array.Clear(__instance.shipDistArray, 0, 1024);
                if (__instance.uiGame.dfSpaceGuideOn && __instance.shipRenderer != null)
                {
                    var num5 = 0;
                    var shipCount = __instance.shipRenderer.shipCount;
                    if (shipCount > 1024) shipCount = 1024;
                    var shipsArr = __instance.shipRenderer.shipsArr;
                    var num7 = float.MaxValue;
                    for (var index3 = 0; index3 < shipCount; index3++)
                        if (shipsArr[index3].anim.z > 0.95f)
                        {
                            __instance.shipDistArray[index3] = shipsArr[index3].pos.sqrMagnitude;
                            if (__instance.shipDistArray[index3] < 160000f || __instance.shipDistArray[index3] > 4E+10f)
                            {
                                __instance.shipDistArray[index3] = 0f;
                            }
                            else
                            {
                                num5 = index3 + 1;
                                if (__instance.shipDistArray[index3] < num7) num7 = __instance.shipDistArray[index3];
                            }
                        }
                        else
                        {
                            __instance.shipDistArray[index3] = 0f;
                        }

                    var num8 = 4;
                    num7 *= 2.25f;
                    for (var index4 = 0; index4 < num5; index4++)
                        if (__instance.shipDistArray[index4] >= 160000f && __instance.shipDistArray[index4] < num7)
                        {
                            var upos = __instance.relPos + Maths.QRotateLF(__instance.relRot, shipsArr[index4].pos);
                            if (__instance.CheckVisible(pId0, 0, upos, camUPos))
                            {
                                __instance.SetEntry(ref _guidecnt, ESpaceGuideType.Ship, 0, (int)shipsArr[index4].itemId, shipsArr[index4].pos, 3f);
                                if (--num8 == 0) break;
                            }
                        }
                }

                // Handle cosmic messages - Updated to match latest game version
                if (GameMain.gameScenario != null)
                {
                    try
                    {
                        CosmicMessageManager cosmicMessageManager = GameMain.gameScenario.cosmicMessageManager;
                        if (cosmicMessageManager != null && cosmicMessageManager.messages != null)
                        {
                            CosmicMessageData[] messages = cosmicMessageManager.messages;
                            for (int l = 1; l < messages.Length; l++)
                            {
                                CosmicMessageData cosmicMessageData = messages[l];
                                if (cosmicMessageData != null && cosmicMessageData.protoId != 0)
                                {
                                    bool flag8 = false;
                                    int protoId = cosmicMessageData.protoId;
                                    
                                    // Safe LDB access
                                    try { LDB.cosmicMessages.Select(cosmicMessageData.protoId); }
                                    catch (System.Exception) { continue; }
                                    
                                    if (__instance.uiGame.dfSpaceGuideOnFinal && !flag8 && 
                                        __instance.history != null && __instance.history.GetMessagePin(protoId) == EPin.Show)
                                    {
                                        flag8 = true;
                                    }
                                    
                                    VectorLF3 vectorLF6 = cosmicMessageData.uPosition - camUPos;
                                    double num15 = directionOfView.x * vectorLF6.x + directionOfView.y * vectorLF6.y + directionOfView.z * vectorLF6.z;
                                    
                                    if (!flag8 && cosmicMessageManager.IsMessageVisible(protoId))
                                    {
                                        flag8 = true;
                                        if (__instance.gameData != null && __instance.gameData.mainPlayer != null &&
                                            (cosmicMessageData.uPosition - __instance.gameData.mainPlayer.uPosition).magnitude < 500.0)
                                        {
                                            flag8 = false;
                                        }
                                        if (flag8 && __instance.history != null && __instance.history.GetMessagePin(protoId) == EPin.Hide)
                                        {
                                            flag8 = false;
                                        }
                                        if (!flag8 && num15 > 0.0)
                                        {
                                            num15 /= vectorLF6.magnitude;
                                            if (num15 > 0.999)
                                            {
                                                flag8 = true;
                                            }
                                        }
                                        if (cosmicMessageData.doodadProtoId == 5)
                                        {
                                            flag8 = __instance.uiGame.dfSpaceGuideOnFinal;
                                        }
                                    }
                                    
                                    if (!flag8 && __instance.gameData != null && __instance.gameData.mainPlayer != null && 
                                        __instance.gameData.mainPlayer.navigation != null && 
                                        __instance.gameData.mainPlayer.navigation.indicatorMsgId == protoId && num15 > 0.0)
                                    {
                                        num15 /= vectorLF6.magnitude;
                                        if (num15 > 0.999)
                                        {
                                            flag8 = true;
                                        }
                                    }
                                    
                                    if (__instance.mouseInMessage == protoId)
                                    {
                                        flag8 = true;
                                    }
                                    
                                    if (flag8)
                                    {
                                        flag8 = __instance.CheckVisible(pId0, 0, cosmicMessageData.uPosition, camUPos);
                                    }
                                    
                                    if (flag8)
                                    {
                                        try
                                        {
                                            VectorLF3 vec = Maths.QInvRotateLF(__instance.relRot, cosmicMessageData.uPosition - __instance.relPos);

                                            // --- GS2 Logging START ---
                                            // Determine type first to log it correctly
                                            ESpaceGuideType guideType;
                                            var doodad = LDB.doodads.Select(cosmicMessageData.doodadProtoId);
                                            guideType = (doodad != null && doodad.ID == 5) ?
                                                ESpaceGuideType.DFCommunicator : ESpaceGuideType.CosmicMessage;

                                            if (guideType == ESpaceGuideType.CosmicMessage || guideType == ESpaceGuideType.DFCommunicator)
                                            {
                                                // GS2.Log($"Setting Entry for {guideType} ID:{protoId} | uPos: {cosmicMessageData.uPosition} | RelVec: {vec} | uPosMag: {cosmicMessageData.uPosition.magnitude:E2} | RelVecMag: {vec.magnitude:E2}");
                                            }
                                            // --- GS2 Logging END ---

                                            __instance.SetEntry(ref _guidecnt, guideType, protoId, 0, vec, 1f);
                                        }
                                        catch (System.Exception ex)
                                        {
                                            // GS2.Warn($"Error setting cosmic message entry: {ex.Message}");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // GS2.Warn("Cosmic message manager or messages array is null");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        GS2.Error($"Exception in cosmic message handling: {ex.Message}");
                    }
                }
            }
            //end 0.9.25 update

            // Add safeguards for the final section
            try
            {
                __instance.ClipEntryPool(_guidecnt);
                
                // Check array and length before iterating
                if (__instance.entryPool != null)
                {
                    for (var index5 = 0; index5 < __instance.entryOpenedCount && index5 < __instance.entryPool.Count; index5++)
                    {
                        if (__instance.entryPool[index5] != null)
                        {
                            __instance.entryPool[index5]._LateUpdate();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                GS2.Error($"Error in final section of OnLateUpdate: {ex.Message}");
            }
            return false;
        }

        private static void ProcessHiveStuff(ref UISpaceGuide __instance, ref int _guidecnt)
        {
            if (__instance == null || __instance.gameCamera == null || 
                __instance.sector == null || __instance.sector.dfHives == null ||
                __instance.gameData == null || __instance.gameData.localStar == null)
            {
                GS2.Warn("Cannot process hive stuff - required objects are null");
                return;
            }

            var position = __instance.gameCamera.transform.position;
            var camUPos = __instance.relPos + Maths.QRotateLF(__instance.relRot, position);
            var directionOfView = Maths.QRotateLF(__instance.relRot, __instance.gameCamera.ScreenPointToRay(Input.mousePosition).direction);
            var localStarID = __instance.gameData.localStar == null ? 0 : __instance.gameData.localStar.id * 100;
            var flag2 = VFInput.onGUI || VFInput.onGUIOperate;
            
            // Recheck index bounds in case it changed between checks
            if (__instance.gameData.localStar.index < 0 || 
                __instance.gameData.localStar.index >= __instance.sector.dfHives.Length)
            {
                GS2.Warn($"Local star index {__instance.gameData.localStar.index} is out of bounds for dfHives array ({__instance.sector.dfHives.Length})");
                return;
            }
            
            EnemyDFHiveSystem enemyDFHiveSystem = __instance.sector.dfHives[__instance.gameData.localStar.index];
            int num9 = 0;
            while (enemyDFHiveSystem != null)
            {
                if (!enemyDFHiveSystem.isEmpty)
                {
                    int hiveId = enemyDFHiveSystem.hiveAstroId - 1000000;
                    
                    // Check if hiveId is valid for sector.astros array
                    if (hiveId < 0 || __instance.sector.astros == null || hiveId >= __instance.sector.astros.Length)
                    {
                        GS2.Warn($"Invalid hive ID {hiveId} or sector.astros is null or out of bounds");
                        break;
                    }
                    
                    Vector3 hiveMarkerPos = Vector3.zero;
                    bool hasAnyStructureOrUnit = __instance.uiGame.dfSpaceGuideOnFinal;
                    if (!hasAnyStructureOrUnit && !flag2)
                    {
                        VectorLF3 hiveRelPos = __instance.sector.astros[hiveId].uPos - camUPos;
                            double distanceToHive = directionOfView.x * hiveRelPos.x +
                                                    directionOfView.y * hiveRelPos.y +
                                                    directionOfView.z * hiveRelPos.z;
                            if (distanceToHive > 0.0)
                            {
                                distanceToHive /= hiveRelPos.magnitude;
                                if (distanceToHive > 0.9999)
                                {
                                    hasAnyStructureOrUnit = true;
                                }
                            }
                    }

                    if (__instance.mouseInHive == enemyDFHiveSystem.hiveAstroId)
                    {
                        hasAnyStructureOrUnit = true;
                    }
                    if (!enemyDFHiveSystem.hasAnyStructureOrUnit)
                    {
                        hasAnyStructureOrUnit = false;
                    }
                    if (hasAnyStructureOrUnit)
                    {
                        hiveMarkerPos = Maths.QInvRotateLF(__instance.relRot,
                            __instance.sector.astros[hiveId].uPos - __instance.relPos);
                        if ((double)hiveMarkerPos.magnitude >
                            (double)(__instance.gameData.localStar.systemRadius * 6f) * 40000.0)
                        {
                            hasAnyStructureOrUnit = false;
                        }
                    }

                    if (hasAnyStructureOrUnit)
                    {
                        hasAnyStructureOrUnit = __instance.CheckVisible(localStarID, hiveId, __instance.sector.astros[hiveId].uPos, camUPos);
                    }

                    if (hasAnyStructureOrUnit)
                    {
                        __instance.SetEntry(ref _guidecnt, ESpaceGuideType.DFHive, enemyDFHiveSystem.hiveAstroId, 0, hiveMarkerPos, 0f);
                    }

                    num9++;
                }

                enemyDFHiveSystem = enemyDFHiveSystem.nextSibling;
            }
        }
    }
}