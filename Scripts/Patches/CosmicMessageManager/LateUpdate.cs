// /*
//  * CHANGE LOG:
//  * 2025-03-08: Initial implementation
//  * - Added early exit if arrays aren't ready
//  * 2025-03-08: Updated patch
//  * - Added check for messageData being null in simulators
//  * 2025-03-08: Added detailed logging
//  * - Added logging at each step to track null references
//  * 2025-03-08: Fixed message handling
//  * - Let game handle first 100 messages normally
//  * - Only apply special handling for messages beyond index 100
//  * 
//  * PATCH EXPLANATION:
//  * This patch adds safety checks to CosmicMessageManager.LateUpdate()
//  * to prevent NullReferenceException by:
//  * 1. Skipping the update if the arrays aren't ready
//  * 2. Letting game handle first 100 messages normally
//  * 3. Applying special handling only for extended messages (>100)
//  * 
//  * The original method assumes arrays are initialized and messageData is set,
//  * which may not be true during GalacticScale's extended initialization.
//  * 
//  * Changes from original:
//  * 1. Adds early exit if messages array is null
//  * 2. Preserves original functionality for first 100 messages
//  * 3. Adds null checks and initialization for extended messages
//  */
//
// using HarmonyLib;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace GalacticScale
// {
//     public partial class PatchOnCosmicMessageManager
//     {
//         [HarmonyPrefix]
//         [HarmonyPatch(typeof(CosmicMessageManager), "LateUpdate")]
//         public static bool LateUpdate(CosmicMessageManager __instance)
//         {
//             GS2.Warn("LateUpdate Start");
//             
//             // Let original method handle null instance/gameData checks
//             if (__instance == null)
//             {
//                 GS2.Warn("__instance is null");
//                 return true;
//             }
//             
//             if (__instance.gameData == null)
//             {
//                 GS2.Warn("__instance.gameData is null");
//                 return true;
//             }
//
//             // Skip update if arrays aren't ready
//             if (__instance.messages == null)
//             {
//                 GS2.Warn("__instance.messages is null");
//                 return false;
//             }
//             
//             if (__instance.messageSimulators == null)
//             {
//                 GS2.Warn("__instance.messageSimulators is null");
//                 return false;
//             }
//
//             GS2.Warn($"Arrays initialized - messages.Length: {__instance.messages.Length}, messageSimulators.Length: {__instance.messageSimulators.Length}");
//
//             // Check each simulator's messageData, but only for extended messages (>100)
//             for (int i = 100; i < __instance.messageSimulators.Length; i++)
//             {
//                 var simulator = __instance.messageSimulators[i];
//                 if (simulator != null && simulator.messageData == null)
//                 {
//                     GS2.Warn($"Simulator {i} has null messageData");
//                     // Try to re-initialize from messages array if possible
//                     if (i < __instance.messages.Length && __instance.messages[i] != null)
//                     {
//                         GS2.Warn($"Re-initializing simulator {i}");
//                         simulator.Init(__instance.messages[i]);
//                     }
//                 }
//             }
//
//             if (GameMain.mainPlayer == null)
//             {
//                 GS2.Warn("GameMain.mainPlayer is null");
//                 return false;
//             }
//
//             VectorLF3 uPosition = GameMain.mainPlayer.uPosition;
//             GS2.Warn($"Player position: {uPosition}");
//
//             double num = 160000000000.0;
//             double num2 = 163216000000.0;
//
//             // Process messages - let game handle first 100 normally
//             for (int i = 1; i < __instance.messages.Length; i++)
//             {
//                 // Skip processing if this is a base game message
//                 if (i <= 100)
//                     continue;
//
//                 if (__instance.messages[i] == null)
//                 {
//                     GS2.Warn($"Message {i} is null");
//                     continue;
//                 }
//
//                 if (__instance.messages[i].protoId != i)
//                 {
//                     GS2.Warn($"Message {i} has mismatched protoId: {__instance.messages[i].protoId}");
//                     continue;
//                 }
//
//                 CosmicMessageData cosmicMessageData = __instance.messages[i];
//                 var proto = LDB.cosmicMessages.Select(__instance.messages[i].protoId);
//                 if (proto == null)
//                 {
//                     GS2.Warn($"No cosmic message proto found for id {__instance.messages[i].protoId}");
//                     continue;
//                 }
//
//                 double sqrMagnitude = (uPosition - __instance.messages[i].uPosition).sqrMagnitude;
//                 GS2.Warn($"Message {i} distance: {sqrMagnitude}");
//
//                 if (sqrMagnitude <= num)
//                 {
//                     if (__instance.messageSimulators[i] == null)
//                     {
//                         DoodadProto doodadProto = LDB.doodads.Select(__instance.messages[i].doodadProtoId);
//                         if (doodadProto == null)
//                         {
//                             GS2.Warn($"No doodad proto found for id {__instance.messages[i].doodadProtoId}");
//                             continue;
//                         }
//
//                         if (__instance.messageRoot == null)
//                         {
//                             GS2.Warn("messageRoot is null");
//                             continue;
//                         }
//
//                         var prefab = Resources.Load<GameObject>(doodadProto.PrefabPath);
//                         if (prefab == null)
//                         {
//                             GS2.Warn($"Failed to load prefab from path: {doodadProto.PrefabPath}");
//                             continue;
//                         }
//
//                         GS2.Warn($"Creating new simulator for message {i}");
//                         GameObject gameObject = Object.Instantiate(prefab, __instance.messageRoot.transform);
//                         gameObject.name = string.Format("Cosmic Message {0}", doodadProto.ID);
//                         CosmicMessageSimulator component = gameObject.GetComponent<CosmicMessageSimulator>();
//                         __instance.gameData.history.RegFeatureKey(3500000 + __instance.messages[i].protoId);
//                         component.Init(__instance.messages[i]);
//                         __instance.messageSimulators[i] = component;
//                     }
//                 }
//                 else if (sqrMagnitude > num2 && __instance.messageSimulators[i] != null)
//                 {
//                     GS2.Warn($"Destroying simulator {i} due to distance");
//                     Object.Destroy(__instance.messageSimulators[i].gameObject);
//                     __instance.messageSimulators[i] = null;
//                 }
//
//                 if (__instance.messageSimulators[i] != null)
//                 {
//                     GS2.Warn($"Updating simulator {i}");
//                     __instance.messageSimulators[i].OnLateUpdate();
//                 }
//             }
//
//             GS2.Warn("LateUpdate Complete");
//             return true; // Let original method handle first 100 messages
//         }
//     }
// }