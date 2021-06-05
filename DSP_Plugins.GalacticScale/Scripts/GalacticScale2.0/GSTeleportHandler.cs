using System.Collections;
using UnityEngine;

namespace GalacticScale
{

    public class GSTeleportHandler : MonoBehaviour
    {
        public static PlanetData planet = null;
        public static StarData star = null;

        private void Update()
        {
            if (planet == null && star == null) return;
            if (planet != null) StartCoroutine(Teleport(planet));
            else if (star != null) StartCoroutine(Teleport(star));
            
        }

        private IEnumerator Teleport(PlanetData planet)
        {
            yield return new WaitForEndOfFrame();
            GameMain.mainPlayer.uPosition = planet.uPosition + VectorLF3.unit_z * planet.realRadius;
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            planet = null;
        }
        private IEnumerator Teleport(StarData star)
        {
            yield return new WaitForEndOfFrame();
            GameMain.mainPlayer.uPosition = star.uPosition + VectorLF3.unit_z * 100;
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            planet = null;
        }
    }
}