using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(UIBuildingGrid))]
    public class PatchOnUIBuildingGrid {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool PrefixUpdate(UIBuildingGrid __instance, PlanetData ___reformMapPlanet, ref float ___displayScale, byte[] ___reformCursorMap, Material ___material, Material ___altMaterial) {
            Patch.Debug("Custom UIBuildingGrid Update!", BepInEx.Logging.LogLevel.Debug, true);
            __instance.reformColor = new Color(1, 0, 1);
            if (!texDumped) {
                DumpTextures(___material);
            }

            PlanetData planetData = GameMain.localPlanet;
            Player mainPlayer = GameMain.mainPlayer;
            PlanetFactory planetFactory = planetData?.factory;
            if (planetFactory == null || !planetData.factoryLoaded) {
                planetData = null;
            }
            if (___reformMapPlanet != planetData) {
                __instance.InitReformMap(planetData);
            }
            PlanetGrid planetGrid = null;
            if (mainPlayer != null && planetData != null && planetData.aux != null && (uint) planetData.aux.activeGridIndex < (uint) planetData.aux.customGrids.Count) {
                planetGrid = planetData.aux.customGrids[planetData.aux.activeGridIndex];
            }
            if (planetGrid != null) {
                float realRadius = planetData.realRadius;
                int showingAltitude = mainPlayer.controller.actionBuild.showingAltitude;
                float num = realRadius * 2f;
                float num2 = (realRadius + 0.2f + (float) showingAltitude * 1.33333325f) * 2f;
                CommandState cmd = mainPlayer.controller.cmd;
                if (cmd.state == 1 && planetData.type != EPlanetType.Gas) {
                    Vector3 test = cmd.test;
                    num = test.magnitude * 2f;
                    if (cmd.mode == 4) {
                        float num3 = (realRadius + 0.2f) * 2f;
                        num = ((!(num > num3)) ? num3 : num);
                    }
                }
                if (Mathf.Abs(___displayScale - num) > 10f) {
                    ___displayScale = num;
                }
                else {
                    ___displayScale = ___displayScale * 0.8f + num * 0.2f;
                }
                PlatformSystem platformSystem = planetFactory.platformSystem;
                int num4 = 0;
                if (___reformCursorMap != null) {
                    num4 = ___reformCursorMap.Length;
                    bool expression = ___reformCursorMap.Length != platformSystem.maxReformCount;
                    Assert.False(expression, "断言失败：cursorMap长度不相等");
                }
                PlayerAction_Build actionBuild = mainPlayer.controller.actionBuild;
                ComputeBuffer reformOffsetsBuffer = platformSystem.reformOffsetsBuffer;
                ComputeBuffer reformDataBuffer = platformSystem.reformDataBuffer;
                if (cmd.type == ECommand.Build) {
                    __instance.gridRnd.enabled = true;
                    __instance.gridRnd.transform.localScale = new Vector3(___displayScale, ___displayScale, ___displayScale);
                    __instance.gridRnd.transform.rotation = planetGrid.rotation;
                    __instance.altGridRnd.enabled = ((showingAltitude > 0) ? true : false);
                    __instance.altGridRnd.transform.localScale = new Vector3(num2, num2, num2);
                    ___material.SetFloat("_Segment", planetGrid.segment);
                    int[] reformIndices = actionBuild.reformIndices;
                    int num5 = actionBuild.reformSize * actionBuild.reformSize;
                    int mode = cmd.mode;
                    Vector4 value = Vector4.zero;
                    switch (mode) {
                        case -1:
                            ___material.SetColor("_TintColor", __instance.destructColor);
                            ___material.SetFloat("_ReformMode", 0f);
                            ___material.SetFloat("_ZMin", -0.5f);
                            num5 = 0;
                            if (actionBuild.destructCursor > 0 && actionBuild.castGround) {
                                value = planetGrid.GratboxByCenterSize(actionBuild.groundTestPos, actionBuild.destructCursorSize);
                            }
                            break;
                        case -2:
                            ___material.SetColor("_TintColor", (actionBuild.upgradeLevel < 0) ? __instance.downgradeColor : __instance.upgradeColor);
                            ___material.SetFloat("_ReformMode", 0f);
                            ___material.SetFloat("_ZMin", -0.5f);
                            num5 = 0;
                            if (actionBuild.upgradeCursor > 0 && actionBuild.castGround) {
                                value = planetGrid.GratboxByCenterSize(actionBuild.groundTestPos, actionBuild.upgradeCursorSize);
                            }
                            break;
                        case 4:
                            ___material.SetColor("_TintColor", __instance.reformColor);
                            ___material.SetFloat("_ReformMode", 1f);
                            ___material.SetFloat("_ZMin", -1.5f);
                            break;
                        default:
                            ___material.SetColor("_TintColor", __instance.buildColor);
                            ___material.SetFloat("_ReformMode", 0f);
                            ___material.SetFloat("_ZMin", -0.5f);
                            num5 = 0;
                            break;
                    }
                    ___material.SetVector("_CursorGratBox", value);
                    if (!VFInput.onGUI) {
                        for (int i = 0; i < num5; i++) {
                            int num6 = reformIndices[i];
                            if (num6 >= 0 && num6 < num4) {
                                ___reformCursorMap[num6] = 1;
                            }
                        }
                    }
                    if (platformSystem.reformData != null) {
                        reformDataBuffer.SetData(platformSystem.reformData);
                        ___material.SetBuffer("_DataBuffer", reformDataBuffer);
                    }
                    reformOffsetsBuffer.SetData(platformSystem.reformOffsets);
                    __instance.reformCursorBuffer.SetData(___reformCursorMap);
                    ___material.SetBuffer("_OffsetsBuffer", reformOffsetsBuffer);
                    ___material.SetBuffer("_CursorBuffer", __instance.reformCursorBuffer);
                    if (VFInput.onGUI) {
                        return false;
                    }
                    for (int j = 0; j < num5; j++) {
                        int num7 = reformIndices[j];
                        if (num7 >= 0 && num7 < num4) {
                            ___reformCursorMap[num7] = 0;
                        }
                    }
                }
                else {
                    __instance.gridRnd.enabled = false;
                    __instance.altGridRnd.enabled = false;
                }
            }
            else {
                __instance.gridRnd.enabled = false;
                __instance.altGridRnd.enabled = false;
            }

            return false;
        }

        public static void DumpTextures(params Material[] mats) {
            foreach (Material mat in mats) {
                Patch.Debug("Material name: " + mat.name, BepInEx.Logging.LogLevel.Debug, true);
                if (mat == null) {
                    continue;
                }
                foreach (string str in mat.GetTexturePropertyNames()) {
                    Texture tex = mat.GetTexture(str);
                    Patch.Debug("Texture: " + tex.name + "/ID: " + str + " with wrapMode " + tex.wrapMode.ToString() + ", width " + tex.width + ", height" + tex.height + ", dimension " + tex.dimension.ToString(), BepInEx.Logging.LogLevel.Debug, true);
                    if (tex.name == "segment-table") {
                        Texture2D tex2d = tex as Texture2D;
                        Color c = tex2d.GetPixel(10, 0);
                        Patch.Debug("Color is level " + c.r * 255f, BepInEx.Logging.LogLevel.Debug, true);
                    }
                }
                Patch.Debug("IDs:", BepInEx.Logging.LogLevel.Debug, true);
            }
            texDumped = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void UpdateTexture(Material ___material, Material ___altMaterial) {
            //segment-table
            Patch.Debug("Updating segment-table of material.", BepInEx.Logging.LogLevel.Debug, true);

            Texture tex = ___material.GetTexture("_SegmentTable");
            if (tex.dimension == TextureDimension.Tex2D) {
                Texture2D tex2d = (Texture2D) tex;
                float num = num = 3.05f / 255f;
                for (int i = 8; i < 16; i++) {
                    tex2d.SetPixel(i, 0, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 1, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 2, new Color(num, num, num, 1f));
                    tex2d.SetPixel(i, 3, new Color(num, num, num, 1f));
                }
                tex2d.Apply();
            }

            texUpdated = true;
            Patch.Debug("Updated segment-table of material!", BepInEx.Logging.LogLevel.Debug, true);
        }

        public static bool texDumped = false;
        public static bool texUpdated = false;
    }
}