using System.Collections;
using UnityEngine;

namespace GalacticScale
{
    public class TeleportComponent : MonoBehaviour
    {
        public PlanetData TargetPlanet;
        public StarData TargetStar;
        public bool TeleportEnabled;

        public void FixedUpdate()
        {
            if (!GS2.Config.CheatMode) return;
            if (DSPGame.IsMenuDemo) return;
            if (TargetStar == null && TargetPlanet == null || TeleportEnabled == false || !(GameMain.localStar != null && GameMain.localStar.loaded)) return;
            if (TargetPlanet != null)
            {
                GS2.Warn($"TP to Planet {TargetPlanet?.name} of star {TargetPlanet?.star?.name}");

                GameMain.data.ArriveStar(TargetPlanet?.star);
                StartCoroutine(Teleport(TargetPlanet));
            }
            else if (TargetStar != null)
            {
                GS2.Warn($"TP to Star {TargetStar?.name}");
                GameMain.data.ArriveStar(TargetStar);
                StartCoroutine(Teleport(TargetStar));
            }
        }
        private IEnumerator Teleport(PlanetData planet)
        {
            yield return new WaitForEndOfFrame();
            TargetPlanet = null;
            TargetStar = null;
            GameMain.mainPlayer.uPosition = planet.uPosition + VectorLF3.unit_z * planet.realRadius;
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            TeleportEnabled = false;
            GameMain.mainPlayer.transform.localScale = Vector3.one;
        }

        private IEnumerator Teleport(StarData star)
        {
            yield return new WaitForEndOfFrame();
            TargetPlanet = null;
            TargetStar = null;
            GameMain.mainPlayer.uPosition = star.uPosition + VectorLF3.unit_z * 1;
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            TeleportEnabled = false;
            GameMain.mainPlayer.transform.localScale = Vector3.one;
            GameCamera.instance.FrameLogic();
        }

        public bool NavArrowClick(UIStarmap starmap)
        {
            if (starmap.focusStar != null && VFInput.control && GS2.Config.CheatMode)
            {
                GS2.TP.TargetStar = starmap.focusStar.star;
                GS2.TP.TeleportEnabled = true;
                return false;
            }

            if (starmap.focusPlanet != null && VFInput.control && GS2.Config.CheatMode)
            {
                GS2.TP.TargetPlanet = starmap.focusPlanet.planet;
                GS2.TP.TeleportEnabled = true;
                return false;
            }

            return true;
        }
    }
}