using System.Collections.Generic;
using System.Linq;

namespace GalacticScale
{
    public class GSStars : List<GSStar>
    {
        private readonly GS3.Random random = new(GSSettings.Seed);

        public GSStars(IEnumerable<GSStar> i) : base(i)
        {
        }

        public GSStars()
        {
            random = new GS3.Random(GSSettings.Seed);
        }

        public GSPlanets HabitablePlanets
        {
            get
            {
                var h = from star in this from planet in star.Planets.Habitable select planet;

                return new GSPlanets(h);
            }
        }

        public GSStar RandomStar
        {
            get
            {
                //GS3.Warn($"Star Count:{this.Count} random:{random.Id}");
                if (Count == 1) return this[0];
                if (Count == 0) GS3.Error("No Stars To Pick From!");
                return this[random.Next(Count)];
            }
        }

        public new GSStar Add(GSStar star)
        {
            base.Add(star);
            return star;
        }
    }
}