using System.Collections.Generic;

namespace GalacticScale
{
    public class SingletonExample //left here for future use
    {
        private SingletonExample()
        {
        }

        public static SingletonExample Instance => Internal.instance;

        private class Internal
        {
            internal static readonly SingletonExample instance = new();

            static Internal()
            {
            }
        }
    }


    public class VegeTypesDictionary
    {
        private VegeTypesDictionary()
        {
        }

        public static Dictionary<string, int> Instance => Internal.instance;

        //private static void init()
        //{
        //instance = new VegeTypesDictionary()
        ////{
        //    ["LandingPod"] = 9999,
        //    ["AlienStone1"] = 66,
        //    ["AlienStone2"] = 67,
        //    ["AlienStone3"] = 68,
        //    ["AlienStone4"] = 69,
        //    ["AlienStone5"] = 70,
        //    ["AlienStone6"] = 71,
        //    ["AlienStone7"] = 72,
        //    ["AlienStone8"] = 73,
        //    ["AlienStone9"] = 74,
        //    ["AlienStone10"] = 75,
        //    ["AlienStone11"] = 76,
        //    ["AlienStone12"] = 77,
        //    ["AlienStone13"] = 78,
        //    ["AlienStone14"] = 79,
        //    ["AlienStone15"] = 80,
        //    ["AlienStone16"] = 81,
        //    ["AlienStone17"] = 82,
        //    ["AlienStone18"] = 83,
        //    ["AlienStone19"] = 84,
        //    ["AlienStone20"] = 85,
        //    ["AlienStone21"] = 86,
        //    ["AlienStone22"] = 87,
        //    ["AlienStone23"] = 88,
        //    ["AlienStone24"] = 89,
        //    ["AlienStone25"] = 90,
        //    ["Grass0"] = 61,
        //    ["Grass1"] = 62,
        //    ["Grass2"] = 63,
        //    ["Iceberg1"] = 19,
        //    ["Iceberg2"] = 20,
        //    ["Iceberg3"] = 21,
        //    ["MedFragment6"] = 1005,
        //    ["MedGrass7"] = 1001,
        //    ["MedGrass8"] = 1002,
        //    ["MedGrass9"] = 1003,
        //    ["MedGrass10"] = 1004,
        //    ["MedStone1"] = 601,
        //    ["MedStone2"] = 602,
        //    ["MedStone3"] = 603,
        //    ["MedStone4"] = 604,
        //    ["MedStone5"] = 605,
        //    ["MedTree1"] = 101,
        //    ["MedTree2"] = 102,
        //    ["MedTree3"] = 103,
        //    ["MedTree4"] = 104,
        //    ["MedTree5"] = 105,
        //    ["MedTree6"] = 106,
        //    ["MedBroken7"] = 1006,
        //    ["MedBroken8"] = 1007,
        //    ["RedStoneGrass3"] = 1011,
        //    ["RedStoneGrass8"] = 1012,
        //    ["RedStoneGrass9"] = 1013,
        //    ["RedStoneTree1"] = 111,
        //    ["RedStoneTree2"] = 112,
        //    ["RedStoneTree4"] = 113,
        //    ["RedStoneTree5"] = 114,
        //    ["RedStoneTree6"] = 115,
        //    ["RedStoneTree7"] = 116,
        //    ["Stone1"] = 1,
        //    ["Stone2"] = 2,
        //    ["Stone3"] = 3,
        //    ["Stone4"] = 4,
        //    ["Stone5"] = 5,
        //    ["Stone6"] = 6,
        //    ["Stone7"] = 7,
        //    ["Stone8"] = 8,
        //    ["Stone9"] = 9,
        //    ["Stone10"] = 10,
        //    ["Stone11"] = 11,
        //    ["Stone12"] = 12,
        //    ["Stone13"] = 48,
        //    ["Stone14"] = 49,
        //    ["Stone15"] = 50,
        //    ["Stone16"] = 51,
        //    ["Stone17"] = 52,
        //    ["Stone18"] = 53,
        //    ["Stone19"] = 54,
        //    ["Stone20"] = 55,
        //    ["Stone21"] = 56,
        //    ["Stone22"] = 57,
        //    ["Stone23"] = 58,
        //    ["Stone24"] = 59,
        //    ["Tree1"] = 13,
        //    ["Tree2"] = 14,
        //    ["Tree3"] = 15,
        //    ["Tree4"] = 16,
        //    ["Tree5"] = 17,
        //    ["Tree6"] = 18,
        //    ["Tree7"] = 19,
        //    ["Tree8"] = 20,
        //    ["Tree9"] = 21,
        //    ["Tree10"] = 22,
        //    ["Tree11"] = 23,
        //    ["Tree12"] = 24,
        //    ["Tree13"] = 25,
        //    ["Tree14"] = 26,
        //    ["Tree15"] = 27,
        //    ["Tree16"] = 28,
        //    ["Tree17"] = 29,
        //    ["Tree18"] = 30,
        //    ["Tree19"] = 31,
        //    ["Tree20"] = 32,
        //    ["Tree21"] = 33,
        //    ["Tree22"] = 34,
        //    ["Tree23"] = 35,
        //    ["Tree24"] = 36,
        //    ["Tree25"] = 37,
        //    ["Tree26"] = 38,
        //    ["Tree27"] = 39,
        //    ["Tree28"] = 40,
        //    ["Tree29"] = 41,
        //    ["Tree30"] = 42,
        //    ["Tree31"] = 43,
        //    ["Tree32"] = 44,
        //    ["Tree33"] = 45,
        //    ["Tree34"] = 46,
        //    ["Tree35"] = 60,
        //    ["JungleTree1"] = 121,
        //    ["JungleTree2"] = 122,
        //    ["JungleTree3"] = 123,
        //    ["JungleTree4"] = 124,
        //    ["JungleTree5"] = 125,
        //    ["JungleTree6"] = 126,
        //    ["JungleGrass7"] = 1021,
        //    ["JungleGrass8"] = 1022,
        //    ["JungleGrass9"] = 1023
        //}
        //}
        public static int Find(string s)
        {
            if (s == null) GS2.Warn("Vege code null");
            if (Instance.ContainsKey(s)) return Instance[s];

            GS2.Warn("Vege ID for " + s + " not found");
            return 9999;
        }

        public static string Find(int i)
        {
            if (Instance.ContainsValue(i)) return Utils.ReverseLookup(Instance, i);

            return "LandingPod";
        }

        private class Internal
        {
            internal static readonly Dictionary<string, int> instance = new()
            {
                ["Stone1"] = 1,
                ["Stone2"] = 2,
                ["Stone3"] = 3,
                ["Stone4"] = 4,
                ["Stone5"] = 5,
                ["Stone6"] = 6,
                ["Stone7"] = 7,
                ["Stone8"] = 8,
                ["Stone9"] = 9,
                ["Stone10"] = 10,
                ["Stone11"] = 11,
                ["Stone12"] = 12,
                ["Tree1"] = 13,
                ["Tree2"] = 14,
                ["Tree3"] = 15,
                ["Tree4"] = 16,
                ["Tree5"] = 17,
                ["Tree6"] = 18,
                ["Iceberg1"] = 19,
                ["Iceberg2"] = 20,
                ["Iceberg3"] = 21,
                ["Tree7"] = 22,
                ["Tree8"] = 23,
                ["Tree9"] = 24,
                ["Tree10"] = 25,
                ["Tree11"] = 26,
                ["Tree12"] = 27,
                ["Tree13"] = 28,
                ["Tree14"] = 29,
                ["Tree15"] = 30,
                ["Tree16"] = 31,
                ["Tree17"] = 32,
                ["Tree20"] = 33,
                ["Tree20"] = 34,
                ["Tree21"] = 35,
                ["Tree22"] = 36,
                ["Tree23"] = 37,
                ["Tree25"] = 38,
                ["Tree26"] = 39,
                ["Tree27"] = 40,
                ["Tree28"] = 41,
                ["Tree29"] = 42,
                ["Tree30"] = 43,
                ["Tree31"] = 44,
                ["Tree32"] = 45,
                ["Tree33"] = 46,
                ["Tree34"] = 47,
                ["Stone13"] = 48,
                ["Stone14"] = 49,
                ["Stone15"] = 50,
                ["Stone16"] = 51,
                ["Stone17"] = 52,
                ["Stone18"] = 53,
                ["Stone19"] = 54,
                ["Stone20"] = 55,
                ["Stone21"] = 56,
                ["Stone22"] = 57,
                ["Stone23"] = 58,
                ["Stone24"] = 59,
                ["Tree35"] = 60,
                ["Grass0"] = 61,
                ["Grass1"] = 62,
                ["Grass2"] = 63,
                ["Tree18"] = 64,
                ["Tree24"] = 65,
                ["AlienStone1"] = 66,
                ["AlienStone2"] = 67,
                ["AlienStone3"] = 68,
                ["AlienStone4"] = 69,
                ["AlienStone5"] = 70,
                ["AlienStone6"] = 71,
                ["AlienStone7"] = 72,
                ["AlienStone8"] = 73,
                ["AlienStone9"] = 74,
                ["AlienStone10"] = 75,
                ["AlienStone11"] = 76,
                ["AlienStone12"] = 77,
                ["AlienStone13"] = 78,
                ["AlienStone14"] = 79,
                ["AlienStone15"] = 80,
                ["AlienStone16"] = 81,
                ["AlienStone17"] = 82,
                ["AlienStone18"] = 83,
                ["AlienStone19"] = 84,
                ["AlienStone20"] = 85,
                ["AlienStone21"] = 86,
                ["AlienStone22"] = 87,
                ["AlienStone23"] = 88,
                ["AlienStone24"] = 89,
                ["AlienStone25"] = 90,
                ["MedTree1"] = 101,
                ["MedTree2"] = 102,
                ["MedTree3"] = 103,
                ["MedTree4"] = 104,
                ["MedTree5"] = 105,
                ["MedTree6"] = 106,

                ["RedStoneTree1"] = 111,
                ["RedStoneTree2"] = 112,
                ["RedStoneTree4"] = 113,
                ["RedStoneTree5"] = 114,
                ["RedStoneTree6"] = 115,
                ["RedStoneTree7"] = 116,

                ["JungleTree1"] = 121,
                ["JungleTree2"] = 122,
                ["JungleTree3"] = 123,
                ["JungleTree4"] = 124,
                ["JungleTree5"] = 125,
                ["JungleTree6"] = 126,

                ["WhiteTree1"] = 131,
                ["WhiteTree2"] = 132,
                ["WhiteTree3"] = 133,
                ["WhiteTree4"] = 134,
                ["WhiteTree5"] = 135,
                ["WhiteTree6"] = 136,

                ["MedStone1"] = 601,
                ["MedStone2"] = 602,
                ["MedStone3"] = 603,
                ["MedStone4"] = 604,
                ["MedStone5"] = 605,

                ["DesertFragment1"] = 611,
                ["DesertFragment2"] = 612,
                ["DesertFragment3"] = 613,
                ["DesertFragment4"] = 614,
                ["DesertFragment5"] = 615,
                ["DesertFragment6"] = 616,
                ["DesertFragment7"] = 617,
                ["DesertFragment8"] = 618,
                ["DesertFragment9"] = 619,


                ["MedGrass7"] = 1001,
                ["MedGrass8"] = 1002,
                ["MedGrass9"] = 1003,
                ["MedGrass10"] = 1004,
                ["MedFragment6"] = 1005,
                ["MedFragment7"] = 1006,
                ["MedFragment8"] = 1007,
                ["MedBroken7"] = 1006,
                ["MedBroken8"] = 1007,
                ["RedStoneGrass3"] = 1011,
                ["RedStoneGrass8"] = 1012,
                ["RedStoneGrass9"] = 1013,

                ["JungleGrass7"] = 1021,
                ["JungleGrass8"] = 1022,
                ["JungleGrass9"] = 1023,

                ["Seaweed7"] = 1031,
                ["Seaweed8"] = 1032,
                ["Seaweed9"] = 1033,

                ["DesertFragment10"] = 1041,
                ["DesertFragment11"] = 1042,
                ["DesertFragment12"] = 1043,

                ["LandingPod"] = 9999
            };

            static Internal()
            {
            }
        }
    }
}