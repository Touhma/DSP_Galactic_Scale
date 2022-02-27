using System.Collections.Generic;
using System.Linq;

namespace GalacticScale
{
    public class GSPlanets : List<GSPlanet>
    {
        public GSPlanets()
        {
        }

        public GSPlanets(IEnumerable<GSPlanet> planets) : base(planets)
        {
        }

        public GSPlanets Habitable
        {
            get
            {
                var h = from planet in this where planet.IsHabitable select planet;
                return new GSPlanets(h);
            }
        }

        public new GSPlanet Add(GSPlanet gsPlanet)
        {
            base.Add(gsPlanet);
            return gsPlanet;
        }
    }
}