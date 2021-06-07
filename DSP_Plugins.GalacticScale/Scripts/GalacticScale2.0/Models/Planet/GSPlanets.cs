using System.Collections.Generic;
using System.Linq;
namespace GalacticScale
{
    public class GSPlanets : List<GSPlanet>
    {
        public GSPlanets() : base() { }
        public GSPlanets(IEnumerable<GSPlanet> planets) : base(planets) { }
        public new GSPlanet Add(GSPlanet gsPlanet)
        {
            base.Add(gsPlanet);
            return gsPlanet;
        }
        public GSPlanets Habitable
        {
            get
            {
                var h = from planet in this
                        where planet.isHabitable
                        select planet;
                return new GSPlanets(h);
            }
        }
    }
}