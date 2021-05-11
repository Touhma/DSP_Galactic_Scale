namespace GalacticScale
{
    public class GSVeinParams
    {
        public float count;
        public float density;
        public float richness;
        public GSVeinParams(float count, float density, float richness)
        {
            this.count = count;
            this.density = density;
            this.richness = richness;
        }
        public GSVeinParams()
        {

        }
        public static GSVeinParams Random(int seed = -1)
        {
            //This needs to be calibrated, I'm guessing count/density/richness are probably greater than 0-0.999
            if (seed < 0) seed = GSSettings.Seed;
            System.Random random = new System.Random(seed);
            return new GSVeinParams((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        }
    }
}