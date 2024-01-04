using System;
using HarmonyLib;
using NGPT;
using UnityEngine;

namespace GalacticScale.Patches
{
    public partial class PatchOnGuideMissionStandardMode
    {
       

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GuideMissionStandardMode), nameof(GuideMissionStandardMode.CalculateCurve))]
        public static bool CalculateCurve(ref GuideMissionStandardMode __instance)
        {
            var localPlanet = GameMain.localPlanet;
            var localStar = GameMain.localStar;
            var uPositionOfBirthPlanet = localPlanet.uPosition;
            var uPositionOfFlyByPlanet = localPlanet.uPosition * 0.95;
            var uPositionOfStar = localStar.uPosition;
            PlanetData planetDataOfFlyByPlanet = null;
            var MoonAvoidDistance = 0.0;
            GS3.Warn($"LocalPlanet : {localPlanet.name}");
            //If Gas Giant Host
            for (var index = 0; index < localStar.planetCount; ++index)
                if (localStar.planets[index] == localPlanet.orbitAroundPlanet)
                {
                    planetDataOfFlyByPlanet = localStar.planets[index];
                    uPositionOfFlyByPlanet = localStar.planets[index].uPosition;
                    GS3.Warn($"Set Flyby Planet to {planetDataOfFlyByPlanet.name}");

                    break;
                }


            if (planetDataOfFlyByPlanet == null)
                if (localStar.planetCount > 1)
                {
                    var gsPlanet = GS3.GetGSPlanet(localPlanet);
                    if (gsPlanet.MoonCount > 1)
                    {
                        planetDataOfFlyByPlanet = gsPlanet.Moons[0].planetData;
                        uPositionOfFlyByPlanet = gsPlanet.Moons[0].planetData.uPosition;
                        GS3.Warn($"Set Flyby Planet to {gsPlanet.Moons[0].Name}");
                    }
                    else
                    {
                        var gsStar = GS3.GetGSStar(localPlanet.star);
                        GSPlanet closestPlanet = null;
                        var closestDistance = double.MaxValue;
                        foreach (var planet in gsStar.Planets)
                        {
                            if (planet == gsPlanet) continue;
                            var distance = (planet.planetData.uPosition - localPlanet.uPosition).magnitude;
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestPlanet = planet;
                            }
                        }

                        if (closestPlanet != null)
                        {
                            planetDataOfFlyByPlanet = closestPlanet.planetData;
                            uPositionOfFlyByPlanet = planetDataOfFlyByPlanet.uPosition;
                            GS3.Warn($"Set Flyby Planet to {closestPlanet.Name}");
                        }
                    }
                }

            if (planetDataOfFlyByPlanet == null) GS3.Warn("Failed to set Flyby Planet");
            //If Gas Giant has a moon to miss
            for (var index = 0; index < localStar.planetCount; ++index)
                if (localStar.planets[index].orbitAroundPlanet == planetDataOfFlyByPlanet)
                {
                    MoonAvoidDistance = localStar.planets[index].orbitRadius * 40000.0;
                    GS3.Warn($"Set MoonAvoid Distance to {MoonAvoidDistance}");
                }
            // if (localStar.planets[index].type != EPlanetType.Gas && localStar.planets[index].orbitAroundPlanet == planetDataOfFlyByPlanet)
            //     MoonAvoidDistance = localStar.planets[index].orbitRadius * 40000.0;

            MoonAvoidDistance += 2000.0;
            if (MoonAvoidDistance < 7000.0) MoonAvoidDistance = 7000.0;
            GS3.Warn($"Moon Avoid Distance = {MoonAvoidDistance}");
            Vector3 DirectionBetweenPlanets = (uPositionOfFlyByPlanet - uPositionOfBirthPlanet).normalized;
            Vector3 DirectionBetweenStarAndPlanet = (uPositionOfStar - uPositionOfFlyByPlanet).normalized;
            var OrbitDirection = localPlanet.runtimeOrbitRotation * Vector3.up;
            var Sign = Vector3.Dot(DirectionBetweenStarAndPlanet, Vector3.Cross(OrbitDirection, DirectionBetweenPlanets)) >= 0.0 ? 1f : -1f;
            var q = Quaternion.AngleAxis(60f * Sign, DirectionBetweenPlanets);
            var vectorLf3_2 = uPositionOfBirthPlanet * 0.6 + uPositionOfFlyByPlanet * 0.4 + Maths.QRotateLF(q, OrbitDirection * 3000f);
            Vector3 vector3_2 = uPositionOfBirthPlanet * 1.05 - uPositionOfFlyByPlanet * 0.05 + new VectorLF3(0.0f, 70f, 0.0f) - vectorLf3_2;
            var v = vectorLf3_2 - uPositionOfFlyByPlanet;
            var num4 = (float)((360.0 - Vector3.Angle(-DirectionBetweenPlanets, DirectionBetweenStarAndPlanet)) * 0.400000005960464);
            var num5 = num4 + num4 * 0.400000005960464;
            var num6 = (float)(num4 * (Math.PI / 180.0) * MoonAvoidDistance * 0.349999994039536);
            var vectorLf3_3 = Maths.QRotateLF(Quaternion.AngleAxis(num4 * Sign, OrbitDirection), v);
            var vectorLf3_4 = vectorLf3_3.normalized * MoonAvoidDistance + uPositionOfFlyByPlanet - (VectorLF3)OrbitDirection * MoonAvoidDistance * 0.5 * Mathf.Clamp01(num4 / 120f);
            var normalized3 = Vector3.Cross(vectorLf3_2 - vectorLf3_4, -vector3_2).normalized;
            var vector3_3 = Quaternion.AngleAxis((float)num5, normalized3) * vector3_2.normalized * num6;
            var vector3_4 = -vector3_3.normalized * 12000f;
            var vector3_5 = -vector3_2.normalized * (num6 * 0.7f);
            vectorLf3_3 = Maths.QRotateLF(Quaternion.AngleAxis(40f, Vector3.down * Sign), uPositionOfStar - vectorLf3_4);
            vectorLf3_3 = vectorLf3_3.normalized + new VectorLF3(0.0, 0.25, 0.0);
            var vectorLf3_5 = vectorLf3_3.normalized * 80000.0 + uPositionOfStar;
            vectorLf3_3 = uPositionOfStar - vectorLf3_5;
            var normalized4 = vectorLf3_3.normalized;
            vectorLf3_3 = uPositionOfStar - vectorLf3_4;
            var normalized5 = vectorLf3_3.normalized;
            vectorLf3_3 = normalized4 + normalized5;
            var vectorLf3_6 = vectorLf3_3.normalized * (localStar.physicsRadius * 0.800000011920929) + uPositionOfStar;
            vectorLf3_3 = vectorLf3_5 - vectorLf3_6;
            Vector3 normalized6 = vectorLf3_3.normalized;
            vectorLf3_3 = vectorLf3_6 - (vectorLf3_4 + (VectorLF3)vector3_4);
            Vector3 normalized7 = vectorLf3_3.normalized;
            var vector3_6 = (normalized6 + normalized7).normalized * 14000f;
            var vector3_7 = -vector3_6;
            vectorLf3_3 = vectorLf3_6 + (VectorLF3)vector3_6 - vectorLf3_5;
            Vector3 vector3_8 = vectorLf3_3.normalized * 24000.0;
            var vector3_9 = -vector3_8 * 0.15f;
            __instance.bezierObject = new GameObject("Bezier Path");
            __instance.bezierObject.transform.localPosition = Vector3.zero;
            __instance.bezierObject.transform.localRotation = Quaternion.identity;
            var bezierPath = __instance.bezierObject.AddComponent<BezierPath>();
            while (bezierPath.nodeCount < 4)
                bezierPath.InsertNode(0);
            var num7 = 1f;
            bezierPath.localLengthError = 0.1f;
            bezierPath.SetMiddleControlPoint(0, vectorLf3_5 * num7);
            bezierPath.SetBackTangent(0, vector3_9 * num7);
            bezierPath.SetForwardTangent(0, vector3_8 * num7);
            bezierPath.SetMiddleControlPoint(1, vectorLf3_6 * num7);
            bezierPath.SetBackTangent(1, vector3_6 * num7);
            bezierPath.SetForwardTangent(1, vector3_7 * num7);
            bezierPath.SetMiddleControlPoint(2, vectorLf3_4 * num7);
            bezierPath.SetBackTangent(2, vector3_4 * num7);
            bezierPath.SetForwardTangent(2, vector3_3 * num7);
            bezierPath.SetMiddleControlPoint(3, vectorLf3_2 * num7);
            bezierPath.SetBackTangent(3, vector3_5 * num7);
            bezierPath.SetForwardTangent(3, vector3_2 * num7);
            bezierPath.ValidateSamples();
            __instance.bezierPath = bezierPath;
            __instance.curveLength = bezierPath.length / 1f;
            __instance.curveDistance = 0.0f;
            __instance.curveSpeed = __instance.spaceCapsule.speedOverCurveTime.Evaluate(0.0f);
            __instance.curveSpeed *= 10000f;
            GS3.Warn($"CurveLength:{__instance.curveLength} CurveDistance:{__instance.curveDistance} CurveSpeed:{__instance.curveSpeed}");
            GS3.Warn($"GSSettings BirthPlanet:{GSSettings.BirthPlanet.Name} Galaxy BirthPlanet:{GS3.galaxy.PlanetById(GS3.galaxy.birthPlanetId).name} LocalPlanet:{localPlanet.name}");
            // GS3.WarnJson(bezierPath);
            //GS3.Warn(localPlanet.uPosition.ToString());
            return false;
        }
    }
}