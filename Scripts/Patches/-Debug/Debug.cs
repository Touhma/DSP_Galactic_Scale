using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;
using static GalacticScale.GS3;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;
using Logger = BepInEx.Logging.Logger;

namespace GalacticScale.Patches

{
    public class PatchOnUnspecified_Debug
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerAction_Inspect),nameof(PlayerAction_Inspect.GetObjectSelectDistance))]
        public static void GetObjectSelectDistance(ref PlayerAction_Inspect __instance, ref float __result, EObjectType objType, int objid)
        {
            if (objid == 0)
            {
                return;
            }
            if (__instance.player.factory == null)
            {
                return;
            }

            if (objType != EObjectType.Entity) return;
            var id = __instance.player.factory.entityPool[objid].protoId;
            if (id == 2107 || id == 2103 || id == 2104) __result = 2000f;
            if (id == 2105) __result = 15000f;
            if (__result == 35f) __result = 50f;
        }

	    
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefenseSystem), nameof(DefenseSystem.AfterTurretsImport))]
        private static void AfterTurretsImport(ref DefenseSystem __instance)
        {
            int cursor = __instance.turrets.cursor;
            TurretComponent[] buffer = __instance.turrets.buffer;
            for (int i = 1; i < cursor; i++)
            {
                ref TurretComponent ptr = ref buffer[i];
                if (ptr.id == 1) TurretComponentTranspiler.AddTurret(__instance, ref ptr);
			    
            }
        }
    }
}