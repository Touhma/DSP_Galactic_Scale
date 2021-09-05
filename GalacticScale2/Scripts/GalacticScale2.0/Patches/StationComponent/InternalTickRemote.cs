using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnStationComponent
    {
        // Strategy: find where  replace all ldc.i4.s 10 instructions with dynamic references to the relevant star's planetCount
        //
        // GameMain.galaxy.PlanetById(int) returns null if not a planet, otherwise PlanetData
        /* 0x0002D43D 7E05130004   */ // IL_000D: callVirt    class GalaxyData GameMain::get_galaxy
        // IL_xxxx: See below for how we find the right planet ID
        // IL_xxxx: callVirt class PlanetData GalaxyData::PlanetById(int)
        //
        // From there, PlanetData.star.planetCount gets what we need.
        //
        // Finding the planetID:
        // First, the C# code at this time:
        //   int num43 = shipData.planetA / 100 * 100; //this is later passed to astroPoses[i] and it basically represents a planet ID.
        //   int num44 = shipData.planetB / 100 * 100; //this is later passed to astroPoses[i] and it basically represents a planet ID.
        //	 for (int k = num43; k<num43 + 10; k++) {...}
        //   if (num44 != num43) {
        //     for (int l = num44; l<num44 + 10; l++) {...}}
        //
        // Note that "basically represents" is important - astroPoses is a zero-indexed array, but IDs start at 1.
        //
        // For loops are kind of backwards in CIL (compared to C# anyhow). Here's original IL for the planet IDs and prep...
        /* 0x0002EFFD 1221         */ // IL_1BCD: ldloca.s V_33                  // shipData.
        /* 0x0002EFFF 7B28050004   */ // IL_1BCF: ldfld int32 ShipData::planetA  // load planetA ID from prior
        /* 0x0002F004 1F64         */ // IL_1BD4: ldc.i4.s  100                  // load 100
        /* 0x0002F006 5B           */ // IL_1BD6: div                            // divide planetA ID by 100
        /* 0x0002F007 1F64         */ // IL_1BD7: ldc.i4.s  100                  // load 100
        /* 0x0002F009 5A           */ // IL_1BD9: mul                            // multiply result by 100
        /* 0x0002F00A 1340         */ // IL_1BDA: stloc.s V_64                   // store into a variable (!)
        /* 0x0002F00C 1221         */ // IL_1BDC: ldloca.s V_33                  // shipData. again
        /* 0x0002F00E 7B29050004   */ // IL_1BDE: ldfld int32 ShipData::planetB  // load planetB ID from prior
        /* 0x0002F013 1F64         */ // IL_1BE3: ldc.i4.s  100                  // load 100
        /* 0x0002F015 5B           */ // IL_1BE5: div                            // divide planetB ID by 100
        /* 0x0002F016 1F64         */ // IL_1BE6: ldc.i4.s  100                  // load 100
        /* 0x0002F018 5A           */ // IL_1BE8: mul                            // multiply result by 100
        /* 0x0002F019 1341         */ // IL_1BE9: stloc.s V_65                   // store into a different variable (!!)
        /* 0x0002F01B 1140         */
        // IL_1BEB: ldloc.s V_64                   // loop prep - load the planetA ID stored prior
        /* 0x0002F01D 1342         */
        // IL_1BED: stloc.s V_66                   // loop prep - save that ID into a new temp variable (i)
        /* 0x0002F01F 380E010000   */
        // IL_1BEF: br IL_1D02                     //skip to the for loop's limit checking line
        // ...                                     //skipping the code inside the loop
        /* 0x0002F132 1142         */ // IL_1D02: ldloc.s   V_66                 // load in the loop variable
        /* 0x0002F134 1140         */ // IL_1D04: ldloc.s   V_64                 // load planet A's ID
        /* 0x0002F136 1F0A         */ // IL_1D06: ldc.i4.s  10                   // load the value 10
        /* 0x0002F138 58           */
        // IL_1D08: add                            // add the last two loads - planet A's ID + 10
        /* 0x0002F139 3FE6FEFFFF   */
        // IL_1D09: blt       IL_1BF4              // skip back to the actual loop code if the result is less than the loop variable loaded before it
        //
        // Unfortunately we can't guarantee that V_64 and V_65 are consistently going to be the variables we need, especially between patches and with other mods.
        // But, thankfully, we know that the line immediately before ldc.i4.s 10 is the variable we want to refer to.
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Ldc_I4_S && codes[i].OperandIs(10))
                {
                    var newInstructions = new List<CodeInstruction>();
                    newInstructions.Add(new CodeInstruction(codes[i - 1])); //The line before adding 10 is the line which loads in the planet ID we care about, so copy it
                    newInstructions.Add(Transpilers.EmitDelegate<Del>(bodyID =>
                        // We add 1 to the body ID because it was originally an index in the astroPoses array but we need the actual ID of it.
                        // We add 1 to the planet count because the loop is <, not <=
                        GameMain.galaxy.PlanetById(bodyID + 1).star.planetCount + 1));
                    codes.RemoveAt(i); // Remove the original loading of 10
                    codes.InsertRange(i, newInstructions); //Instead, load the count of planets around the target star (plus one)
                }

            return codes.AsEnumerable();
        }

        private delegate int Del(int bodyID);
        //       [HarmonyPrefix, HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        //       public static bool InternalTickRemote( ref StationComponent __instance,
        //   int timeGene,
        //   double dt,
        //   float shipSailSpeed,
        //   float shipWarpSpeed,
        //   int shipCarries,
        //   StationComponent[] gStationPool,
        //   AstroPose[] astroPoses,
        //   VectorLF3 relativePos,
        //   Quaternion relativeRot,
        //   bool starmap,
        //   int[] consumeRegister)
        // {
        //   bool canWarp = (double) shipWarpSpeed > (double) shipSailSpeed + 1.0;
        //   __instance.warperFree = DSPGame.IsMenuDemo;
        //   if (__instance.warperCount < __instance.warperMaxCount)
        //   {
        //     lock (__instance.storage)
        //     {
        //       for (int index = 0; index < __instance.storage.Length; ++index)
        //       {
        //         if (__instance.storage[index].itemId == 1210 && __instance.storage[index].count > 0)
        //         {
        //           ++__instance.warperCount;
        //           --__instance.storage[index].count;
        //           break;
        //         }
        //       }
        //     }
        //   }
        //   int num1 = 0;
        //   int num2 = 0;
        //   int num3 = 0;
        //   int num4 = 0;
        //   int num5 = 0;
        //   int num6 = 0;
        //   int num7 = 0;
        //   int num8 = 0;
        //   int num9 = 0;
        //   int num10 = 0;
        //   int num11 = 0;
        //   int num12 = 0;
        //   int num13 = 0;
        //   int num14 = 0;
        //   if (timeGene == __instance.gene)
        //   {
        //     ++__instance._tmp_iter_remote;
        //     if (__instance.remotePairCount > 0 && __instance.idleShipCount > 0)
        //     {
        //       __instance.remotePairProcess %= __instance.remotePairCount;
        //       int remotePairProcess1 = __instance.remotePairProcess;
        //       do
        //       {
        //         int num15 = (shipCarries - 1) * __instance.deliveryShips / 100;
        //         SupplyDemandPair remotePair1 = __instance.remotePairs[__instance.remotePairProcess];
        //         if (remotePair1.supplyId == __instance.gid)
        //         {
        //           lock (__instance.storage)
        //           {
        //             num1 = __instance.storage[remotePair1.supplyIndex].max;
        //             num2 = __instance.storage[remotePair1.supplyIndex].count;
        //             num3 = __instance.storage[remotePair1.supplyIndex].remoteSupplyCount;
        //             num4 = __instance.storage[remotePair1.supplyIndex].totalSupplyCount;
        //             num5 = __instance.storage[remotePair1.supplyIndex].itemId;
        //           }
        //         }
        //         if (remotePair1.supplyId == __instance.gid && num1 <= num15)
        //           num15 = num1 - 1;
        //         if (num15 < 0)
        //           num15 = 0;
        //         if (remotePair1.supplyId == __instance.gid && num2 > num15 && (num3 > num15 && num4 > num15))
        //         {
        //           StationComponent stationComponent = gStationPool[remotePair1.demandId];
        //           if (stationComponent != null)
        //           {
        //             double trip = (astroPoses[__instance.planetId].uPos - astroPoses[stationComponent.planetId].uPos).magnitude + (double) astroPoses[__instance.planetId].uRadius + (double) astroPoses[stationComponent.planetId].uRadius;
        //             bool flag1 = trip < __instance.tripRangeShips;
        //             bool flag2 = trip >= __instance.warpEnableDist;
        //             if (__instance.warperNecessary & flag2 && (__instance.warperCount < 2 || !canWarp))
        //               flag1 = false;
        //             if (flag1)
        //             {
        //               lock (stationComponent.storage)
        //               {
        //                 num9 = stationComponent.storage[remotePair1.demandIndex].remoteDemandCount;
        //                 num10 = stationComponent.storage[remotePair1.demandIndex].totalDemandCount;
        //               }
        //             }
        //             if (flag1 && num9 > 0 && num10 > 0)
        //             {
        //               long num16 = __instance.CalcTripEnergyCost(trip, shipSailSpeed, canWarp);
        //               if (__instance.energy >= num16)
        //               {
        //                 int num17 = shipCarries < num2 ? shipCarries : num2;
        //                 int index = __instance.QueryIdleShip(__instance.nextShipIndex);
        //                 if (index >= 0)
        //                 {
        //                   __instance.nextShipIndex = (index + 1) % __instance.workShipDatas.Length;
        //                   __instance.workShipDatas[__instance.workShipCount].stage = -2;
        //                   __instance.workShipDatas[__instance.workShipCount].planetA = __instance.planetId;
        //                   __instance.workShipDatas[__instance.workShipCount].planetB = stationComponent.planetId;
        //                   __instance.workShipDatas[__instance.workShipCount].otherGId = stationComponent.gid;
        //                   __instance.workShipDatas[__instance.workShipCount].direction = 1;
        //                   __instance.workShipDatas[__instance.workShipCount].t = 0.0f;
        //                   __instance.workShipDatas[__instance.workShipCount].itemId = __instance.workShipOrders[__instance.workShipCount].itemId = num5;
        //                   __instance.workShipDatas[__instance.workShipCount].itemCount = num17;
        //                   __instance.workShipDatas[__instance.workShipCount].gene = __instance._tmp_iter_remote;
        //                   __instance.workShipDatas[__instance.workShipCount].shipIndex = index;
        //                   __instance.workShipOrders[__instance.workShipCount].otherStationGId = stationComponent.gid;
        //                   __instance.workShipOrders[__instance.workShipCount].thisIndex = remotePair1.supplyIndex;
        //                   __instance.workShipOrders[__instance.workShipCount].otherIndex = remotePair1.demandIndex;
        //                   __instance.workShipOrders[__instance.workShipCount].thisOrdered = 0;
        //                   __instance.workShipOrders[__instance.workShipCount].otherOrdered = num17;
        //                   if (flag2)
        //                   {
        //                     lock (consumeRegister)
        //                     {
        //                       if (__instance.warperCount >= 2)
        //                       {
        //                         __instance.workShipDatas[__instance.workShipCount].warperCnt += 2;
        //                         __instance.warperCount -= 2;
        //                         consumeRegister[1210] += 2;
        //                       }
        //                       else if (__instance.warperCount >= 1)
        //                       {
        //                         ++__instance.workShipDatas[__instance.workShipCount].warperCnt;
        //                         --__instance.warperCount;
        //                         ++consumeRegister[1210];
        //                       }
        //                       else if (__instance.warperFree)
        //                         __instance.workShipDatas[__instance.workShipCount].warperCnt += 2;
        //                     }
        //                   }
        //                   lock (stationComponent.storage)
        //                     stationComponent.storage[remotePair1.demandIndex].remoteOrder += num17;
        //                   ++__instance.workShipCount;
        //                   --__instance.idleShipCount;
        //                   __instance.IdleShipGetToWork(index);
        //                   lock (__instance.storage)
        //                     __instance.storage[remotePair1.supplyIndex].count -= num17;
        //                   __instance.energy -= num16;
        //                   break;
        //                 }
        //                 break;
        //               }
        //               break;
        //             }
        //           }
        //         }
        //         if (remotePair1.demandId == __instance.gid)
        //         {
        //           lock (__instance.storage)
        //           {
        //             num6 = __instance.storage[remotePair1.demandIndex].remoteDemandCount;
        //             num7 = __instance.storage[remotePair1.demandIndex].totalDemandCount;
        //           }
        //         }
        //         if (remotePair1.demandId == __instance.gid && num6 > 0 && num7 > 0)
        //         {
        //           StationComponent stationComponent = gStationPool[remotePair1.supplyId];
        //           if (stationComponent != null)
        //           {
        //             double trip = (astroPoses[__instance.planetId].uPos - astroPoses[stationComponent.planetId].uPos).magnitude + (double) astroPoses[__instance.planetId].uRadius + (double) astroPoses[stationComponent.planetId].uRadius;
        //             bool flag1 = trip < __instance.tripRangeShips;
        //             if (flag1 && !__instance.includeOrbitCollector && stationComponent.isCollector)
        //               flag1 = false;
        //             bool flag2 = trip >= __instance.warpEnableDist;
        //             if (__instance.warperNecessary & flag2 && (__instance.warperCount < 2 || !canWarp))
        //               flag1 = false;
        //             lock (stationComponent.storage)
        //             {
        //               num11 = stationComponent.storage[remotePair1.supplyIndex].max;
        //               num12 = stationComponent.storage[remotePair1.supplyIndex].count;
        //               num13 = stationComponent.storage[remotePair1.supplyIndex].remoteSupplyCount;
        //               num14 = stationComponent.storage[remotePair1.supplyIndex].totalSupplyCount;
        //             }
        //             if (num11 <= num15)
        //               num15 = num11 - 1;
        //             if (num15 < 0)
        //               num15 = 0;
        //             if (flag1 && num12 > num15 && (num13 > num15 && num14 > num15))
        //             {
        //               long num16 = __instance.CalcTripEnergyCost(trip, shipSailSpeed, canWarp);
        //               if (!stationComponent.isCollector)
        //               {
        //                 bool flag3 = false;
        //                 __instance.remotePairProcess %= __instance.remotePairCount;
        //                 int num17 = __instance.remotePairProcess + 1;
        //                 int remotePairProcess2 = __instance.remotePairProcess;
        //                 int index1 = num17 % __instance.remotePairCount;
        //                 do
        //                 {
        //                   SupplyDemandPair remotePair2 = __instance.remotePairs[index1];
        //                   if (remotePair2.supplyId == __instance.gid && remotePair2.demandId == stationComponent.gid)
        //                   {
        //                     lock (__instance.storage)
        //                     {
        //                       num2 = __instance.storage[remotePair2.supplyIndex].count;
        //                       num3 = __instance.storage[remotePair2.supplyIndex].remoteSupplyCount;
        //                       num4 = __instance.storage[remotePair2.supplyIndex].totalSupplyCount;
        //                       num5 = __instance.storage[remotePair2.supplyIndex].itemId;
        //                     }
        //                   }
        //                   if (remotePair2.supplyId == __instance.gid && remotePair2.demandId == stationComponent.gid)
        //                   {
        //                     lock (stationComponent.storage)
        //                     {
        //                       num9 = stationComponent.storage[remotePair2.demandIndex].remoteDemandCount;
        //                       num10 = stationComponent.storage[remotePair2.demandIndex].totalDemandCount;
        //                     }
        //                   }
        //                   if (remotePair2.supplyId == __instance.gid && remotePair2.demandId == stationComponent.gid && (num2 >= num15 && num3 >= num15) && (num4 >= num15 && num9 > 0 && num10 > 0))
        //                   {
        //                     if (__instance.energy >= num16)
        //                     {
        //                       int num18 = shipCarries < num2 ? shipCarries : num2;
        //                       int index2 = __instance.QueryIdleShip(__instance.nextShipIndex);
        //                       if (index2 >= 0)
        //                       {
        //                         __instance.nextShipIndex = (index2 + 1) % __instance.workShipDatas.Length;
        //                         __instance.workShipDatas[__instance.workShipCount].stage = -2;
        //                         __instance.workShipDatas[__instance.workShipCount].planetA = __instance.planetId;
        //                         __instance.workShipDatas[__instance.workShipCount].planetB = stationComponent.planetId;
        //                         __instance.workShipDatas[__instance.workShipCount].otherGId = stationComponent.gid;
        //                         __instance.workShipDatas[__instance.workShipCount].direction = 1;
        //                         __instance.workShipDatas[__instance.workShipCount].t = 0.0f;
        //                         __instance.workShipDatas[__instance.workShipCount].itemId = __instance.workShipOrders[__instance.workShipCount].itemId = num5;
        //                         __instance.workShipDatas[__instance.workShipCount].itemCount = num18;
        //                         __instance.workShipDatas[__instance.workShipCount].gene = __instance._tmp_iter_remote;
        //                         __instance.workShipDatas[__instance.workShipCount].shipIndex = index2;
        //                         __instance.workShipOrders[__instance.workShipCount].otherStationGId = stationComponent.gid;
        //                         __instance.workShipOrders[__instance.workShipCount].thisIndex = remotePair2.supplyIndex;
        //                         __instance.workShipOrders[__instance.workShipCount].otherIndex = remotePair2.demandIndex;
        //                         __instance.workShipOrders[__instance.workShipCount].thisOrdered = 0;
        //                         __instance.workShipOrders[__instance.workShipCount].otherOrdered = num18;
        //                         if (flag2)
        //                         {
        //                           lock (consumeRegister)
        //                           {
        //                             if (__instance.warperCount >= 2)
        //                             {
        //                               __instance.workShipDatas[__instance.workShipCount].warperCnt += 2;
        //                               __instance.warperCount -= 2;
        //                               consumeRegister[1210] += 2;
        //                             }
        //                             else if (__instance.warperCount >= 1)
        //                             {
        //                               ++__instance.workShipDatas[__instance.workShipCount].warperCnt;
        //                               --__instance.warperCount;
        //                               ++consumeRegister[1210];
        //                             }
        //                             else if (__instance.warperFree)
        //                               __instance.workShipDatas[__instance.workShipCount].warperCnt += 2;
        //                           }
        //                         }
        //                         lock (stationComponent.storage)
        //                           stationComponent.storage[remotePair2.demandIndex].remoteOrder += num18;
        //                         ++__instance.workShipCount;
        //                         --__instance.idleShipCount;
        //                         __instance.IdleShipGetToWork(index2);
        //                         lock (__instance.storage)
        //                           __instance.storage[remotePair2.supplyIndex].count -= num18;
        //                         __instance.energy -= num16;
        //                         flag3 = true;
        //                         break;
        //                       }
        //                       break;
        //                     }
        //                     break;
        //                   }
        //                   index1 = (index1 + 1) % __instance.remotePairCount;
        //                 }
        //                 while (remotePairProcess2 != index1);
        //                 if (flag3)
        //                   break;
        //               }
        //               if (__instance.energy >= num16)
        //               {
        //                 int index = __instance.QueryIdleShip(__instance.nextShipIndex);
        //                 if (index >= 0)
        //                 {
        //                   lock (__instance.storage)
        //                     num8 = __instance.storage[remotePair1.demandIndex].itemId;
        //                   __instance.nextShipIndex = (index + 1) % __instance.workShipDatas.Length;
        //                   __instance.workShipDatas[__instance.workShipCount].stage = -2;
        //                   __instance.workShipDatas[__instance.workShipCount].planetA = __instance.planetId;
        //                   __instance.workShipDatas[__instance.workShipCount].planetB = stationComponent.planetId;
        //                   __instance.workShipDatas[__instance.workShipCount].otherGId = stationComponent.gid;
        //                   __instance.workShipDatas[__instance.workShipCount].direction = 1;
        //                   __instance.workShipDatas[__instance.workShipCount].t = 0.0f;
        //                   __instance.workShipDatas[__instance.workShipCount].itemId = __instance.workShipOrders[__instance.workShipCount].itemId = num8;
        //                   __instance.workShipDatas[__instance.workShipCount].itemCount = 0;
        //                   __instance.workShipDatas[__instance.workShipCount].gene = __instance._tmp_iter_remote;
        //                   __instance.workShipDatas[__instance.workShipCount].shipIndex = index;
        //                   __instance.workShipOrders[__instance.workShipCount].otherStationGId = stationComponent.gid;
        //                   __instance.workShipOrders[__instance.workShipCount].thisIndex = remotePair1.demandIndex;
        //                   __instance.workShipOrders[__instance.workShipCount].otherIndex = remotePair1.supplyIndex;
        //                   __instance.workShipOrders[__instance.workShipCount].thisOrdered = shipCarries;
        //                   __instance.workShipOrders[__instance.workShipCount].otherOrdered = -shipCarries;
        //                   if (flag2)
        //                   {
        //                     lock (consumeRegister)
        //                     {
        //                       if (__instance.warperCount >= 2)
        //                       {
        //                         __instance.workShipDatas[__instance.workShipCount].warperCnt += 2;
        //                         __instance.warperCount -= 2;
        //                         consumeRegister[1210] += 2;
        //                       }
        //                       else if (__instance.warperCount >= 1)
        //                       {
        //                         ++__instance.workShipDatas[__instance.workShipCount].warperCnt;
        //                         --__instance.warperCount;
        //                         ++consumeRegister[1210];
        //                       }
        //                       else if (__instance.warperFree)
        //                         __instance.workShipDatas[__instance.workShipCount].warperCnt += 2;
        //                     }
        //                   }
        //                   lock (__instance.storage)
        //                     __instance.storage[remotePair1.demandIndex].remoteOrder += shipCarries;
        //                   lock (stationComponent.storage)
        //                     stationComponent.storage[remotePair1.supplyIndex].remoteOrder -= shipCarries;
        //                   ++__instance.workShipCount;
        //                   --__instance.idleShipCount;
        //                   __instance.IdleShipGetToWork(index);
        //                   __instance.energy -= num16;
        //                   break;
        //                 }
        //                 break;
        //               }
        //               break;
        //             }
        //           }
        //         }
        //         ++__instance.remotePairProcess;
        //         __instance.remotePairProcess %= __instance.remotePairCount;
        //       }
        //       while (remotePairProcess1 != __instance.remotePairProcess);
        //       ++__instance.remotePairProcess;
        //       __instance.remotePairProcess %= __instance.remotePairCount;
        //     }
        //   }
        //   float num19 = Mathf.Sqrt(shipSailSpeed / 600f);
        //   float f = num19;
        //   if ((double) f > 1.0)
        //     f = Mathf.Log(f) + 1f;
        //   AstroPose astroPose1 = astroPoses[__instance.planetId];
        //   float num20 = shipSailSpeed * 0.03f * f;
        //   float num21 = shipSailSpeed * 0.12f * f;
        //   float num22 = shipSailSpeed * 0.4f * num19;
        //   float num23 = (float) ((double) num19 * (3.0 / 500.0) + 9.99999974737875E-06);
        //   for (int destinationIndex = 0; destinationIndex < __instance.workShipCount; ++destinationIndex)
        //   {
        //     ShipData workShipData = __instance.workShipDatas[destinationIndex];
        //     bool flag1 = false;
        //     Quaternion urot = Quaternion.identity;
        //     if (workShipData.otherGId <= 0)
        //     {
        //       workShipData.direction = -1;
        //       if (workShipData.stage > 0)
        //         workShipData.stage = 0;
        //     }
        //     VectorLF3 vectorLf3_1;
        //     if (workShipData.stage < -1)
        //     {
        //       if (workShipData.direction > 0)
        //       {
        //         workShipData.t += 0.03335f;
        //         if ((double) workShipData.t > 1.0)
        //         {
        //           workShipData.t = 0.0f;
        //           workShipData.stage = -1;
        //         }
        //       }
        //       else
        //       {
        //         workShipData.t -= 0.03335f;
        //         if ((double) workShipData.t < 0.0)
        //         {
        //           workShipData.t = 0.0f;
        //           __instance.AddItem(workShipData.itemId, workShipData.itemCount);
        //           if (__instance.workShipOrders[destinationIndex].itemId > 0)
        //           {
        //             lock (__instance.storage)
        //             {
        //               if (__instance.storage[__instance.workShipOrders[destinationIndex].thisIndex].itemId == __instance.workShipOrders[destinationIndex].itemId)
        //                 __instance.storage[__instance.workShipOrders[destinationIndex].thisIndex].remoteOrder -= __instance.workShipOrders[destinationIndex].thisOrdered;
        //             }
        //             __instance.workShipOrders[destinationIndex].ClearThis();
        //           }
        //           Array.Copy((Array) __instance.workShipDatas, destinationIndex + 1, (Array) __instance.workShipDatas, destinationIndex, __instance.workShipDatas.Length - destinationIndex - 1);
        //           Array.Copy((Array) __instance.workShipOrders, destinationIndex + 1, (Array) __instance.workShipOrders, destinationIndex, __instance.workShipOrders.Length - destinationIndex - 1);
        //           --__instance.workShipCount;
        //           ++__instance.idleShipCount;
        //           __instance.WorkShipBackToIdle(workShipData.shipIndex);
        //           Array.Clear((Array) __instance.workShipDatas, __instance.workShipCount, __instance.workShipDatas.Length - __instance.workShipCount);
        //           Array.Clear((Array) __instance.workShipOrders, __instance.workShipCount, __instance.workShipOrders.Length - __instance.workShipCount);
        //           --destinationIndex;
        //           continue;
        //         }
        //       }
        //       workShipData.uPos = astroPose1.uPos + Maths.QRotateLF(astroPose1.uRot, (VectorLF3) __instance.shipDiskPos[workShipData.shipIndex]);
        //       workShipData.uVel.x = 0.0f;
        //       workShipData.uVel.y = 0.0f;
        //       workShipData.uVel.z = 0.0f;
        //       workShipData.uSpeed = 0.0f;
        //       workShipData.uRot = astroPose1.uRot * __instance.shipDiskRot[workShipData.shipIndex];
        //       workShipData.uAngularVel.x = 0.0f;
        //       workShipData.uAngularVel.y = 0.0f;
        //       workShipData.uAngularVel.z = 0.0f;
        //       workShipData.uAngularSpeed = 0.0f;
        //       workShipData.pPosTemp = (VectorLF3) Vector3.zero;
        //       workShipData.pRotTemp = Quaternion.identity;
        //       __instance.shipRenderers[workShipData.shipIndex].anim.z = 0.0f;
        //     }
        //     else if (workShipData.stage == -1)
        //     {
        //       if (workShipData.direction > 0)
        //       {
        //         workShipData.t += num23;
        //         float num15 = workShipData.t;
        //         if ((double) workShipData.t > 1.0)
        //         {
        //           workShipData.t = 1f;
        //           num15 = 1f;
        //           workShipData.stage = 0;
        //         }
        //         __instance.shipRenderers[workShipData.shipIndex].anim.z = num15;
        //         float num16 = (3f - num15 - num15) * num15 * num15;
        //         workShipData.uPos = astroPose1.uPos + Maths.QRotateLF(astroPose1.uRot, (VectorLF3) (__instance.shipDiskPos[workShipData.shipIndex] + __instance.shipDiskPos[workShipData.shipIndex].normalized * (25f * num16)));
        //         workShipData.uRot = astroPose1.uRot * __instance.shipDiskRot[workShipData.shipIndex];
        //       }
        //       else
        //       {
        //         workShipData.t -= num23 * 0.6666667f;
        //         float num15 = workShipData.t;
        //         if ((double) workShipData.t < 0.0)
        //         {
        //           workShipData.t = 1f;
        //           num15 = 0.0f;
        //           workShipData.stage = -2;
        //         }
        //         __instance.shipRenderers[workShipData.shipIndex].anim.z = num15;
        //         float num16 = (3f - num15 - num15) * num15 * num15;
        //         VectorLF3 vectorLf3_2 = astroPose1.uPos + Maths.QRotateLF(astroPose1.uRot, (VectorLF3) __instance.shipDiskPos[workShipData.shipIndex]);
        //         VectorLF3 vectorLf3_3 = astroPose1.uPos + Maths.QRotateLF(astroPose1.uRot, workShipData.pPosTemp);
        //         workShipData.uPos = vectorLf3_2 * (1.0 - (double) num16) + vectorLf3_3 * (double) num16;
        //         workShipData.uRot = astroPose1.uRot * Quaternion.Slerp(__instance.shipDiskRot[workShipData.shipIndex], workShipData.pRotTemp, (float) ((double) num16 * 2.0 - 1.0));
        //       }
        //       workShipData.uVel.x = 0.0f;
        //       workShipData.uVel.y = 0.0f;
        //       workShipData.uVel.z = 0.0f;
        //       workShipData.uSpeed = 0.0f;
        //       workShipData.uAngularVel.x = 0.0f;
        //       workShipData.uAngularVel.y = 0.0f;
        //       workShipData.uAngularVel.z = 0.0f;
        //       workShipData.uAngularSpeed = 0.0f;
        //     }
        //     else if (workShipData.stage == 0)
        //     {
        //       AstroPose astroPose2 = astroPoses[workShipData.planetB];
        //       VectorLF3 vectorLf3_2 = (workShipData.direction <= 0 ? astroPose1.uPos + Maths.QRotateLF(astroPose1.uRot, (VectorLF3) (__instance.shipDiskPos[workShipData.shipIndex] + __instance.shipDiskPos[workShipData.shipIndex].normalized * 25f)) : astroPose2.uPos + Maths.QRotateLF(astroPose2.uRot, (VectorLF3) (gStationPool[workShipData.otherGId].shipDockPos + gStationPool[workShipData.otherGId].shipDockPos.normalized * 25f))) - workShipData.uPos;
        //       double num15 = Math.Sqrt(vectorLf3_2.x * vectorLf3_2.x + vectorLf3_2.y * vectorLf3_2.y + vectorLf3_2.z * vectorLf3_2.z);
        //       VectorLF3 vectorLf3_3 = workShipData.direction > 0 ? astroPose1.uPos - workShipData.uPos : astroPose2.uPos - workShipData.uPos;
        //       double num16 = vectorLf3_3.x * vectorLf3_3.x + vectorLf3_3.y * vectorLf3_3.y + vectorLf3_3.z * vectorLf3_3.z;
        //       bool flag2 = num16 <= (double) astroPose1.uRadius * (double) astroPose1.uRadius * 2.25;
        //       bool flag3 = false;
        //       if (num15 < 6.0)
        //       {
        //         workShipData.t = 1f;
        //         workShipData.stage = workShipData.direction;
        //         flag3 = true;
        //       }
        //       float num17 = 0.0f;
        //       if (canWarp)
        //       {
        //         vectorLf3_1 = astroPose1.uPos - astroPose2.uPos;
        //         double num18 = vectorLf3_1.magnitude * 2.0;
        //         double num24 = (double) shipWarpSpeed < num18 ? (double) shipWarpSpeed : num18;
        //         double num25 = __instance.warpEnableDist * 0.5;
        //         if ((double) workShipData.warpState <= 0.0)
        //         {
        //           workShipData.warpState = 0.0f;
        //           if (num16 > 25000000.0 && num15 > num25 && (double) workShipData.uSpeed >= (double) shipSailSpeed && (workShipData.warperCnt > 0 || __instance.warperFree))
        //           {
        //             --workShipData.warperCnt;
        //             workShipData.warpState += (float) dt;
        //           }
        //         }
        //         else
        //         {
        //           num17 = (float) (num24 * ((Math.Pow(1001.0, (double) workShipData.warpState) - 1.0) / 1000.0));
        //           double num26 = (double) num17 * 0.0449 + 5000.0 + (double) shipSailSpeed * 0.25;
        //           double num27 = num15 - num26;
        //           if (num27 < 0.0)
        //             num27 = 0.0;
        //           if (num15 < num26)
        //             workShipData.warpState -= (float) (dt * 4.0);
        //           else
        //             workShipData.warpState += (float) dt;
        //           if ((double) workShipData.warpState < 0.0)
        //             workShipData.warpState = 0.0f;
        //           else if ((double) workShipData.warpState > 1.0)
        //             workShipData.warpState = 1f;
        //           if ((double) workShipData.warpState > 0.0)
        //           {
        //             num17 = (float) (num24 * ((Math.Pow(1001.0, (double) workShipData.warpState) - 1.0) / 1000.0));
        //             if ((double) num17 * dt > num27)
        //               num17 = (float) (num27 / dt * 1.01);
        //           }
        //         }
        //       }
        //       double num28 = num15 / ((double) workShipData.uSpeed + 0.1) * 0.382 * (double) f;
        //       float num29;
        //       if ((double) workShipData.warpState > 0.0)
        //       {
        //         num29 = workShipData.uSpeed = shipSailSpeed + num17;
        //         if ((double) num29 > (double) shipSailSpeed)
        //           num29 = shipSailSpeed;
        //       }
        //       else
        //       {
        //         float num18 = workShipData.uSpeed * (float) num28 + 6f;
        //         if ((double) num18 > (double) shipSailSpeed)
        //           num18 = shipSailSpeed;
        //         float num24 = (float) (dt * (flag2 ? (double) num20 : (double) num21));
        //         if ((double) workShipData.uSpeed < (double) num18 - (double) num24)
        //           workShipData.uSpeed += num24;
        //         else if ((double) workShipData.uSpeed > (double) num18 + (double) num22)
        //           workShipData.uSpeed -= num22;
        //         else
        //           workShipData.uSpeed = num18;
        //         num29 = workShipData.uSpeed;
        //       }
        //       int index1 = -1;
        //       double num30 = 0.0;
        //       double d = 1E+40;
        //       int num31 = workShipData.planetA / 100 * 100;
        //       int num32 = workShipData.planetB / 100 * 100;
        //       for (int index2 = num31; index2 < num31 + GameMain.galaxy.PlanetById(num31 + 1).star.planetCount + 1; ++index2)
        //       {
        //         float uRadius = astroPoses[index2].uRadius;
        //         if ((double) uRadius >= 1.0)
        //         {
        //           VectorLF3 vectorLf3_4 = workShipData.uPos - astroPoses[index2].uPos;
        //           double num18 = vectorLf3_4.x * vectorLf3_4.x + vectorLf3_4.y * vectorLf3_4.y + vectorLf3_4.z * vectorLf3_4.z;
        //           double num24 = -((double) workShipData.uVel.x * vectorLf3_4.x + (double) workShipData.uVel.y * vectorLf3_4.y + (double) workShipData.uVel.z * vectorLf3_4.z);
        //           if ((num24 > 0.0 || num18 < (double) uRadius * (double) uRadius * 7.0) && num18 < d)
        //           {
        //             num30 = num24 < 0.0 ? 0.0 : num24;
        //             index1 = index2;
        //             d = num18;
        //           }
        //         }
        //       }
        //       if (num32 != num31)
        //       {
        //         for (int index2 = num32; index2 < num32 + GameMain.galaxy.PlanetById(num32 + 1).star.planetCount + 1; ++index2)
        //         {
        //           float uRadius = astroPoses[index2].uRadius;
        //           if ((double) uRadius >= 1.0)
        //           {
        //             VectorLF3 vectorLf3_4 = workShipData.uPos - astroPoses[index2].uPos;
        //             double num18 = vectorLf3_4.x * vectorLf3_4.x + vectorLf3_4.y * vectorLf3_4.y + vectorLf3_4.z * vectorLf3_4.z;
        //             double num24 = -((double) workShipData.uVel.x * vectorLf3_4.x + (double) workShipData.uVel.y * vectorLf3_4.y + (double) workShipData.uVel.z * vectorLf3_4.z);
        //             if ((num24 > 0.0 || num18 < (double) uRadius * (double) uRadius * 7.0) && num18 < d)
        //             {
        //               num30 = num24 < 0.0 ? 0.0 : num24;
        //               index1 = index2;
        //               d = num18;
        //             }
        //           }
        //         }
        //       }
        //       VectorLF3 zero = VectorLF3.zero;
        //       VectorLF3 vectorLf3_5 = VectorLF3.zero;
        //       float num33 = 0.0f;
        //       VectorLF3 vectorLf3_6 = (VectorLF3) Vector3.zero;
        //       if (index1 > 0)
        //       {
        //         float uRadius = astroPoses[index1].uRadius;
        //         if (index1 % 100 == 0)
        //           uRadius *= 2.5f;
        //         vectorLf3_1 = astroPoses[index1].uPosNext - astroPoses[index1].uPos;
        //         double num18 = Math.Max(1.0, (vectorLf3_1.magnitude - 0.5) * 0.6);
        //         double num24 = 1.0 + 1600.0 / (double) uRadius;
        //         double num25 = 1.0 + 250.0 / (double) uRadius;
        //         double num26 = num24 * (num18 * num18);
        //         double num27 = index1 == workShipData.planetA || index1 == workShipData.planetB ? 1.25 : 1.5;
        //         double num34 = Math.Sqrt(d);
        //         double num35 = (double) uRadius / num34 * 1.6 - 0.1;
        //         if (num35 > 1.0)
        //           num35 = 1.0;
        //         else if (num35 < 0.0)
        //           num35 = 0.0;
        //         double num36 = num34 - (double) uRadius * 0.82;
        //         if (num36 < 1.0)
        //           num36 = 1.0;
        //         double num37 = ((double) num29 - 6.0) / (num36 * (double) f) * 0.6 - 0.01;
        //         if (num37 > 1.5)
        //           num37 = 1.5;
        //         else if (num37 < 0.0)
        //           num37 = 0.0;
        //         VectorLF3 vectorLf3_4 = workShipData.uPos + (VectorLF3) workShipData.uVel * num30 - astroPoses[index1].uPos;
        //         double num38 = vectorLf3_4.magnitude / (double) uRadius;
        //         if (num38 < num27)
        //         {
        //           double num39 = (num38 - 1.0) / (num27 - 1.0);
        //           if (num39 < 0.0)
        //             num39 = 0.0;
        //           double num40 = 1.0 - num39 * num39;
        //           vectorLf3_5 = vectorLf3_4.normalized * (num37 * num37 * num40 * 2.0 * (1.0 - (double) workShipData.warpState));
        //         }
        //         VectorLF3 v1 = workShipData.uPos - astroPoses[index1].uPos;
        //         VectorLF3 vectorLf3_7 = new VectorLF3(v1.x / num34, v1.y / num34, v1.z / num34);
        //         zero += vectorLf3_7 * num35;
        //         num33 = (float) num35;
        //         double num41 = num34 / (double) uRadius;
        //         double num42 = num41 * num41;
        //         double num43 = (num26 - num42) / (num26 - num25);
        //         if (num43 > 1.0)
        //           num43 = 1.0;
        //         else if (num43 < 0.0)
        //           num43 = 0.0;
        //         if (num43 > 0.0)
        //         {
        //           VectorLF3 v2 = Maths.QInvRotateLF(astroPoses[index1].uRot, v1);
        //           VectorLF3 vectorLf3_8 = Maths.QRotateLF(astroPoses[index1].uRotNext, v2) + astroPoses[index1].uPosNext;
        //           double num39 = (3.0 - num43 - num43) * num43 * num43;
        //           VectorLF3 uPos = workShipData.uPos;
        //           vectorLf3_6 = (vectorLf3_8 - uPos) * num39;
        //         }
        //       }
        //       Vector3 up;
        //       workShipData.uRot.ForwardUp(out workShipData.uVel, out up);
        //       Vector3 lhs = up * (1f - num33) + (Vector3) zero * num33;
        //       Vector3 vector3_1 = lhs - Vector3.Dot(lhs, workShipData.uVel) * workShipData.uVel;
        //       vector3_1.Normalize();
        //       Vector3 vector3_2 = (Vector3) (vectorLf3_2.normalized + vectorLf3_5);
        //       Vector3 vector3_3 = Vector3.Cross(workShipData.uVel, vector3_2);
        //       float num44 = (float) ((double) workShipData.uVel.x * (double) vector3_2.x + (double) workShipData.uVel.y * (double) vector3_2.y + (double) workShipData.uVel.z * (double) vector3_2.z);
        //       Vector3 vector3_4 = Vector3.Cross(up, vector3_1);
        //       double num45 = (double) up.x * (double) vector3_1.x + (double) up.y * (double) vector3_1.y + (double) up.z * (double) vector3_1.z;
        //       if ((double) num44 < 0.0)
        //         vector3_3 = vector3_3.normalized;
        //       if (num45 < 0.0)
        //         vector3_4 = vector3_4.normalized;
        //       float num46 = num28 < 3.0 ? (float) ((3.25 - num28) * 4.0) : (float) ((double) num29 / (double) shipSailSpeed * (flag2 ? 0.200000002980232 : 1.0));
        //       vector3_3 = vector3_3 * num46 + vector3_4 * 2f;
        //       Vector3 vector3_5 = vector3_3 - workShipData.uAngularVel;
        //       float num47 = (double) vector3_5.sqrMagnitude < 0.100000001490116 ? 1f : 0.05f;
        //       workShipData.uAngularVel += vector3_5 * num47;
        //       double num48 = (double) workShipData.uSpeed * dt;
        //       workShipData.uPos.x = workShipData.uPos.x + (double) workShipData.uVel.x * num48 + vectorLf3_6.x;
        //       workShipData.uPos.y = workShipData.uPos.y + (double) workShipData.uVel.y * num48 + vectorLf3_6.y;
        //       workShipData.uPos.z = workShipData.uPos.z + (double) workShipData.uVel.z * num48 + vectorLf3_6.z;
        //       Vector3 normalized1 = workShipData.uAngularVel.normalized;
        //       double num49 = (double) workShipData.uAngularVel.magnitude * dt * 0.5;
        //       float w = (float) Math.Cos(num49);
        //       float num50 = (float) Math.Sin(num49);
        //       Quaternion quaternion = new Quaternion(normalized1.x * num50, normalized1.y * num50, normalized1.z * num50, w);
        //       workShipData.uRot = quaternion * workShipData.uRot;
        //       if ((double) workShipData.warpState > 0.0)
        //       {
        //         float t = workShipData.warpState * workShipData.warpState * workShipData.warpState;
        //         workShipData.uRot = Quaternion.Slerp(workShipData.uRot, Quaternion.LookRotation(vector3_2, vector3_1), t);
        //         workShipData.uAngularVel *= 1f - t;
        //       }
        //       if (num15 < 100.0)
        //       {
        //         float num18 = (float) (1.0 - num15 / 100.0);
        //         float num24 = (3f - num18 - num18) * num18 * num18;
        //         float t = num24 * num24;
        //         if (workShipData.direction > 0)
        //         {
        //           urot = Quaternion.Slerp(workShipData.uRot, astroPose2.uRot * (gStationPool[workShipData.otherGId].shipDockRot * new Quaternion(0.7071068f, 0.0f, 0.0f, -0.7071068f)), t);
        //         }
        //         else
        //         {
        //           vectorLf3_1 = workShipData.uPos - astroPose1.uPos;
        //           Vector3 normalized2 = (Vector3) vectorLf3_1.normalized;
        //           Vector3 normalized3 = (workShipData.uVel - Vector3.Dot(workShipData.uVel, normalized2) * normalized2).normalized;
        //           urot = Quaternion.Slerp(workShipData.uRot, Quaternion.LookRotation(normalized3, normalized2), t);
        //         }
        //         flag1 = true;
        //       }
        //       if (flag3)
        //       {
        //         workShipData.uRot = urot;
        //         if (workShipData.direction > 0)
        //         {
        //           workShipData.pPosTemp = Maths.QInvRotateLF(astroPose2.uRot, workShipData.uPos - astroPose2.uPos);
        //           workShipData.pRotTemp = Quaternion.Inverse(astroPose2.uRot) * workShipData.uRot;
        //         }
        //         else
        //         {
        //           workShipData.pPosTemp = Maths.QInvRotateLF(astroPose1.uRot, workShipData.uPos - astroPose1.uPos);
        //           workShipData.pRotTemp = Quaternion.Inverse(astroPose1.uRot) * workShipData.uRot;
        //         }
        //         urot = Quaternion.identity;
        //         flag1 = false;
        //       }
        //       if ((double) __instance.shipRenderers[workShipData.shipIndex].anim.z > 1.0)
        //         __instance.shipRenderers[workShipData.shipIndex].anim.z -= (float) dt * 0.3f;
        //       else
        //         __instance.shipRenderers[workShipData.shipIndex].anim.z = 1f;
        //       __instance.shipRenderers[workShipData.shipIndex].anim.w = workShipData.warpState;
        //     }
        //     else if (workShipData.stage == 1)
        //     {
        //       AstroPose astroPose2 = astroPoses[workShipData.planetB];
        //       float num15;
        //       if (workShipData.direction > 0)
        //       {
        //         workShipData.t -= num23 * 0.6666667f;
        //         float num16 = workShipData.t;
        //         if ((double) workShipData.t < 0.0)
        //         {
        //           workShipData.t = 1f;
        //           num16 = 0.0f;
        //           workShipData.stage = 2;
        //         }
        //         num15 = (3f - num16 - num16) * num16 * num16;
        //         float num17 = num15 * 2f;
        //         float num18 = (float) ((double) num15 * 2.0 - 1.0);
        //         VectorLF3 vectorLf3_2 = astroPose2.uPos + Maths.QRotateLF(astroPose2.uRot, (VectorLF3) (gStationPool[workShipData.otherGId].shipDockPos + gStationPool[workShipData.otherGId].shipDockPos.normalized * 7.27f));
        //         if ((double) num15 > 0.5)
        //         {
        //           VectorLF3 vectorLf3_3 = astroPose2.uPos + Maths.QRotateLF(astroPose2.uRot, workShipData.pPosTemp);
        //           workShipData.uPos = vectorLf3_2 * (1.0 - (double) num18) + vectorLf3_3 * (double) num18;
        //           workShipData.uRot = astroPose2.uRot * Quaternion.Slerp(gStationPool[workShipData.otherGId].shipDockRot * new Quaternion(0.7071068f, 0.0f, 0.0f, -0.7071068f), workShipData.pRotTemp, (float) ((double) num18 * 1.5 - 0.5));
        //         }
        //         else
        //         {
        //           VectorLF3 vectorLf3_3 = astroPose2.uPos + Maths.QRotateLF(astroPose2.uRot, (VectorLF3) (gStationPool[workShipData.otherGId].shipDockPos + gStationPool[workShipData.otherGId].shipDockPos.normalized * -14.4f));
        //           workShipData.uPos = vectorLf3_3 * (1.0 - (double) num17) + vectorLf3_2 * (double) num17;
        //           workShipData.uRot = astroPose2.uRot * (gStationPool[workShipData.otherGId].shipDockRot * new Quaternion(0.7071068f, 0.0f, 0.0f, -0.7071068f));
        //         }
        //       }
        //       else
        //       {
        //         workShipData.t += num23;
        //         float num16 = workShipData.t;
        //         if ((double) workShipData.t > 1.0)
        //         {
        //           workShipData.t = 1f;
        //           num16 = 1f;
        //           workShipData.stage = 0;
        //         }
        //         num15 = (3f - num16 - num16) * num16 * num16;
        //         workShipData.uPos = astroPose2.uPos + Maths.QRotateLF(astroPose2.uRot, (VectorLF3) (gStationPool[workShipData.otherGId].shipDockPos + gStationPool[workShipData.otherGId].shipDockPos.normalized * (float) (39.4000015258789 * (double) num15 - 14.3999996185303)));
        //         workShipData.uRot = astroPose2.uRot * (gStationPool[workShipData.otherGId].shipDockRot * new Quaternion(0.7071068f, 0.0f, 0.0f, -0.7071068f));
        //       }
        //       workShipData.uVel.x = 0.0f;
        //       workShipData.uVel.y = 0.0f;
        //       workShipData.uVel.z = 0.0f;
        //       workShipData.uSpeed = 0.0f;
        //       workShipData.uAngularVel.x = 0.0f;
        //       workShipData.uAngularVel.y = 0.0f;
        //       workShipData.uAngularVel.z = 0.0f;
        //       workShipData.uAngularSpeed = 0.0f;
        //       __instance.shipRenderers[workShipData.shipIndex].anim.z = (float) ((double) num15 * 1.70000004768372 - 0.699999988079071);
        //     }
        //     else
        //     {
        //       if (workShipData.direction > 0)
        //       {
        //         workShipData.t -= 0.0334f;
        //         if ((double) workShipData.t < 0.0)
        //         {
        //           workShipData.t = 0.0f;
        //           StationComponent stationComponent = gStationPool[workShipData.otherGId];
        //           StationStore[] storage = stationComponent.storage;
        //           vectorLf3_1 = astroPoses[workShipData.planetA].uPos - astroPoses[workShipData.planetB].uPos;
        //           if (vectorLf3_1.sqrMagnitude > __instance.warpEnableDist * __instance.warpEnableDist && workShipData.warperCnt == 0 && stationComponent.warperCount > 0)
        //           {
        //             lock (consumeRegister)
        //             {
        //               ++workShipData.warperCnt;
        //               --stationComponent.warperCount;
        //               ++consumeRegister[1210];
        //             }
        //           }
        //           if (workShipData.itemCount > 0)
        //           {
        //             stationComponent.AddItem(workShipData.itemId, workShipData.itemCount);
        //             workShipData.itemCount = 0;
        //             if (__instance.workShipOrders[destinationIndex].otherStationGId > 0)
        //             {
        //               lock (storage)
        //               {
        //                 if (storage[__instance.workShipOrders[destinationIndex].otherIndex].itemId == __instance.workShipOrders[destinationIndex].itemId)
        //                   storage[__instance.workShipOrders[destinationIndex].otherIndex].remoteOrder -= __instance.workShipOrders[destinationIndex].otherOrdered;
        //               }
        //               __instance.workShipOrders[destinationIndex].ClearOther();
        //             }
        //             if (__instance.remotePairCount > 0)
        //             {
        //               __instance.remotePairProcess %= __instance.remotePairCount;
        //               int remotePairProcess = __instance.remotePairProcess;
        //               int index = __instance.remotePairProcess;
        //               do
        //               {
        //                 SupplyDemandPair remotePair = __instance.remotePairs[index];
        //                 if (remotePair.demandId == __instance.gid && remotePair.supplyId == stationComponent.gid)
        //                 {
        //                   lock (__instance.storage)
        //                   {
        //                     num6 = __instance.storage[remotePair.demandIndex].remoteDemandCount;
        //                     num7 = __instance.storage[remotePair.demandIndex].totalDemandCount;
        //                     num8 = __instance.storage[remotePair.demandIndex].itemId;
        //                   }
        //                 }
        //                 if (remotePair.demandId == __instance.gid && remotePair.supplyId == stationComponent.gid)
        //                 {
        //                   lock (storage)
        //                   {
        //                     num12 = storage[remotePair.supplyIndex].count;
        //                     num13 = storage[remotePair.supplyIndex].remoteSupplyCount;
        //                     num14 = storage[remotePair.supplyIndex].totalSupplyCount;
        //                   }
        //                 }
        //                 if (remotePair.demandId == __instance.gid && remotePair.supplyId == stationComponent.gid && (num6 > 0 && num7 > 0) && (num12 >= shipCarries && num13 >= shipCarries && num14 >= shipCarries))
        //                 {
        //                   int num15 = num12;
        //                   if (num15 > shipCarries)
        //                     num15 = shipCarries;
        //                   workShipData.itemId = __instance.workShipOrders[destinationIndex].itemId = num8;
        //                   workShipData.itemCount = num15;
        //                   lock (storage)
        //                     storage[remotePair.supplyIndex].count -= num15;
        //                   __instance.workShipOrders[destinationIndex].otherStationGId = stationComponent.gid;
        //                   __instance.workShipOrders[destinationIndex].thisIndex = remotePair.demandIndex;
        //                   __instance.workShipOrders[destinationIndex].otherIndex = remotePair.supplyIndex;
        //                   __instance.workShipOrders[destinationIndex].thisOrdered = num15;
        //                   __instance.workShipOrders[destinationIndex].otherOrdered = 0;
        //                   lock (__instance.storage)
        //                   {
        //                     __instance.storage[remotePair.demandIndex].remoteOrder += num15;
        //                     break;
        //                   }
        //                 }
        //                 else
        //                   index = (index + 1) % __instance.remotePairCount;
        //               }
        //               while (remotePairProcess != index);
        //             }
        //           }
        //           else
        //           {
        //             int itemId = workShipData.itemId;
        //             int count = shipCarries;
        //             stationComponent.TakeItem(ref itemId, ref count);
        //             workShipData.itemCount = count;
        //             if (__instance.workShipOrders[destinationIndex].otherStationGId > 0)
        //             {
        //               lock (storage)
        //               {
        //                 if (storage[__instance.workShipOrders[destinationIndex].otherIndex].itemId == __instance.workShipOrders[destinationIndex].itemId)
        //                   storage[__instance.workShipOrders[destinationIndex].otherIndex].remoteOrder -= __instance.workShipOrders[destinationIndex].otherOrdered;
        //               }
        //               __instance.workShipOrders[destinationIndex].ClearOther();
        //             }
        //             lock (__instance.storage)
        //             {
        //               if (__instance.storage[__instance.workShipOrders[destinationIndex].thisIndex].itemId == __instance.workShipOrders[destinationIndex].itemId)
        //               {
        //                 if (__instance.workShipOrders[destinationIndex].thisOrdered != count)
        //                 {
        //                   int num15 = count - __instance.workShipOrders[destinationIndex].thisOrdered;
        //                   __instance.storage[__instance.workShipOrders[destinationIndex].thisIndex].remoteOrder += num15;
        //                   __instance.workShipOrders[destinationIndex].thisOrdered += num15;
        //                 }
        //               }
        //             }
        //           }
        //           workShipData.direction = -1;
        //         }
        //       }
        //       else
        //       {
        //         workShipData.t += 0.0334f;
        //         if ((double) workShipData.t > 1.0)
        //         {
        //           workShipData.t = 0.0f;
        //           workShipData.stage = 1;
        //         }
        //       }
        //       AstroPose astroPose2 = astroPoses[workShipData.planetB];
        //       workShipData.uPos = astroPose2.uPos + Maths.QRotateLF(astroPose2.uRot, (VectorLF3) (gStationPool[workShipData.otherGId].shipDockPos + gStationPool[workShipData.otherGId].shipDockPos.normalized * -14.4f));
        //       workShipData.uVel.x = 0.0f;
        //       workShipData.uVel.y = 0.0f;
        //       workShipData.uVel.z = 0.0f;
        //       workShipData.uSpeed = 0.0f;
        //       workShipData.uRot = astroPose2.uRot * (gStationPool[workShipData.otherGId].shipDockRot * new Quaternion(0.7071068f, 0.0f, 0.0f, -0.7071068f));
        //       workShipData.uAngularVel.x = 0.0f;
        //       workShipData.uAngularVel.y = 0.0f;
        //       workShipData.uAngularVel.z = 0.0f;
        //       workShipData.uAngularSpeed = 0.0f;
        //       workShipData.pPosTemp = (VectorLF3) Vector3.zero;
        //       workShipData.pRotTemp = Quaternion.identity;
        //       __instance.shipRenderers[workShipData.shipIndex].anim.z = 0.0f;
        //     }
        //     __instance.workShipDatas[destinationIndex] = workShipData;
        //     if (flag1)
        //     {
        //       __instance.shipRenderers[workShipData.shipIndex].SetPose(workShipData.uPos, urot, relativePos, relativeRot, workShipData.uVel * workShipData.uSpeed, workShipData.itemCount > 0 ? workShipData.itemId : 0);
        //       if (starmap)
        //       {
        //         ref ShipUIRenderingData local = ref __instance.shipUIRenderers[workShipData.shipIndex];
        //         VectorLF3 uPos = workShipData.uPos;
        //         Quaternion _urot = urot;
        //         vectorLf3_1 = astroPoses[workShipData.planetA].uPos - astroPoses[workShipData.planetB].uPos;
        //         double magnitude = vectorLf3_1.magnitude;
        //         double uSpeed = (double) workShipData.uSpeed;
        //         int _itemId = workShipData.itemCount > 0 ? workShipData.itemId : 0;
        //         local.SetPose(uPos, _urot, (float) magnitude, (float) uSpeed, _itemId);
        //       }
        //     }
        //     else
        //     {
        //       __instance.shipRenderers[workShipData.shipIndex].SetPose(workShipData.uPos, workShipData.uRot, relativePos, relativeRot, workShipData.uVel * workShipData.uSpeed, workShipData.itemCount > 0 ? workShipData.itemId : 0);
        //       if (starmap)
        //       {
        //         ref ShipUIRenderingData local = ref __instance.shipUIRenderers[workShipData.shipIndex];
        //         VectorLF3 uPos = workShipData.uPos;
        //         Quaternion uRot = workShipData.uRot;
        //         vectorLf3_1 = astroPoses[workShipData.planetA].uPos - astroPoses[workShipData.planetB].uPos;
        //         double magnitude = vectorLf3_1.magnitude;
        //         double uSpeed = (double) workShipData.uSpeed;
        //         int _itemId = workShipData.itemCount > 0 ? workShipData.itemId : 0;
        //         local.SetPose(uPos, uRot, (float) magnitude, (float) uSpeed, _itemId);
        //       }
        //     }
        //     if ((double) __instance.shipRenderers[workShipData.shipIndex].anim.z < 0.0)
        //       __instance.shipRenderers[workShipData.shipIndex].anim.z = 0.0f;
        //   }
        //   __instance.ShipRenderersOnTick(astroPoses, relativePos, relativeRot);
        //   return false;
        // }
    }
}