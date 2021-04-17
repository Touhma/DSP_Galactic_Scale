using System.Collections.Generic;

namespace GalacticScale
{
    public static class settings
    {
        public static List<star> Stars= new List<star>();
        public static int starCount { get => Stars.Count; }
        public static star BirthStar;
        public static void set(star birthStar, List<star> stars)
        {
            BirthStar = birthStar;
            Stars = stars;
        }
        //static settings()
        //{
        //    List<planet> p = new List<planet>
        //    {
        //        new planet("Urf")
        //    };
        //    BirthStar = new star("BeetleJuice", ESpectrType.O,EStarType.MainSeqStar, p);
        //    for (var i = 0;i < 2;i++)
        //    {
        //        Stars.Add(new star("Star"+i.ToString(), ESpectrType.X, EStarType.BlackHole, p));
        //    }
        //}
        //private static settings instance = new settings();
        //static settings() { }
        //private settings() { }
        //private List<star> stars;
        //public List<star> Stars { get => instance.stars; set => instance.stars = value; }
        //private star birthStar;
        //public star BirthStar { get => instance.birthStar; set => instance.birthStar = value; }
        //public int starCount { get { return stars.Count + 1; } }
        //public static settings Instance { get { return instance; } set { } }
    }

    public class star
    {
        public string Name;
        public ESpectrType Spectr;
        public EStarType Type;
        public List<planet> Planets;
        public star(string name, ESpectrType spectr, EStarType type, List<planet> planets)
        {
            this.Name = name;
            this.Spectr = spectr;
            this.Type = type;
            this.Planets = planets;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
    public class planet
    {
        public string Name;
        public planet(string name)
        {
            this.Name = name;
        }
        public override string ToString()
        {
          return this.Name;
        }
    }
}