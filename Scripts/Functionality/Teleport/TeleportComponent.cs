using System.Collections;
using UnityEngine;

namespace GalacticScale
{
    public class TeleportComponent : MonoBehaviour
    {
        public bool TeleportEnabled;
        public int ticker;
        public PlanetData TargetPlanet;
        public StarData TargetStar;

        public void FixedUpdate()
        {
            ticker++;
            if (ticker > 60)
                // GS3.Warn(
                // $"FU: TP:{TargetPlanet != null} TS:{TargetStar != null} TE:{TeleportEnabled} LS:{GameMain.localStar != null} LSL{GameMain.localStar?.loaded}");
                ticker = 0;
            else return;
            if (!GS3.Config.CheatMode) return;
            if (DSPGame.IsMenuDemo) return;
            if (TargetStar == null && TargetPlanet == null || TeleportEnabled == false || !(GameMain.localStar != null && GameMain.localStar.loaded)) return;
            if (TargetPlanet != null)
            {
                GS3.Warn($"TP to Planet {TargetPlanet?.name} of star {TargetPlanet?.star?.name}");

                GameMain.data.ArriveStar(TargetPlanet?.star);
                StartCoroutine(Teleport(TargetPlanet));
            }
            else if (TargetStar != null)
            {
                GS3.Warn($"TP to Star {TargetStar?.name}");
                GameMain.data.ArriveStar(TargetStar);
                StartCoroutine(Teleport(TargetStar));
            }
        }

        private IEnumerator Teleport(PlanetData planet)
        {
            GS3.Warn($"Teleporting to {planet.name}");
            yield return new WaitForEndOfFrame();
            GameMain.mainPlayer.uPosition = planet.uPosition + VectorLF3.unit_z * planet.realRadius;
            yield return new WaitForEndOfFrame();
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            yield return new WaitForEndOfFrame();
            GameCamera.instance.FrameLogic();
            yield return new WaitForEndOfFrame();
            GameMain.mainPlayer.transform.localScale = Vector3.one * GS3.Config.MechaScale;
            if (GameMain.localPlanet == planet)
            {
                TargetPlanet = null;
                TargetStar = null;
                TeleportEnabled = false;
            }
        }

        private IEnumerator Teleport(StarData star)
        {
            yield return new WaitForEndOfFrame();
            // yield return new WaitForSeconds(0.2f);

            GameMain.mainPlayer.uPosition = star.uPosition + VectorLF3.unit_z * 1;
            yield return new WaitForEndOfFrame();
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            yield return new WaitForEndOfFrame();
            GameMain.mainPlayer.transform.localScale = Vector3.one;
            yield return new WaitForEndOfFrame();
            GameCamera.instance.FrameLogic();
            yield return new WaitForEndOfFrame();
            GameMain.mainPlayer.transform.localScale = Vector3.one * GS3.Config.MechaScale;
            if (GameMain.localStar == star)
            {
                TargetPlanet = null;
                TargetStar = null;
                TeleportEnabled = false;
            }
        }

        public bool NavArrowClick(UIStarmap starmap)
        {
            if (starmap.focusStar != null && VFInput.control && GS3.Config.CheatMode)
            {
                GS3.TP.TargetStar = starmap.focusStar.star;
                GS3.TP.TeleportEnabled = true;
                return false;
            }

            if (starmap.focusPlanet != null && VFInput.control && GS3.Config.CheatMode)
            {
                GS3.TP.TargetPlanet = starmap.focusPlanet.planet;
                GS3.TP.TeleportEnabled = true;
                return false;
            }

            return true;
        }
    }
}