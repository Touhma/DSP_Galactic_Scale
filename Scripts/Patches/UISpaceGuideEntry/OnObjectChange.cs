namespace GalacticScale
{
    public class PatchOnUISpaceGuideEntry
    {
        //[HarmonyPostfix, HarmonyPatch(typeof(UISpaceGuideEntry), "OnObjectChange")]
        //public static void OnObjectChange(ESpaceGuideType ___guideType, float ___radius, int ___objId, GalaxyData ___galaxy, ref RectTransform ___rectTrans, ref Text ___nameText, ref Image ___markIcon) {
        //    if (___guideType == ESpaceGuideType.Planet) {
        //        PlanetData planetData = ___galaxy.PlanetById(___objId);
        //        if (planetData.scale < 1) {
        //            Object.DestroyImmediate(___rectTrans.GetComponent<Text>().);
        //        }
        //    }
        //}
        //}
    }
}