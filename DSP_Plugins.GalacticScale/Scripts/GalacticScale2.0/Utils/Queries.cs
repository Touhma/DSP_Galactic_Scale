namespace GalacticScale
{
    public static partial class GS2
    {
        public static GSStar GetGSStar(StarData star)
        {
            return GetGSStar(star.id);
        }

        public static GSStar GetGSStar(int id)
        {
            return gsStars[id];
        }
        public static GSStar GetGSStar(string name)
        {
            //Log("Checking GSStars. All " + GSStars.Count + " of them.");
            foreach (var kvp in gsStars)
            {
                GSStar s = kvp.Value;
                //Log("Checking "+ p.Name + " == " + name);
                if (s.Name == name) return s;
            }
            return null;
        }
        public static GSPlanet GetGSPlanet(PlanetData planet)
        {
            return GetGSPlanet(planet.id);
        }
        public static GSPlanet GetGSPlanet(string name)
        {
            //Log("Checking gsPlanets. All " + gsPlanets.Count + " of them.");
            foreach (var kvp in gsPlanets)
            {
                GSPlanet p = kvp.Value;
                //Log("Checking "+ p.Name + " == " + name);
                if (p.Name == name) return p;
            }
            return null;
        }
        public static GSPlanet GetGSPlanet(int vanillaID)
        {
            //Warn($"Finding GSPlanet By ID '{vanillaID}' requested by {GetCaller(2)}");
            if (vanillaID < 0)
            {
                Warn("Failed to get GSPlanet. ID less than 0. ID:" + vanillaID);
                return null;
            }
            //Log("2Finding GSPlanet By ID " + vanillaID);
            if (!gsPlanets.ContainsKey(vanillaID))
            {
                Warn("Failed to get GSPlanet. ID does not exist. ID:" + vanillaID + GetCaller());
                return null;
            }
            //Log("3Finding GSPlanet By ID " + vanillaID);
            if (gsPlanets[vanillaID] == null)
            {
                Warn("Failed to get GSPlanet. ID exists, but GSPlanet is null. ID:" + vanillaID);
                return null;
            }
            return gsPlanets[vanillaID];
        }
    }
}