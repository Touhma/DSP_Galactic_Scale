using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnTrashSystem
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrashSystem), "Gravity")]
        public static bool Gravity(
            ref bool __result,
            ref TrashData trash,
            AstroPose[] astroPoses,
            double dt,
            int testStarId,
            double testStarGravity,
            int localPlanetId,
            PlanetRawData localPlanetData)
        {
            // GS2.Warn("Trash");
            var flag1 = true;
            trash.nearPlanetId = 0;
            var zero1 = VectorLF3.zero;
            var nearStarId = trash.nearStarId;
            if (nearStarId > 0)
            {
                var num1 = 0.0;
                var zero2 = VectorLF3.zero;
                var flag2 = false;
                for (var index = nearStarId + 1; index <= nearStarId + ((GameMain.localStar != null)?GameMain.localStar.planetCount:8); ++index)
                {
                    double uRadius = astroPoses[index].uRadius;
                    if (uRadius >= 1.0)
                    {
                        var vectorLf3_1 = new VectorLF3(astroPoses[index].uPos.x - trash.uPos.x,
                            astroPoses[index].uPos.y - trash.uPos.y, astroPoses[index].uPos.z - trash.uPos.z);
                        var num2 = Math.Sqrt(vectorLf3_1.x * vectorLf3_1.x + vectorLf3_1.y * vectorLf3_1.y +
                                             vectorLf3_1.z * vectorLf3_1.z);
                        var num3 = uRadius / num2;
                        if (num3 <= 1.5 && num3 >= 0.05)
                        {
                            var num4 = 1.0;
                            if (num2 < 800.0)
                            {
                                num4 = Math.Pow(10.0, (800.0 - num2) / 150.0);
                                flag2 = true;
                                trash.nearPlanetId = index;
                            }

                            var num5 = uRadius + 0.35;
                            if (index == localPlanetId && num2 < 210.0)
                            {
                                Vector3 vpos = Maths.QInvRotateLF(astroPoses[index].uRot,
                                    new Vector3((float) -vectorLf3_1.x, (float) -vectorLf3_1.y,
                                        (float) -vectorLf3_1.z));
                                num5 = localPlanetData.QueryModifiedHeight(vpos) + 0.15;
                                if (num5 < uRadius + 0.15)
                                    num5 = uRadius + 0.15;
                            }

                            if (num2 < num5)
                            {
                                if (num5 > 600.0)
                                {
                                    flag1 = false;
                                }
                                else
                                {
                                    var v1 = -vectorLf3_1;
                                    var v2 = Maths.QInvRotateLF(astroPoses[index].uRot, v1);
                                    var vectorLf3_2 = (Maths.QRotateLF(astroPoses[index].uRotNext, v2) +
                                        astroPoses[index].uPosNext - trash.uPos) * 60.0;
                                    var vectorLf3_3 = trash.uVel - vectorLf3_2;
                                    var normalized1 = v1.normalized;
                                    var vectorLf3_4 = normalized1;
                                    if (index == localPlanetId)
                                    {
                                        var normalized2 = Maths.QInvRotate(astroPoses[index].uRot, vectorLf3_3)
                                            .normalized;
                                        RaycastHit hitInfo;
                                        if (Physics.Raycast(new Ray((Vector3) v2 - normalized2, normalized2),
                                            out hitInfo, 3f, 512))
                                            vectorLf3_4 = Maths.QRotateLF(astroPoses[index].uRot, hitInfo.normal)
                                                .normalized;
                                    }

                                    if (vectorLf3_3.magnitude > 1.0)
                                    {
                                        var num6 = -vectorLf3_4.x * vectorLf3_3.x - vectorLf3_4.y * vectorLf3_3.y -
                                                   vectorLf3_4.z * vectorLf3_3.z;
                                        var vectorLf3_5 = vectorLf3_4 * num6;
                                        var vectorLf3_6 = vectorLf3_3 + vectorLf3_5;
                                        trash.uPos = astroPoses[index].uPos + normalized1 * (num5 + 0.005);
                                        trash.uVel = vectorLf3_5 * 0.349999994039536 + vectorLf3_6 * 0.925000011920929 +
                                                     vectorLf3_2;
                                    }
                                    else
                                    {
                                        trash.landPlanetId = index;
                                        trash.uPos = astroPoses[index].uPos + normalized1 * (num5 + 0.005);
                                        trash.uVel = VectorLF3.zero;
                                        trash.lPos = v2;
                                    }
                                }
                            }

                            var num7 = 50.0 * num3 * num3 * num4 / num2;
                            zero2.x += vectorLf3_1.x * num7;
                            zero2.y += vectorLf3_1.y * num7;
                            zero2.z += vectorLf3_1.z * num7;
                            num1 += num4;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (!flag2)
                {
                    double uRadius = astroPoses[nearStarId].uRadius;
                    var vectorLf3 = new VectorLF3(astroPoses[nearStarId].uPos.x - trash.uPos.x,
                        astroPoses[nearStarId].uPos.y - trash.uPos.y, astroPoses[nearStarId].uPos.z - trash.uPos.z);
                    var d = vectorLf3.x * vectorLf3.x + vectorLf3.y * vectorLf3.y + vectorLf3.z * vectorLf3.z;
                    var num2 = Math.Sqrt(d);
                    var num3 = trash.nearStarGravity / num2 / d;
                    zero2.x += vectorLf3.x * num3;
                    zero2.y += vectorLf3.y * num3;
                    zero2.z += vectorLf3.z * num3;
                    ++num1;
                    if (num2 > 2400000.0)
                    {
                        trash.nearPlanetId = 0;
                        trash.nearStarId = 0;
                        trash.nearStarGravity = 0.0;
                    }
                    else if (num2 < uRadius)
                    {
                        trash.SetEmpty();
                        flag1 = false;
                    }
                }

                var num8 = dt / num1;
                trash.uVel.x += zero2.x * num8;
                trash.uVel.y += zero2.y * num8;
                trash.uVel.z += zero2.z * num8;
            }
            else if (testStarId > 0)
            {
                var vectorLf3 = new VectorLF3(astroPoses[testStarId].uPos.x - trash.uPos.x,
                    astroPoses[testStarId].uPos.y - trash.uPos.y, astroPoses[testStarId].uPos.z - trash.uPos.z);
                if (vectorLf3.x * vectorLf3.x + vectorLf3.y * vectorLf3.y + vectorLf3.z * vectorLf3.z < 4000000000000.0)
                {
                    trash.nearStarId = testStarId;
                    trash.nearStarGravity = testStarGravity;
                }
            }

            __result = flag1;
            return false;
        }
    }
}