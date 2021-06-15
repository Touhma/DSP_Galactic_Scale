using System.Collections.Generic;
using System.Linq;

namespace GalacticScale {

    public class GSStars : List<GSStar> {
        GS2.Random random = new GS2.Random(GSSettings.Seed);
        public GSStars(IEnumerable<GSStar> i) : base(i) { }
        public GSStars() : base() { }
        public new GSStar Add(GSStar star) {
            base.Add(star);
            return star;
        }
        public GSPlanets HabitablePlanets {
            get {
                var h = from star in this
                        from planet in star.Planets.Habitable
                        select planet;

                return new GSPlanets(h);
            }
        }
        public GSStar RandomStar {
            get {
                GS2.Warn($"Star Count:{this.Count}");
                if (this.Count == 1) {
                    return this[0];
                }

                return this[random.Next(this.Count)];
            }
        }
    }
}