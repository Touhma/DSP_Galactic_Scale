using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static Dictionary<int, int[]> keyedLUTs = new Dictionary<int, int[]>();

        public static void SetLuts(int segments, float planetRadius)
        {
            if (!DSPGame.IsMenuDemo && !Vanilla)
            {
                // Prevent special LUT's being created in main menu
                if (keyedLUTs.ContainsKey(segments) && keyedLUTs.ContainsKey(segments) &&
                    PatchOnUIBuildingGrid.LUT512.ContainsKey(segments)) return;
                        Warn("Creating LUT for Segment Count:" + segments + ", planetRadius:" + planetRadius);
        var numSegments =
            segments / 4; //Number of segments on a quarter circle (the other 3/4 will result by mirroring)
        Warn("numSegments:" + numSegments);
        var lut = new int[numSegments];
        var segmentAngle =
            Math.PI / 2f / numSegments; //quarter circle divided by num segments is the angle per segment
        Warn("Segment Angle:" + segmentAngle);
        // float lastMajorRadius = 0;
        // Warn($"Last Major Radius:{lastMajorRadius}");
        var lastMajorRadiusCount = segments;
        Warn("Last Major Radius Count:" + lastMajorRadiusCount);

        var classicLUT = new int[512];
        classicLUT[0] = 1;
        bool onlyhalve = false;
        bool lastnothalf = false;
        for (var cnt = 0; cnt < numSegments; cnt++)
        {
            var ringradius =
                Math.Cos(cnt * segmentAngle) *
                planetRadius; //cos of the nth segment is the x-distance of the point in a 2d circle
            var classicIdx =
                (int)Math.Ceiling(Math.Abs(Math.Cos((float)((cnt + 1) / (segments / 4f) * Math.PI * 0.5))) *
                                  segments);
            var tilesize = ringradius / lastMajorRadiusCount;

            //If the new radius is smaller than 90% of the currently used radius, use it as the new segment count to avoid tile squishing
            // if (ringradius < (0.9 * lastMajorRadius))
            // {
            //     lastMajorRadius = ringradius;
            //     lastMajorRadiusCount = (int)(ringradius / 4.0) * 4;
            // }
            var a = 1f / 2f;
            var b = 5f / 8f;
            var c = 4f / 5f;
            var d = 2f / 3f;
            var e = 3f / 4f;
            var f = 5f / 6f;

            var acount = lastMajorRadiusCount * a;
            var bcount = lastMajorRadiusCount * b;
            var ccount = lastMajorRadiusCount * c;
            var dcount = lastMajorRadiusCount * d;
            var ecount = lastMajorRadiusCount * e;
            var fcount = lastMajorRadiusCount * f;

            var availratios = new List<float>();
            if (acount % 4 == 0) availratios.Add(a);
            if (bcount % 4 == 0) availratios.Add(b);
            if (ccount % 4 == 0) availratios.Add(c);
            if (dcount % 4 == 0) availratios.Add(d);
            if (ecount % 4 == 0) availratios.Add(e);
            if (fcount % 4 == 0) availratios.Add(f);
            var bestCount = lastMajorRadiusCount;
            Log("Available Ratios For Segment " + cnt + ":");

            foreach (var ratio in availratios)
            {
                if (tilesize > 0.9f && tilesize < 1.1f) continue;
                var proposedcount = lastMajorRadiusCount * ratio;
                var halfResult = proposedcount * Math.Pow(0.5f, numSegments - cnt);
                if (halfResult> 1)
                {
                    Warn($">1! Where Remaining Circles:{numSegments-cnt} and result = {proposedcount * Math.Pow(0.5f, numSegments - cnt)}");
                    continue;
                }
                Warn($"result = {proposedcount * Math.Pow(0.5f, numSegments - cnt)}");

                var proposedTileSize = ringradius / proposedcount;
                Warn("Proposed (" + proposedcount + ") Tilesize for Ratio " + ratio + " is " + proposedTileSize +$"vs {tilesize} currently  used");
                if (Math.Abs(proposedTileSize - 1f) < Math.Abs(tilesize - 1f) &&
                    (proposedcount < 40 || proposedcount % 20 == 0) && (!onlyhalve || ratio == 0.5f))
                {
                    Log("Proposed Count " + proposedcount + " gives better tilesize (" + proposedTileSize +
                        ") than current (" + tilesize + "). Switching to ratio +" + ratio);
                    tilesize = proposedTileSize;
                    bestCount = (int)proposedcount;
                }


                if (halfResult == 1) lastnothalf = true;
            }

            if (onlyhalve)
            {
                bestCount = lastMajorRadiusCount / 2;
            }

            if (lastnothalf) onlyhalve = true;
            lastMajorRadiusCount = bestCount;
            // if (cnt == numSegments - 1) lastMajorRadiusCount = 4;
            lut[cnt] = lastMajorRadiusCount;
            Log(" " + classicIdx );
            classicLUT[classicIdx] = lastMajorRadiusCount;
            Log($"{cnt} RadiusCount = " + lastMajorRadiusCount);
        }
        
        
        if (lut[numSegments - 1] == 2)
        {
            Warn("Yup");
            int[] offsetLUT = new int[numSegments];
            for (var i = 0; i < lut.Length - 1; i++) offsetLUT[i + 1] = lut[i];
            offsetLUT[0] = offsetLUT[1];
            int[] offsetClassicLUT = new int[512];
            for (var i = 0; i < classicLUT.Length - 1; i++) offsetClassicLUT[i + 1] = classicLUT[i];
            offsetClassicLUT[0] = offsetClassicLUT[1];
            lut = offsetLUT;
            classicLUT = offsetClassicLUT;
        }

                if (segments != 200)
                {
                    var last = 1;
                    for (var oldlLutIdx = 1; oldlLutIdx < 512; oldlLutIdx++)
                        if (classicLUT[oldlLutIdx] > last)
                        {
                            //Offset of 1 is required to avoid mismatch of some longitude circles
                            var temp = classicLUT[oldlLutIdx];
                            classicLUT[oldlLutIdx] = last;
                            last = temp;
                        }
                        else
                        {
                            classicLUT[oldlLutIdx] = last;
                        }
                }
                else
                {
                    lut = new[]
                    {
                        4, 8, 16, 20, 32, 32, 40, 40, 60, 60, 60, 80, 80, 80, 100, 100, 100, 100, 100, 120, 120, 120,
                        120, 120, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 200, 200, 200, 200, 200, 200, 200,
                        200, 200, 200, 200, 200, 200, 200, 200, 200
                    };
                    classicLUT = new int[512]
                    {
                        1,
                        4,
                        4,
                        4,
                        4,
                        4,
                        4,
                        4,
                        8,
                        8,
                        8,
                        8,
                        8,
                        8,
                        8,
                        8,
                        16,
                        16,
                        16,
                        16,
                        20,
                        20,
                        20,
                        20,
                        20,
                        20,
                        20,
                        20,
                        32,
                        32,
                        32,
                        32,
                        32,
                        32,
                        32,
                        32,
                        32,
                        32,
                        32,
                        32,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        40,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        60,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        80,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        100,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        120,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        160,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        200,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        240,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        300,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        400,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500,
                        500
                    };
                }

                //Fill all Look Up Tables (Dictionaries really)
                if (!keyedLUTs.ContainsKey(segments)) keyedLUTs.Add(segments, lut);
                if (!keyedLUTs.ContainsKey(segments)) keyedLUTs.Add(segments, lut);
                if (!PatchOnUIBuildingGrid.LUT512.ContainsKey(segments))
                    PatchOnUIBuildingGrid.LUT512.Add(segments, classicLUT);
                LogJson(lut);
                LogJson(classicLUT);
            }
        }
    }
}