//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using HarmonyLib;
//using UnityEngine;

//namespace GalacticScale.Scripts.PatchPlanetSize
//{
//    [HarmonyPatch(typeof(PlayerAction_Build))]
//    public class PatchOnPlayerAction_Build
//    {

//        //Strategy: 1) Add scale factor to gas giant calls
//        // There are two flavors of this currently, one for floats and one for Vector3s:
//        //
//        // Float:
//        /* 0x0006E1D7 113C         */// IL_0A8F: ldloc.s   V_60
//        /* 0x0006E1D9 6FD4090006   */// IL_0A91: callvirt  instance float32 PlanetData::get_realRadius()
//        /* 0x0006E1DE 22CDCCCC3C   */// IL_0A96: ldc.r4    0.025
//        /* 0x0006E1E3 5A           */// IL_0A9B: mul
//        //
//        // Vector3:
//        /* 0x0006EE4D 02           */// IL_1705: ldarg.0
//        /* 0x0006EE4E 7BD3070004   */// IL_1706: ldfld class Player PlayerAction::player
//        /* 0x0006EE53 6F98070006   */// IL_170B: callvirt instance class PlanetData Player::get_planetData()
//        /* 0x0006EE58 6FD4090006   */// IL_1710: callvirt  instance float32 PlanetData::get_realRadius()
//        /* 0x0006EE5D 286600000A   */// IL_1715: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
//        /* 0x0006EE62 22CDCCCC3C   */// IL_171A: ldc.r4    0.025
//        /* 0x0006EE67 286600000A   */// IL_171F: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
//        //
//        // 1) Add scale factor to gas giant calls
//        // find all calls to ldc.r4 with operand 0.025
//        // if the prior instruction is callvirt float32 PlanetData::get_realRadius() and the next call is mul
//        //   loop BACKWARD until we reach ldloc.s (sanity check: limit to 10 prior instructions)
//        //     create a copy of each instruction FROM the ldloc.s instruction TO (but not including) the get_realRadius instruction (this gets us the same PlanetData instance)
//        //     create a new instruction to call the GetScaleFactored() function on the same planetData instance
//        //     create a new div instruction to divide the data already on the stack by the result of GetScaleFactored()
//        //     insert those instructions at i+2 (where i is still the location where we found ldc.r4 0.025), so that the result of the mul is the thing being divided
//        // else if the prior instruction is calling Vector3.Multiply and the further-prior instruction is callvirt float32 PlanetData::get_realRadius()
//        //   loop BACKWARD until we reach ldarg.0 (sanity check: limit to 10 prior instructions)
//        //     create a copy of each instruction FROM ldarg.0 TO (but not including) the get_realRadius instruction (this gets us the same PlanetData instance)
//        //     create a new instruction to call the GetScaleFactored() function on the same planetData instance
//        //     create a new call instruction to call Vector3.Divide, dividing the data already on the stack by the result of GetScaleFactored()
//        //     insert those instructions at i+2 (where i is still the location where we found ldc.r4 0.025), so that the result of the prior vector multiplication is what we vector divide
//        [HarmonyTranspiler]
//        [HarmonyPatch("CheckBuildConditions")]
//        public static IEnumerable<CodeInstruction> CheckBuildConditionsTranspiler(IEnumerable<CodeInstruction> instructions)
//        {
//            var codes = new List<CodeInstruction>(instructions);
//            for (var i = 0; i < codes.Count; i++)
//                if (i > 0 && codes[i].Is(OpCodes.Ldc_R4, 0.025f) && i < codes.Count - 1)
//                // This condition is to add scale factor to gas giant calls. First we check for the existing static scale factor of 0.025f
//                // We check late and stop early because we operate with info all AROUND the current line when we do anything
//                {
//                    if (codes[i - 1].Calls(typeof(PlanetData).GetProperty("realRadius").GetGetMethod()) && codes[i + 1].opcode == OpCodes.Mul)
//                    // Check if the prior instruction is get_realRadius() and the next call is mul
//                    {
//                        var newInstructions = new List<CodeInstruction>();
//                        for (var j = i - 2; j > 0 && j > i - 11; j--)
//                            //loop BACKWARD until we reach ldloc.s (sanity check: limit to 10 prior instructions)
//                            if (codes[j].opcode == OpCodes.Ldloc_S)
//                            {
//                                for (; j < i - 1; j++)
//                                    //create a copy of each instruction FROM the ldloc.s instruction TO (but not including) the get_realRadius instruction (this gets us the same PlanetData instance)
//                                    newInstructions.Add(new CodeInstruction(codes[j]));
//                                break;
//                            }
//                        if (newInstructions.Count != 0)
//                        //If we didn't find ldloc.s, don't do anything with this instance as it is not a recognized pattern.
//                        {
//                            //create a new instruction to call the GetScaleFactored() function on the same planetData instance
//                            newInstructions.Add(new CodeInstruction(OpCodes.Callvirt, typeof(PlanetDataExtension).GetMethod("GetScaleFactored")));

//                            //create a new div instruction to divide the data already on the stack by the result of GetScaleFactored()
//                            newInstructions.Add(new CodeInstruction(OpCodes.Div));

//                            //insert those instructions at i+2 (where i is still the location where we found ldc.r4 0.025), so that the result of the mul is the thing being divided
//                            codes.InsertRange(i + 2, newInstructions);
//                        }
//                    }
//                    else if (codes[i - 1].Calls(typeof(Vector3).GetMethod("op_Multiply", new[] { typeof(Vector3), typeof(float) })) && codes[i - 2].Calls(typeof(PlanetData).GetProperty("realRadius").GetGetMethod()))
//                    // check if the prior instruction is calling Vector3.Multiply and the further-prior instruction is callvirt float32 PlanetData::get_realRadius()
//                    {
//                        var newInstructions = new List<CodeInstruction>();
//                        for (var j = i - 3; j > 0 && j > i - 12; j--)
//                            //loop BACKWARD until we reach ldarg.0 (sanity check: limit to 10 prior instructions)
//                            if (codes[j].opcode == OpCodes.Ldarg_0)
//                            {
//                                for (; j < i - 2; j++)
//                                    //create a copy of each instruction FROM ldarg.0 TO (but not including) the get_realRadius instruction (this gets us the same PlanetData instance)
//                                    newInstructions.Add(new CodeInstruction(codes[j]));
//                                break;
//                            }
//                        if (newInstructions.Count != 0)
//                        //If we didn't find ldarg.0, don't do anything with this instance as it is not a recognized pattern.
//                        {
//                            //create a new instruction to call the GetScaleFactored() function on the same planetData instance
//                            newInstructions.Add(new CodeInstruction(OpCodes.Callvirt, typeof(PlanetDataExtension).GetMethod("GetScaleFactored")));

//                            //create a new call instruction to call Vector3.Divide, dividing the data already on the stack by the result of GetScaleFactored()
//                            newInstructions.Add(new CodeInstruction(OpCodes.Call, typeof(Vector3).GetMethod("op_Division")));

//                            //insert those instructions at i+2 (where i is still the location where we found ldc.r4 0.025), so that the result of the prior vector multiplication is what we vector divide
//                            codes.InsertRange(i + 2, newInstructions);
//                        }

//                    }

//                }
//            return codes.AsEnumerable();
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch("CheckBuildConditions")]
//        static bool CheckBuildConditions(bool __result,
//            PlayerAction_Build __instance, ref string ___cursorText,
//            ref bool ___cursorWarning, ref bool ___cursorValid,
//            ref bool ___waitConfirm, ref int[] ____tmp_ids,
//            ref NearColliderLogic ___nearcdLogic,
//            ref PlanetFactory ___factory,
//            Pose ___previewPose
//            )
//        {
//            int count = __instance.buildPreviews.Count;
//            if (count < 2) return __result; // Check we are building
//            BuildPreview preview = __instance.buildPreviews[0];
//            int objId = preview.inputObjId;
//            if (objId < 0 || objId >= ___factory.entityPool.Length) return __result; // Sanity Check
//            EntityData entity = ___factory.entityPool[objId];
//            if (entity.isNull) return __result;
//            ItemProto itemProto = LDB.items.Select((int)entity.protoId); // Grab the prototype of the first object in the chain
//            if (itemProto == null) return __result;
//            if (itemProto.prefabDesc == null) return __result;
//            if (itemProto.prefabDesc.oilMiner) // Check that we are connected to an oil miner
//            {
//                if (preview.condition == EBuildCondition.JointCannotLift) // Make sure the error is that the endpoint must be horizontal
//                {
//                    preview.condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
//                    for (int i = 0; i < count; i++) // Check the rest of the belt for errors
//                    {
//                        if ((__instance.buildPreviews[i].condition != EBuildCondition.Ok && __instance.buildPreviews[i].condition != EBuildCondition.JointCannotLift))
//                        {
//                            __result = (bool)false;
//                            return __result; //If there's some other problem with the belt, bail out.
//                        }
//                    }
//                    ___cursorText = "Click to build";
//                    ___cursorWarning = false; // Prevent red text
//                    __result = true; // Override the build condition check
//                    UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
//                }
//            }

//            return __result;
//        }


//        //Strategy: 1) Add scale factor to gas giant call
//        // This is the Vector3 flavor, identical instructions to the above
//        //
//        // Vector3:
//        /* 0x0006EE4D 02           */// IL_1705: ldarg.0
//        /* 0x0006EE4E 7BD3070004   */// IL_1706: ldfld class Player PlayerAction::player
//        /* 0x0006EE53 6F98070006   */// IL_170B: callvirt instance class PlanetData Player::get_planetData()
//        /* 0x0006EE58 6FD4090006   */// IL_1710: callvirt  instance float32 PlanetData::get_realRadius()
//        /* 0x0006EE5D 286600000A   */// IL_1715: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
//        /* 0x0006EE62 22CDCCCC3C   */// IL_171A: ldc.r4    0.025
//        /* 0x0006EE67 286600000A   */// IL_171F: call      valuetype [UnityEngine.CoreModule]UnityEngine.Vector3 [UnityEngine.CoreModule]UnityEngine.Vector3::op_Multiply(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, float32)
//                                     //
//                                     // 1) Add scale factor to gas giant calls
//                                     // find all calls to ldc.r4 with operand 0.025
//                                     // if the prior instruction is calling Vector3.Multiply and the further-prior instruction is callvirt float32 PlanetData::get_realRadius()
//                                     //   loop BACKWARD until we reach ldarg.0 (sanity check: limit to 10 prior instructions)
//                                     //     create a copy of each instruction FROM ldarg.0 TO (but not including) the get_realRadius instruction (this gets us the same PlanetData instance)
//                                     //     create a new instruction to call the GetScaleFactored() function on the same planetData instance
//                                     //     create a new call instruction to call Vector3.Divide, dividing the data already on the stack by the result of GetScaleFactored()
//                                     //     insert those instructions at i+2 (where i is still the location where we found ldc.r4 0.025), so that the result of the prior vector multiplication is what we vector divide
//        [HarmonyTranspiler]
//        [HarmonyPatch("DetermineBuildPreviews")]
//        public static IEnumerable<CodeInstruction> DetermineBuildPreviewsTranspiler(IEnumerable<CodeInstruction> instructions)
//        {
//            var codes = new List<CodeInstruction>(instructions);
//            for (var i = 0; i < codes.Count; i++)
//                if (i > 0 && codes[i].Is(OpCodes.Ldc_R4, 0.025f) && i < codes.Count - 1)
//                    // This condition Add scale factor to gas giant calls. First we check for the existing static scale factor of 0.025f
//                    // We check late and stop early because we operate with info all AROUND the current line when we do anything
//                    if (codes[i - 1].Calls(typeof(Vector3).GetMethod("op_Multiply", new[] { typeof(Vector3), typeof(float) })) && codes[i - 2].Calls(typeof(PlanetData).GetProperty("realRadius").GetGetMethod()))
//                    // check if the prior instruction is calling Vector3.Multiply and the further-prior instruction is callvirt float32 PlanetData::get_realRadius()
//                    {
//                        var newInstructions = new List<CodeInstruction>();
//                        for (var j = i - 3; j > 0 && j > i - 12; j--)
//                            //loop BACKWARD until we reach ldarg.0 (sanity check: limit to 10 prior instructions)
//                            if (codes[j].opcode == OpCodes.Ldarg_0)
//                            {
//                                for (; j < i - 2; j++)
//                                    //create a copy of each instruction FROM ldarg.0 TO (but not including) the get_realRadius instruction (this gets us the same PlanetData instance)
//                                    newInstructions.Add(new CodeInstruction(codes[j]));
//                                break;
//                            }
//                        if (newInstructions.Count != 0)
//                        //If we didn't find ldarg.0, don't do anything with this instance as it is not a recognized pattern.
//                        {
//                            //create a new instruction to call the GetScaleFactored() function on the same planetData instance
//                            newInstructions.Add(new CodeInstruction(OpCodes.Callvirt, typeof(PlanetDataExtension).GetMethod("GetScaleFactored")));

//                            //create a new call instruction to call Vector3.Divide, dividing the data already on the stack by the result of GetScaleFactored()
//                            newInstructions.Add(new CodeInstruction(OpCodes.Call, typeof(Vector3).GetMethod("op_Division")));

//                            //insert those instructions at i+2 (where i is still the location where we found ldc.r4 0.025), so that the result of the prior vector multiplication is what we vector divide
//                            codes.InsertRange(i + 2, newInstructions);
//                        }

//                    }
//            return codes.AsEnumerable();
//        }
//    }
//}

