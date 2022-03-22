using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GalacticScale;

public static partial class GS2
{
    public static Dictionary<int, int[]> keyedLUTs = new();

    public static void SetLuts(int segments, float planetRadius)
    {
        if (!DSPGame.IsMenuDemo && !Vanilla)
        {
            Log($"Setting LUTS for {planetRadius}");
            // Prevent special LUT's being created in main menu
            if (keyedLUTs.ContainsKey(segments) && PatchOnUIBuildingGrid.LUT512.ContainsKey(segments)) return;
            var numSegments = segments / 4; //Number of segments on a quarter circle (the other 3/4 will result by mirroring)
            var lut = new int[numSegments];
            var segmentAngle = Mathf.PI / 2f / numSegments; //quarter circle divided by num segments is the angle per segment


            // var lastMajorRadius = planetRadius;
            // var lastMajorRadiusCount = segments;
            // var lastMajorGridSize = 1f;
            // var lastCount = segments;
            var classicLUT = new int[512];
            classicLUT[0] = 1;
            Dictionary<int, float> RingRadii = new();
            Dictionary<int, float> Circumferences = new();
            Dictionary<int, int> SegmentCount = new();
            Dictionary<int, float> GridSizes = new();

            float StartGridSize(int index, int segmentCount = -1)
            {
                if (segmentCount < 0) segmentCount = SegmentCount[index];
                return Circumferences[index] / segmentCount / 5;
            }

            float EndGridSize(int index, int segmentCount)
            {
                var circumference = Mathf.PI * Mathf.Cos((index + 1) * segmentAngle) * planetRadius * 2;
                return circumference / segmentCount / 5;
            }
            for (var index = 0; index < numSegments; index++)
            {
                if (index == 0)
                {
                    RingRadii.Add(0, planetRadius);
                    Circumferences.Add(0, planetRadius * 2 * Mathf.PI);
                    SegmentCount.Add(0, segments);
                    GridSizes.Add(0, StartGridSize(0));
                    lut[index] = SegmentCount[index];
                    Log(GridSizes[0].ToString());
                    continue;
                }

                RingRadii.Add(index, Mathf.Cos(index * segmentAngle) * planetRadius); //cos of the nth segment is the x-distance of the point in a 2d circle
                Circumferences.Add(index, Mathf.PI * RingRadii[index] * 2); // = Mathf.PI * diameter;
                SegmentCount.Add(index, SegmentCount[index - 1]);
                var gridSize = StartGridSize(index);
                Log($"Calculated gridSize:{gridSize} for planetRadius:{planetRadius} cnt:{index}");
                if (EndGridSize(index, SegmentCount[index]) < 0.9f)
                {
                    var origSegCount = SegmentCount[index];
                    var ratios = new float[6]

                    {
                        0.5f,
                        0.625f,
                        0.66666667f,
                        0.75f,
                        0.8f,
                        0.833333333f
                    };
                    var i = 0;
                    gridSize = 2f; //arbitrarily high number so while loop works
                    var maxGridSize = 1.35f;
                    while (gridSize > GridSizes[0])
                    {
                        ///Trying for perfect ratios first 5:6, 4:5, 3:4, 2:3, 5:8, 1:2

                        if (SegmentCount[index] % 6 == 0 && StartGridSize(index, SegmentCount[index] / 6 * 5) < maxGridSize)
                        {
                            Log("Found Perfect Ratio 6/5");
                            SegmentCount[index] = SegmentCount[index] / 6 * 5;
                            GridSizes.Add(index, StartGridSize(index));
                            
                        }
                        else if (SegmentCount[index] % 5 == 0 && StartGridSize(index, SegmentCount[index] / 5 * 4) < maxGridSize)
                        {
                            Log("Found Perfect Ratio 5/4");
                            SegmentCount[index] = SegmentCount[index] / 5 * 4;
                            GridSizes.Add(index, StartGridSize(index));
                        }
                        else if (SegmentCount[index] % 4 == 0 && StartGridSize(index, SegmentCount[index] / 4 * 3) < maxGridSize)
                        {
                            Log("Found Perfect Ratio 4/3");
                            SegmentCount[index] = SegmentCount[index] / 4 * 3;
                            GridSizes.Add(index, StartGridSize(index));
                        }
                        else if (SegmentCount[index] % 3 == 0 && StartGridSize(index, SegmentCount[index] / 3 * 2) < maxGridSize)
                        {
                            Log("Found Perfect Ratio 3/2");
                            SegmentCount[index] = SegmentCount[index] / 3 * 2;
                            GridSizes.Add(index, StartGridSize(index));
                        }
                        else if (SegmentCount[index] % 8 == 0 && StartGridSize(index, SegmentCount[index] / 8 * 5) < maxGridSize)
                        {
                            Log("Found Perfect Ratio 8/5");
                            SegmentCount[index] = SegmentCount[index] / 8 * 5;
                            GridSizes.Add(index, StartGridSize(index));
                        }
                        else if (SegmentCount[index] % 2 == 0 && StartGridSize(index, SegmentCount[index] / 2) < maxGridSize)
                        {
                            Log("Found Perfect Ratio 2/1");
                            SegmentCount[index] /= 2;
                            GridSizes.Add(index, StartGridSize(index));
                        }
                        /// 
                        if (GridSizes.ContainsKey(index))
                        {
                            break;
                        }
                        
                        Log($"Somehow that failed. Trying Ratio {ratios[i]}");
                        SegmentCount[index] = Mathf.CeilToInt(origSegCount * ratios[i]/4f) * 4;
                        gridSize = StartGridSize(index);
                        Log($"GridSize:{gridSize}");
                        if (SegmentCount[index] < 8)
                        {
                            SegmentCount[index] = 4;
                            break;
                        }

                        i++;
                        if (i > 5)
                        {
                            Error("Couldn't find a ratio that worked!");
                            Log($"{origSegCount} {origSegCount*ratios[0]} {origSegCount*ratios[1]} {origSegCount*ratios[2]} {origSegCount*ratios[3]} {origSegCount*ratios[4]} {origSegCount*ratios[5]}");
                            break;
                        }
                    }
                }

                Log($"Final gridSize:{gridSize} for planetRadius:{planetRadius} cnd:{index} segCount:{SegmentCount[index]}");

                var classicIdx = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float)((index + 1) / (segments / 4f) * Math.PI * 0.5))) * segments);

                //If the new radius is smaller than 90% of the currently used radius, use it as the new segment count to avoid tile squishing
                // if (gridSize < 1.1f)
                // {
                //     lastMajorRadius = ringradius;
                //     lastMajorRadiusCount = (int)(ringradius / 4.0f) * 4;
                // }
                //
                lut[index] = SegmentCount[index];
                // lastCount = lut[index];
                classicLUT[classicIdx] = SegmentCount[index];
                if (index == numSegments - 1)

                {
                    lut[index] = 4;
                    classicLUT[classicIdx] = 4;
                }
                if (index == numSegments - 2)
                {
                    lut[index] = 8;
                    classicLUT[classicIdx] = 8;
                }
            }

            Warn($"Huh {numSegments - 1} {lut.Length} {segments} {planetRadius}");

            Warn("Huh");
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

            if (segments == 200 && false)
            {
                lut = new[]
                {
                    // 4, 8, 16, 20, 32, 32, 40, 40, 60, 60, 60, 80, 80, 80, 100, 100, 100, 100, 100, 120, 120, 120,
                    // 120, 120, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 200, 200, 200, 200, 200, 200, 200,
                    // 200, 200, 200, 200, 200, 200, 200, 200, 200

                    200, 200, 200, 200, 200, 200, 200,
                    200, 200, 200, 200, 200, 200, 200, 200, 200, 160, 160, 160, 160, 160, 160, 160, 160, 160, 160, 120, 120,
                    120, 120, 120, 100, 100, 100, 100, 100, 80, 80, 80, 60, 60, 60, 40, 40, 32, 32, 20, 16, 8, 4
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
            if (!PatchOnUIBuildingGrid.LUT512.ContainsKey(segments)) PatchOnUIBuildingGrid.LUT512.Add(segments, classicLUT);
        }
    }
}