namespace GalacticScale
{
    public class GSVein
    {
        public int count;

        public float richness;

        //public Vector3 position;
        public GSVein(int count, float richness) //, Vector3 position)
        {
            this.richness = richness;
            //this.position = position;
            this.count = count;
        }

        //public GSVein(int count, float richness)
        //{
        //    this.count = count;
        //    this.richness = richness;
        //    //position = Vector3.zero;
        //}
        public GSVein(GSPlanet gsPlanet, int seed = -1)
        {
            if (seed < 0) seed = GSSettings.Seed;

            var random = new GS2.Random(seed);
            richness = (float)random.NextDouble() * gsPlanet.planetData.star.resourceCoef;
            count = random.Next(1, 30);
            //position = Vector3.zero;
        }

        public GSVein()
        {
            richness = -1;
            count = -1;
            //position = Vector3.zero;
        }

        public GSVein Clone()
        {
            return (GSVein)MemberwiseClone();
        }
    }
}