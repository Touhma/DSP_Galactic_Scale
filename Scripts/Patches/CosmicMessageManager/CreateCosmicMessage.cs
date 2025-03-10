using HarmonyLib;
using UnityEngine;
using System;

namespace GalacticScale
{
    public partial class PatchOnCosmicMessageManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CosmicMessageManager), "CreateCosmicMessage")]
        public static bool CreateCosmicMessage(ref CosmicMessageData __result, CosmicMessageManager __instance, int id, int msgSeed)
        {
            GS2.Log($"=== CreateCosmicMessage called ===");
            GS2.Log($"id: {id}, msgSeed: {msgSeed}, maxProtoId: {CosmicMessageProto.maxProtoId}");

            DotNet35Random rnd = new DotNet35Random(msgSeed);
            CosmicMessageData msg = new CosmicMessageData();
            CosmicMessageProto proto = LDB.cosmicMessages.Select(id);

            GS2.Log($"Proto lookup result: {(proto != null ? $"ID={proto.ID}" : "null")}");

            // Special handling for communicators (id=5)
            bool isCommunicator = false;//(id == 5);

            if (proto == null && id <= CosmicMessageProto.maxProtoId && !isCommunicator)
            {
                GS2.Log("Early return: proto is null and id <= maxProtoId (not communicator)");
                __result = msg;
                return false;
            }

            msg.protoId = id;
            msg.seed = msgSeed;

            // Communicators and messages beyond maxProtoId are handled specially
            if (id > CosmicMessageProto.maxProtoId)
            {
                isCommunicator = true;
                GS2.Log($"Processing special cosmic message (id > maxProtoId or communicator)");
                msg.doodadProtoId = isCommunicator ? 5 : msg.doodadProtoId;
                DoodadProto doodadProto = LDB.doodads.Select(msg.doodadProtoId);
                int birthStarId = GameMain.data.galaxy.birthStarId;
                StarData birthStar = GameMain.data.galaxy.StarById(birthStarId);

                GS2.Log($"Birth star: ID={birthStarId}, Name={birthStar?.name ?? "null"}");

                double scaleFactor = rnd.NextDouble() * 0.30000000000000004 + 1.2;
                double radius = (double)birthStar.systemRadius * 40000.0 * scaleFactor;
                if (doodadProto != null)
                {
                    radius += (rnd.NextDouble() - 0.5) * 2.0 * doodadProto.RadiusRange;
                }

                double theta = rnd.NextDouble() * 360.0;
                double phi = rnd.NextDouble() * 360.0;
                double thetaRad = 0.01745329238474369 * theta;
                double phiRad = 0.01745329238474369 * phi;

                GS2.Log($"Calculating position: theta={theta}, phi={phi}, radius={radius}");

                double x = Math.Sin(thetaRad) * Math.Cos(phiRad);
                double y = Math.Sin(thetaRad) * Math.Sin(phiRad);
                double z = Math.Cos(thetaRad);

                msg.birthPosition = new VectorLF3(x, y, z) * radius + birthStar.uPosition;
                GS2.Log($"Set birthPosition={msg.birthPosition} BirthStar Position={birthStar.uPosition}");
            }
            else
            {
                GS2.Log("Processing normal cosmic message");
                msg.birthPosition = proto.Position;
                msg.doodadProtoId = rnd.Next(1, DoodadProto.maxProtoId);
                GS2.Log($"Set birthPosition={msg.birthPosition}, doodadProtoId={msg.doodadProtoId}");
            }

            msg.nearStar = null;
            StarData[] stars = GameMain.data.galaxy.stars;
            GS2.Log($"Searching through {stars.Length} stars for nearest star");

            // For communicators, we want to find the birth star
            if (isCommunicator)
            {
                int birthStarId = GameMain.data.galaxy.birthStarId;
                msg.nearStar = GameMain.data.galaxy.StarById(birthStarId);
                if (msg.nearStar != null)
                {
                    VectorLF3 delta = msg.birthPosition - msg.nearStar.uPosition;
                    msg.center = msg.nearStar.uPosition;
                    msg.orbitRadius = delta.magnitude;
                    double orbitRadiusAU = msg.orbitRadius / 40000.0;
                    msg.orbitalPeriod = Math.Sqrt(39.47841760435743 * orbitRadiusAU * orbitRadiusAU * orbitRadiusAU / (1.3538551990520382E-06 * (double)msg.nearStar.mass));
                    
                    float dotProduct = Vector3.Dot(delta.normalized, Vector3.right);
                    msg.orbitPhase = Math.Acos(dotProduct) * 6.283185307179586;
                    if (delta.z > 0.0)
                    {
                        msg.orbitPhase *= -1.0;
                    }

                    double inclination = delta.y / Math.Sqrt(delta.x * delta.x + delta.z * delta.z);
                    msg.orbitInclination = Math.Atan(inclination) * 6.283185307179586;

                    GS2.Log($"Communicator assigned to birth star: Name={msg.nearStar.name}, OrbitRadius={msg.orbitRadius}, Period={msg.orbitalPeriod}");
                }
            }
            else
            {
                // Normal star search for non-communicator messages
                for (int i = 0; i < stars.Length; i++)
                {
                    VectorLF3 delta = msg.birthPosition - stars[i].uPosition;
                    double maxDistSqr = 1.21 * (double)stars[i].systemRadius * (double)stars[i].systemRadius * 40000.0 * 40000.0;

                    if (delta.sqrMagnitude <= maxDistSqr)
                    {
                        msg.nearStar = stars[i];
                        float dotProduct = Vector3.Dot(delta.normalized, Vector3.right);
                        msg.orbitPhase = Math.Acos(dotProduct) * 6.283185307179586;
                        if (delta.z > 0.0)
                        {
                            msg.orbitPhase *= -1.0;
                        }

                        msg.center = stars[i].uPosition;
                        msg.orbitRadius = delta.magnitude;
                        double orbitRadiusAU = msg.orbitRadius / 40000.0;
                        msg.orbitalPeriod = Math.Sqrt(39.47841760435743 * orbitRadiusAU * orbitRadiusAU * orbitRadiusAU / (1.3538551990520382E-06 * (double)stars[i].mass));

                        double inclination = delta.y / Math.Sqrt(delta.x * delta.x + delta.z * delta.z);
                        msg.orbitInclination = Math.Atan(inclination) * 6.283185307179586;

                        GS2.Log($"Found near star: Name={stars[i].name}, OrbitRadius={msg.orbitRadius}, Period={msg.orbitalPeriod}");
                        break;
                    }
                }
            }

            if (msg.nearStar == null)
            {
                GS2.Log("No near star found, using default orbital parameters");
                DoodadProto doodadProto = LDB.doodads.Select(msg.doodadProtoId);
                if (doodadProto != null)
                {
                    double speed = doodadProto.BaseSpeed + (rnd.NextDouble() - 0.5) * 2.0 * doodadProto.SpeedRange;
                    msg.orbitRadius = doodadProto.BaseRadius + (rnd.NextDouble() - 0.5) * 2.0 * doodadProto.RadiusRange;
                    msg.orbitalPeriod = 2.0 * msg.orbitRadius * 3.141592653589793 / speed;
                    GS2.Log($"Using doodad parameters: Speed={speed}, Radius={msg.orbitRadius}, Period={msg.orbitalPeriod}");
                }

                msg.orbitPhase = rnd.NextDouble() * 360.0 - 0.1;
                msg.orbitInclination = (rnd.NextDouble() - 0.5) * 2.0 * 89.9;
                msg.center = msg.birthPosition - new VectorLF3(-msg.orbitRadius, 0.0, 0.0);
            }

            // Calculate birth rotation
            double rx = (rnd.NextDouble() - 0.5) * 2.0;
            double ry = (rnd.NextDouble() - 0.5) * 2.0;
            double rz = (rnd.NextDouble() - 0.5) * 2.0;
            int attempts = 10;

            while (attempts > 0 && rx * rx + ry * ry + rz * rz > 1.0)
            {
                rx = (rnd.NextDouble() - 0.5) * 2.0;
                ry = (rnd.NextDouble() - 0.5) * 2.0;
                rz = (rnd.NextDouble() - 0.5) * 2.0;
                attempts--;
            }

            double angle = rnd.NextDouble() * 360.0;
            VectorLF3 rotAxis = new VectorLF3(rx, ry, rz).normalized;
            msg.birthRotation = Quaternion.AngleAxis((float)angle, rotAxis) * Quaternion.LookRotation(rotAxis);

            GS2.Log($"Final message state: ProtoId={msg.protoId}, DoodadId={msg.doodadProtoId}, NearStar={msg.nearStar?.name ?? "null"}");

            __result = msg;
            return false;
        }
    }
} 