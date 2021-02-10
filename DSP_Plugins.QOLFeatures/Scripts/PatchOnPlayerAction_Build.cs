using System.Collections.Generic;
using DSP_Plugins.Shared;
using HarmonyLib;
using UnityEngine;

namespace DSP_Plugins.QOLFeatures {
  [HarmonyPatch(typeof(PlayerAction_Build))]
        public class PatchOnPlayerAction_Build {
            [HarmonyPrefix]
            [HarmonyPatch("DetermineDestructPreviews")]
            public static bool DetermineDestructPreviewsPatch(PlayerAction_Build __instance,
                ref NearColliderLogic ___nearcdLogic,
                ref PlanetFactory ___factory) {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl)) {
                    if (!VFInput.onGUI) {
                        UICursor.SetCursor(ECursor.Delete);
                    }
                }
                else {
                    return true;
                }

                if (!Utils.CheckIfInBuildDistance(__instance.cursorTarget)) {
                    // Out of reach
                    UnityEngine.Debug.Log("out of reach");
                    return true;
                }

                List<EntityData> deleteEntitiesList = new List<EntityData>();
                ItemProto itemProto = Traverse.Create(__instance).Method(
                    "GetItemProto", __instance.castObjId).GetValue<ItemProto>();
                __instance.ClearBuildPreviews();

                if (VFInput.reformMinusKey.onDown) {
                    if (__instance.reformSize >= 0) {
                        __instance.reformSize--;
                    }
                }

                if (VFInput.reformPlusKey.onDown) {
                    if (__instance.reformSize < QOLFeatures.ConfigDisassemblingRadiusMax.Value) {
                        __instance.reformSize++;
                    }
                }

                // Management of the sphere delete
                if (Input.GetKey(KeyCode.LeftControl)) {
                    int[] buildingIdsToDelete = new int[QOLFeatures.MAXArrayOfBuildingSize.Value];
                    if (itemProto != null) {
                        ___nearcdLogic.GetBuildingsInAreaNonAlloc(__instance.castObjPos, __instance.reformSize, buildingIdsToDelete);
                    }
                    else {
                        ___nearcdLogic.GetBuildingsInAreaNonAlloc(__instance.cursorTarget, __instance.reformSize, buildingIdsToDelete);
                    }
                   
                    List<int> listBuildingIdsToDelete = new List<int>();

                    foreach (var id in buildingIdsToDelete) {
                        if (id != 0) {
                            listBuildingIdsToDelete.Add(id);
                        }
                    }

                    foreach (var item in listBuildingIdsToDelete) {
                        deleteEntitiesList.Add(___factory.entityPool[item]);
                    }

                    // Management of both
                    if (Input.GetKey(KeyCode.LeftShift)) {
                        if (itemProto != null) {
                            deleteEntitiesList = Utils.GetEntitySortedByTypeAndRadius(itemProto, deleteEntitiesList,
                                __instance.castObjPos, __instance.reformSize);
                        }
                   
                          
               
                    }
                }

                // Management of the Mass Item delete
                if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)) {
                    __instance.previewPose.position = Vector3.zero;
                    __instance.previewPose.rotation = Quaternion.identity;
                    if ((uint) __instance.castObjId > 0U) {
                        if (itemProto != null) {
                            if ((uint) ___factory.entityPool[__instance.castObjId].beltId > 0U) {
                                CargoPath pathByBeltId = Utils.GetPathWithBeltId(___factory,
                                    ___factory.entityPool[__instance.castObjId].beltId);
                                deleteEntitiesList = Utils.GetBeltsEntitiesByCargoPathBuildRange(___factory, pathByBeltId);
                            }
                            else {
                                // deleteEntitiesList = Utils.GetEntitiesByProtoBuildRange(___factory, itemProto);
                                return true;
                            }
                        }
                    }
                }


                foreach (var entityData in deleteEntitiesList) {
                    __instance.AddBuildPreview(new BuildPreview());
                }

                // Common Code
                for (int index = 0; index < __instance.buildPreviews.Count; ++index) {
                    BuildPreview buildPreview = __instance.buildPreviews[index];
                    ItemProto itemProto2 = Traverse.Create(__instance).Method(
                        "GetItemProto", deleteEntitiesList[index].id).GetValue<ItemProto>();
                    buildPreview.item = itemProto2;
                    buildPreview.desc = itemProto2.prefabDesc;
                    buildPreview.lpos = deleteEntitiesList[index].pos;
                    buildPreview.lrot = deleteEntitiesList[index].rot;
                    buildPreview.objId = deleteEntitiesList[index].id;
                    int num = buildPreview.desc.lodCount <= 0
                        ? 0
                        : ((Object) buildPreview.desc.lodMeshes[0] != (Object) null ? 1 : 0);
                    buildPreview.needModel = num != 0;
                    buildPreview.isConnNode = true;
                    if (buildPreview.desc.isInserter) {
                        Pose pose = Traverse.Create(__instance).Method("GetObjectPose2", buildPreview.objId)
                            .GetValue<Pose>();
                        buildPreview.lpos2 = pose.position;
                        buildPreview.lrot2 = pose.rotation;
                    }

                    if ((buildPreview.lpos - __instance.player.position).sqrMagnitude >
                        __instance.player.mecha.buildArea *
                        __instance.player.mecha.buildArea) {
                        buildPreview.condition = EBuildCondition.OutOfReach;
                        __instance.cursorText = "目标超出范围".Translate();
                        __instance.cursorWarning = true;
                    }
                    else {
                        buildPreview.condition = EBuildCondition.Ok;
                        __instance.cursorText = "拆除".Translate() + buildPreview.item.name;
                    }

                    if (buildPreview.desc.multiLevel) {
                        ___factory.ReadObjectConn(buildPreview.objId, 15, out bool _, out int otherObjId,
                            out int _);
                        if ((uint) otherObjId > 0U) {
                            buildPreview.condition = EBuildCondition.Covered;
                            __instance.cursorText = buildPreview.conditionText;
                        }
                    }
                }


                return false;
            }
        }
}