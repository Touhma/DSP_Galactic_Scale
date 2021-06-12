﻿using UnityEngine;

namespace GalacticScale {

    public static class StarDefaults {
        private static GS2.Random r;
        public static float Age(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole:
                case EStarType.NeutronStar:
                case EStarType.WhiteDwarf: return r.Range(1.0f, 1.4f);
                case EStarType.GiantStar:
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(0.2f, 0.9f);
                        case ESpectrType.B: return r.Range(0.2f, 0.99f);
                        case ESpectrType.F: return r.Range(0.2f, 0.9f);
                        case ESpectrType.G: return r.Range(0.2f, 0.9f);
                        case ESpectrType.K: return r.Range(0.1f, 0.99f);
                        case ESpectrType.M: return r.Range(0.02f, 0.5f);
                        case ESpectrType.O: return r.Range(0.2f, 0.9f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float Mass(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(48f, 75f);
                case EStarType.NeutronStar: return r.Range(1.7f, 3.4f);
                case EStarType.WhiteDwarf: return r.Range(0.24f, 1.15f);
                case EStarType.GiantStar:
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(1.74f, 4.25f);
                        case ESpectrType.B: return r.Range(4.29f, 20.0f);
                        case ESpectrType.F: return r.Range(1.2f, 1.7f);
                        case ESpectrType.G: return r.Range(0.850f, 1.230f);
                        case ESpectrType.K: return r.Range(0.61f, 0.9f);
                        case ESpectrType.M: return r.Range(0.38f, 0.61f);
                        case ESpectrType.O: return r.Range(21.0f, 53.0f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }

        public static float Color(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(0.9f, 0.97f);
                case EStarType.NeutronStar: return r.Range(0.83f, 0.89f);
                case EStarType.WhiteDwarf: return r.Range(0.02f, 0.75f);
                case EStarType.GiantStar:
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(0.60f, 0.799f);
                        case ESpectrType.B: return r.Range(0.8f, 0.999f);
                        case ESpectrType.F: return r.Range(0.4f, 0.599f);
                        case ESpectrType.G: return r.Range(0.2f, 0.399f);
                        case ESpectrType.K: return r.Range(0.0f, 0.199f);
                        case ESpectrType.M: return 0f;
                        case ESpectrType.O: return 1f;
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float Luminosity(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(0.005f, 0.0075f);
                case EStarType.NeutronStar: return r.Range(0.27f, 0.44f);

                case EStarType.WhiteDwarf: return r.Range(0.03f, 0.095f);
                case EStarType.GiantStar:
                    switch (s) {
                        case ESpectrType.A:
                        case ESpectrType.F:
                        case ESpectrType.G:
                        case ESpectrType.K: return r.Range(1.1f, 2f);

                        case ESpectrType.B:
                        case ESpectrType.O: return r.Range(9f, 11f);

                        case ESpectrType.M: return r.Range(0.7f, 1f);
                    }
                    break;
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(1.475f, 2.76f);
                        case ESpectrType.B: return r.Range(2.77f, 8.3f);
                        case ESpectrType.F: return r.Range(1.14f, 1.44f);
                        case ESpectrType.G: return r.Range(0.890f, 1.140f);
                        case ESpectrType.K: return r.Range(0.7f, 1.8f);
                        case ESpectrType.M: return r.Range(0.51f, 0.71f);
                        case ESpectrType.O: return r.Range(8.4f, 16.1f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float Lifetime(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            //r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(17f, 36f);
                case EStarType.NeutronStar: return r.Range(1043f, 1111f);

                case EStarType.WhiteDwarf: return r.Range(6800f, 8100f);
                case EStarType.GiantStar:
                    switch (s) {
                        case ESpectrType.K:
                        case ESpectrType.A:
                        case ESpectrType.F:
                        case ESpectrType.G: return r.Range(6000f, 8000f);
                        case ESpectrType.B:
                        case ESpectrType.O: return r.Range(25f, 30f);
                        case ESpectrType.M: return r.Range(9600 - 11300f, 2f);
                    }
                    break;
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(309.0f, 1920.0f);
                        case ESpectrType.B: return r.Range(33f, 354f);
                        case ESpectrType.F: return r.Range(1800f, 6200f);
                        case ESpectrType.G: return r.Range(5450f, 16000f);
                        case ESpectrType.K: return r.Range(9800f, 36000f);
                        case ESpectrType.M: return r.Range(20000f, 97000f);
                        case ESpectrType.O: return r.Range(8.4f, 35f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float Radius(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(3.45f, 4.6f);
                case EStarType.NeutronStar: return r.Range(0.37f, 0.52f);
                case EStarType.WhiteDwarf: return r.Range(0.19f, 0.45f);
                case EStarType.GiantStar: return r.Range(12f, 31f);
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(1.1f, 2.0f);
                        case ESpectrType.B: return r.Range(1.575f, 3.75f);
                        case ESpectrType.F: return r.Range(0.950f, 1.4f);
                        case ESpectrType.G: return r.Range(0.83f, 1.230f);
                        case ESpectrType.K: return r.Range(0.67f, 1.00f);
                        case ESpectrType.M: return r.Range(0.577f, 0.796f);
                        case ESpectrType.O: return r.Range(3.0f, 5.6f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float DysonRadius(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(0.69f, 0.76f);
                case EStarType.NeutronStar: return r.Range(0.85f, 1.03f);
                case EStarType.WhiteDwarf: return r.Range(0.24f, 0.55f);
                case EStarType.GiantStar:
                    switch (s) {
                        case ESpectrType.A:
                        case ESpectrType.F:
                        case ESpectrType.G:
                        case ESpectrType.K: return r.Range(0.79f, 0.8f);
                        case ESpectrType.B:
                        case ESpectrType.O: return r.Range(3f, 3f);
                        case ESpectrType.M: return r.Range(0.75f, 1f);
                    }
                    break;
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(0.44f, 0.60f);
                        case ESpectrType.B: return r.Range(0.59f, 0.8f);
                        case ESpectrType.F: return r.Range(0.325f, 0.545f);
                        case ESpectrType.G: return r.Range(0.260f, 0.330f);
                        case ESpectrType.K: return r.Range(0.239f, 0.264f);
                        case ESpectrType.M: return r.Range(0.229f, 0.239f);
                        case ESpectrType.O: return r.Range(0.8f, 0.93f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float Temperature(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return 0f;
                case EStarType.NeutronStar: return r.Range(10000000f, 99000000f);
                case EStarType.WhiteDwarf: return r.Range(120000f, 178000f); ;
                case EStarType.GiantStar:
                    switch (s) {
                        case ESpectrType.A:
                        case ESpectrType.F:
                        case ESpectrType.G:
                        case ESpectrType.K: return r.Range(3000f, 3600f);
                        case ESpectrType.B:
                        case ESpectrType.O: return r.Range(10000f, 15000f);
                        case ESpectrType.M: return r.Range(2200f, 3400f);
                    }
                    break;
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(7820.0f, 13000.0f);
                        case ESpectrType.B: return r.Range(13000f, 31000.0f);
                        case ESpectrType.F: return r.Range(6350f, 7700f);
                        case ESpectrType.G: return r.Range(5300f, 6400f);
                        case ESpectrType.K: return r.Range(4400f, 5300f);
                        case ESpectrType.M: return r.Range(3550f, 4400f);
                        case ESpectrType.O: return r.Range(31000f, 53000f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float OrbitScaler(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(2.48f, 2.73f);

                case EStarType.NeutronStar: return r.Range(3f, 3.7f);
                case EStarType.WhiteDwarf: return r.Range(0.86f, 1.95f);
                case EStarType.GiantStar:
                    switch (s) {
                        case ESpectrType.A:
                        case ESpectrType.F:
                        case ESpectrType.G:
                        case ESpectrType.M:
                        case ESpectrType.K: return r.Range(2.6f, 3f);

                        case ESpectrType.B:
                        case ESpectrType.O: return r.Range(8f, 11f);
                    }
                    break;
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(1.59f, 2.120f);
                        case ESpectrType.B: return r.Range(2.1f, 2.8f);
                        case ESpectrType.F: return r.Range(1.16f, 1.55f);
                        case ESpectrType.G: return r.Range(0.940f, 1.160f);
                        case ESpectrType.K: return r.Range(0.85f, 0.94f);
                        case ESpectrType.M: return r.Range(0.8194f, 0.854f);
                        case ESpectrType.O: return r.Range(2.8f, 3.3f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float LightBalanceRadius(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            //  r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(2.0f, 2.5f);
                case EStarType.NeutronStar: return r.Range(11f, 15f);
                case EStarType.WhiteDwarf: return r.Range(0.09f, 0.66f);
                case EStarType.GiantStar:
                    switch (s) {
                        case ESpectrType.A:
                        case ESpectrType.F:
                        case ESpectrType.G:
                        case ESpectrType.K: return r.Range(19f, 20f);
                        case ESpectrType.B:
                        case ESpectrType.O: return r.Range(150f, 160f);
                        case ESpectrType.M: return r.Range(15f, 17f);
                    }
                    break;
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.A: return r.Range(2.28f, 3.8f);
                        case ESpectrType.B: return r.Range(3.75f, 6.3f);
                        case ESpectrType.F: return r.Range(1.3f, 2.2f);
                        case ESpectrType.G: return r.Range(0.760f, 1.3f);
                        case ESpectrType.K: return r.Range(0.45f, 0.76f);
                        case ESpectrType.M: return r.Range(0.346f, 0.449f);
                        case ESpectrType.O: return r.Range(6.4f, 8.4f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }

        public static float ClassFactor(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;

            //  r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return r.Range(1.0f, 1.35f);
                case EStarType.NeutronStar: return r.Range(0.54f, 0.95f);
                case EStarType.WhiteDwarf: return r.Range(-3.4f, 0.22f);
                case EStarType.GiantStar:
                case EStarType.MainSeqStar:
                    switch (s) {
                        case ESpectrType.O: return r.Range(1.5f, 2.0f);
                        case ESpectrType.B: return r.Range(0.5f, 1.5f);
                        case ESpectrType.A: return r.Range(-0.5f, 0.50f);
                        case ESpectrType.F: return r.Range(-1.5f, -0.50f);
                        case ESpectrType.G: return r.Range(-2.5f, -1.5f);
                        case ESpectrType.K: return r.Range(-3.5f, -2.5f);
                        case ESpectrType.M: return r.Range(-4f, -3.5f);
                    }
                    break;
                default: break;
            }
            return 0f;
        }
        public static float HabitableRadius(GSStar star) {
            EStarType t = star.Type;
            ESpectrType s = star.Spectr;
            var habitableRadius = Mathf.Pow(1.7f, 2f + ClassFactor(star)) + 0.25f * Mathf.Min(1f, OrbitScaler(star));
            //  r = new GS2.Random(star.Seed);
            switch (t) {
                case EStarType.BlackHole: return 0f;
                case EStarType.NeutronStar: return 0f;
                case EStarType.WhiteDwarf: return 0.15f * (float)(r.Range(0f, 1f) * 0.4 + 0.8);
                case EStarType.GiantStar: return 9f * habitableRadius;
                default: break;
            }
            return habitableRadius;
        }
        public static GSStar Random(int seed = -1, string name = null, bool special = false) {
            //if (seed == -1) seed = GS2.random.Next();
            if (seed == -1) {
                seed = GSSettings.Seed;
            }

            if (name == null) {
                name = "Star-" + seed;
            }

            int rand = r.Next(7);
            ESpectrType spectr = special ? ESpectrType.X : (ESpectrType)rand;
            EStarType type = special ? (GS2.random.Next(10) > 5) ? EStarType.GiantStar : (GS2.random.Next(10) <= 1) ? EStarType.WhiteDwarf : (GS2.random.Next(10) <= 5) ? EStarType.BlackHole : EStarType.NeutronStar : EStarType.MainSeqStar;
            GSStar star = new GSStar(seed, name, spectr, type, new GSPlanets());
            return star;
        }
    }

}