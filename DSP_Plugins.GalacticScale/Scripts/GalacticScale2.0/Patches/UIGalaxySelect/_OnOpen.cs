using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {

        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        public static bool _OnOpen(UIGalaxySelect __instance)
        {
            if (GS2.generator == null)
            {
                return true;
            }

            if (StartButton == null)
            {
                StartButton = __instance.GetComponentInChildren<UIButton>().gameObject;
            }

            StartButton?.SetActive(true);
            GS2.Log(StartButton?.name);
            __instance.random = new DotNet35Random(GSSettings.Seed);//new DotNet35Random((int)(System.DateTime.Now.Ticks / 10000L));
            __instance.gameDesc = new GameDesc();
            __instance.gameDesc.SetForNewGame(UniverseGen.algoVersion, __instance.random.Next(100000000), GS2.generator.Config.DefaultStarCount, 1, 1f);
            GS2.gameDesc = __instance.gameDesc;
            __instance.starmapGroup.gameObject.SetActive(true);
            __instance.starmap._Open();
            __instance.SetStarmapGalaxy();
            return false;
        }

        public static GameObject StartButton;
    }
}