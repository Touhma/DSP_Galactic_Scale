using System.Collections.Generic;

namespace GalacticScale
{
    public class GSPlanets : List<GSPlanet>
    {
        public GSPlanets() : base() { }
        public GSPlanets(GSPlanets planets) : base(planets) { }
        public new GSPlanet Add(GSPlanet gsPlanet)
        {
            base.Add(gsPlanet);
            return gsPlanet;
        }
    }
}