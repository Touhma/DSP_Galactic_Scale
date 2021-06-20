using System;
using System.Collections.Generic;

namespace GalacticScale
{
    public static class StarPositions
    {
        public static List<VectorLF3> tmp_poses;
        public static List<VectorLF3> tmp_drunk;
        public static int GenerateTempPoses(int seed, int targetCount, int iterCount, double minDist, double minStepLen, double maxStepLen, double flatten)
        {
            if (tmp_poses == null)
            {
                tmp_poses = new List<VectorLF3>();
                tmp_drunk = new List<VectorLF3>();
            }
            else
            {
                tmp_poses.Clear();
                tmp_drunk.Clear();
            }
            if (iterCount < 1)
            {
                iterCount = 1;
            }
            else if (iterCount > 16)
            {
                iterCount = 16;
            }

            RandomPoses(seed, targetCount * iterCount, minDist, minStepLen, maxStepLen, flatten);
            for (int index = tmp_poses.Count - 1; index >= 0; --index)
            {
                if (index % iterCount != 0)
                {
                    tmp_poses.RemoveAt(index);
                }

                if (tmp_poses.Count <= targetCount)
                {
                    break;
                }
            }
            return tmp_poses.Count;
        }
        private static void RandomPoses(int seed, int maxCount, double minDist, double minStepLen, double maxStepLen, double flatten)
        {
            GS2.Random random = new GS2.Random(GSSettings.Seed);
            double num1 = random.NextDouble();
            tmp_poses.Add(VectorLF3.zero);
            int num2 = 6;
            int num3 = 8;
            if (num2 < 1)
            {
                num2 = 1;
            }

            if (num3 < 1)
            {
                num3 = 1;
            }

            int num4 = (int)(num1 * (num3 - num2) + num2);
            int max = 256;
            //if (GS2.gameDesc.starCount > 512) max = 512;
            if (GS2.gameDesc.starCount > 1024)
            {
                max = 1024;
            }

            for (int index = 0; index < num4; ++index)
            {
                int num5 = 0;

                while (num5++ < max)
                {
                    double num6 = random.NextDouble() * 2.0 - 1.0;
                    double num7 = (random.NextDouble() * 2.0 - 1.0) * flatten;
                    double num8 = random.NextDouble() * 2.0 - 1.0;
                    double num9 = random.NextDouble();
                    double d = num6 * num6 + num7 * num7 + num8 * num8;
                    if (d <= 1.0 && d >= 1E-08)
                    {
                        double num10 = Math.Sqrt(d);
                        double num11 = (num9 * (maxStepLen - minStepLen) + minDist) / num10;
                        VectorLF3 pt = new VectorLF3(num6 * num11, num7 * num11, num8 * num11);
                        if (!Utils.CheckStarCollision(tmp_poses, pt, minDist))
                        {
                            tmp_drunk.Add(pt);
                            tmp_poses.Add(pt);
                            if (tmp_poses.Count >= maxCount)
                            {
                                return;
                            }

                            break;
                        }
                    }
                }
            }
            int num12 = 0;



            while (num12++ < max)
            {
                for (int index = 0; index < tmp_drunk.Count; ++index)
                {
                    if (random.NextDouble() <= 0.7)
                    {
                        int num5 = 0;
                        while (num5++ < 256)
                        {
                            double num6 = random.NextDouble() * 2.0 - 1.0;
                            double num7 = (random.NextDouble() * 2.0 - 1.0) * flatten;
                            double num8 = random.NextDouble() * 2.0 - 1.0;
                            double num9 = random.NextDouble();
                            double d = num6 * num6 + num7 * num7 + num8 * num8;
                            if (d <= 1.0 && d >= 1E-08)
                            {
                                double num10 = Math.Sqrt(d);
                                double num11 = (num9 * (maxStepLen - minStepLen) + minDist) / num10;
                                VectorLF3 pt = new VectorLF3(tmp_drunk[index].x + num6 * num11, tmp_drunk[index].y + num7 * num11, tmp_drunk[index].z + num8 * num11);
                                if (!Utils.CheckStarCollision(tmp_poses, pt, minDist))
                                {
                                    tmp_drunk[index] = pt;
                                    tmp_poses.Add(pt);
                                    if (tmp_poses.Count >= maxCount)
                                    {
                                        return;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}