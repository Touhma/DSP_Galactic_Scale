using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnUIStarmap
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmap), "OnCursorFunction3Click")]
        public static bool OnCursorFunction3Click(
            //StarData ___viewStar, 
            UIStarmapStar ___focusStar,
            //UIStarmapStar ___mouseHoverStar, 
            //PlanetData ___viewPlanet, 
            //UIStarmapPlanet ___mouseHoverPlanet, 
            UIStarmapPlanet ___focusPlanet)
        {
            //var go = GameObject.Find("UI Root/Overlay Canvas/In Game/Starmap UIs/starmap-screen-ui/cursor-view/bg");
            //string mhs = (___mouseHoverStar == null) ? "null" : ___mouseHoverStar.star.name;
            //string fs = (___focusStar == null) ? "null" : ___focusStar.star.name;
            //string vs = (___viewStar == null) ? "null" : ___viewStar.name;
            //string vp = (___viewPlanet == null) ? "null" : ___viewPlanet.name;
            //string mhp = (___mouseHoverPlanet == null) ? "null" : ___mouseHoverPlanet.planet.name;
            //string fp = (___focusPlanet == null) ? "null" : ___focusPlanet.planet.name;
            //GS2.Warn(
            //    "MouseHoverStar:" + mhp +
            //    " focusStar:" + fs +
            //    " ViewStar:" + vs +
            //    " ViewPlanet:" + vp +
            //    " mouseHoverPlanet:" + mhp +
            //    " focusPlanet:" + fp);
            if (___focusStar != null && VFInput.control && GS2.Config.CheatMode)
            {
                Bootstrap.TeleportStar = ___focusStar.star;
                Bootstrap.TeleportEnabled = true;
                return false;
            }

            if (___focusPlanet != null && VFInput.control && GS2.Config.CheatMode)
            {
                Bootstrap.TeleportPlanet = ___focusPlanet.planet;
                Bootstrap.TeleportEnabled = true;
                return false;
            }

            return true;
        }
    }
}