using System.Collections;
using UnityEngine;

namespace GalacticScale
{

    public class GSTeleportHandler : MonoBehaviour
    {
        public static PlanetData planet = null;

        private void Update()
        {
            if (planet == null) return;
            StartCoroutine(Teleport(planet));
            
        }

        private IEnumerator Teleport(PlanetData planet)
        {
            yield return new WaitForEndOfFrame();
            GameMain.mainPlayer.uPosition = planet.uPosition + VectorLF3.unit_z * planet.realRadius;
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            planet = null;
        }
    }
}