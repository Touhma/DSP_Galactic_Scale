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
                // ["JungleTree3"] = 123,
                ["JungleTree3"] = 122,
                ["JungleTree4"] = 124,
                ["JungleTree5"] = 125,
                ["JungleTree6"] = 126,
                ["WhiteTree1"] = 131,
                ["WhiteTree2"] = 132,
                ["WhiteTree3"] = 133,
                ["WhiteTree4"] = 134,
                ["WhiteTree5"] = 135,
                ["WhiteTree6"] = 136,
                ["PandoraTree1"] = 141,
                ["PandoraTree2"] = 142,
                ["PandoraTree3"] = 143,
                ["PandoraTree4"] = 144,
                ["PandoraTree5"] = 145,
                ["PandoraTree6"] = 146,
                ["PandoraTree7"] = 147,
                ["PandoraTree8"] = 148,
                ["PandoraTree9"] = 149,
                ["PandoraTree10"] = 150,
                ["PandoraTree11"] = 151,
                ["PrairieDesertTree1"] = 161,
                ["PrairieDesertTree2"] = 162,
                ["PrairieDesertTree3"] = 163,
                ["PrairieDesertTree4"] = 164,
                ["PrairieDesertTree5"] = 165,
                ["PrairieDesertTree6"] = 166,
                ["PrairieDesertTree7"] = 167,
                ["PrairieDesertTree8"] = 168,
                ["PrairieDesertTree9"] = 169,
                ["PrairieDesertTree10"] = 170,
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

                ["AshenPermafrost1"] = 621,
                ["AshenPermafrost2"] = 622,
                ["AshenPermafrost3"] = 623,
                ["AshenPermafrost4"] = 624,
                ["AshenPermafrost5"] = 625,
                ["AshenPermafrost6"] = 626,
                ["AshenPermafrost7"] = 627,
                ["AshenPermafrost8"] = 628,
                ["AshenPermafrost9"] = 629,

                ["LavaStone1"] = 631,
                ["LavaStone2"] = 632,
                ["LavaStone3"] = 633,
                ["LavaStone4"] = 634,
                ["LavaStone5"] = 635,
                ["LavaStone6"] = 636,
                ["LavaStone7"] = 637,
                ["LavaStone8"] = 638,
                ["LavaStone9"] = 639,
                ["LavaStone10"] = 640,
                ["LavaStone11"] = 641,
                ["LavaStone12"] = 642,
                ["LavaStone13"] = 643,
                ["LavaStone14"] = 644,
                ["LavaStone15"] = 645,
                ["LavaStone16"] = 646,
                ["LavaStone17"] = 647,

                ["HurricaneStone1"] = 651,
                ["HurricaneStone2"] = 652,
                ["HurricaneStone3"] = 653,
                ["HurricaneStone4"] = 654,
                ["HurricaneStone5"] = 655,
                ["HurricaneStone6"] = 656,
                ["HurricaneStone7"] = 657,
                ["HurricaneStone8"] = 658,
                ["HurricaneStone9"] = 659,

                ["IceFieldFrozenSoilIce1"] = 661,
                ["IceFieldFrozenSoilIce2"] = 662,
                ["IceFieldFrozenSoilIce3"] = 663,
                ["IceFieldFrozenSoilIce4"] = 664,
                ["IceFieldFrozenSoilIce5"] = 665,
                ["IceFieldFrozenSoilIce6"] = 666,
                ["IceFieldFrozenSoilIce7"] = 667,
                ["IceFieldFrozenSoilIce8"] = 668,
                ["IceFieldFrozenSoilIce9"] = 669,
                ["IceFieldFrozenSoilIce10"] = 670,
                ["IceFieldFrozenSoilIce11"] = 671,

                ["Pandora'sStone1"] = 681,
                ["Pandora'sStone2"] = 682,
                ["Pandora'sStone3"] = 683,
                ["Pandora'sStone4"] = 684,
                ["Pandora'sStone5"] = 685,

                ["PrairieDesertStone1"] = 691,
                ["PrairieDesertRock2"] = 692,
                ["PrairieDesertRock3"] = 693,
                ["PrairieDesertStone4"] = 694,

                ["OrangeCrystalDesertStone1"] = 701,
                ["OrangeCrystalDesertStone2"] = 702,
                ["OrangeCrystalDesertStone3"] = 703,
                ["OrangeCrystalDesertStone4"] = 704,
                ["OrangeCrystalDesertStone5"] = 705,
                ["OrangeCrystalDesertStone6"] = 706,
                ["OrangeCrystalDesertStone7"] = 707,
                ["OrangeCrystalDesertStone8"] = 708,
                ["OrangeCrystalDesertStone9"] = 709,
                ["OrangeCrystalDesertStone10"] = 710,
                ["OrangeCrystalDesertStone11"] = 711,
                ["OrangeCrystalDesertStone12"] = 712,
                ["OrangeCrystalDesertStone13"] = 713,

                ["Arctictundra1"] = 721,
                ["ArcticTundra2"] = 722,
                ["ArcticTundra3"] = 723,
                ["ArcticTundra4"] = 724,
                ["ArcticTundra5"] = 725,
                ["Arctictundra6"] = 726,
                ["ArcticTundra7"] = 727,
                ["Arctictundra8"] = 728,
                ["Arctictundra9"] = 729,
                ["Arctictundra10"] = 730,
                ["Arctictundra11"] = 731,
                ["Arctictundra12"] = 732,
                ["Arctictundra13"] = 733,
                ["Arctictundra14"] = 734,

                ["ScarletIceStone1"] = 741,
                ["ScarletIceStone2"] = 742,
                ["ScarletIceStone3"] = 743,
                ["ScarletIceStone4"] = 744,
                ["ScarletIceStone5"] = 745,
                ["ScarletIceStone6"] = 746,
                ["ScarletIceStone7"] = 747,
                ["ScarletIceStone8"] = 748,
                ["ScarletIceStone9"] = 749,
                ["ScarletIceStone10"] = 750,

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

                ["AridDesertSpecialFX1"] = 1044,

                ["AshenPermafrost10"] = 1051,
                ["AshenPermafrost11"] = 1052,
                ["AshenPermafrost12"] = 1053,
                ["AshenPermafrost13"] = 1054,
                ["AshenPermafrost14"] = 1055,

                ["HurricaneStone10"] = 1061,
                ["HurricaneStone11"] = 1062,
                ["HurricaneStone12"] = 1063,
                ["HurricaneStone13"] = 1064,
                ["HurricaneStone14"] = 1065,
                ["HurricaneStone15"] = 1066,

                ["IceFieldFrozenIce12"] = 1071,
                ["IceFieldFrozenIce13"] = 1072,
                ["IceFieldFrozenIce14"] = 1073,
                ["IceFieldFrozenIce15"] = 1074,
                ["Pandora1"] = 1081,
                ["Pandora2"] = 1082,
                ["Pandora3"] = 1083,
                ["Pandora4"] = 1084,
                ["Pandora5"] = 1085,
                ["Pandora6"] = 1086,
                ["PrairieDesertGrass1"] = 1091,
                ["PrairieDesertGrass2"] = 1092,
                ["PrairieDesertGrass3"] = 1093,
                ["PrairieDesertGrass4"] = 1094,
                ["LandingPod"] = 9999
            };

            static Internal()
            {
            }
        }
    }
}