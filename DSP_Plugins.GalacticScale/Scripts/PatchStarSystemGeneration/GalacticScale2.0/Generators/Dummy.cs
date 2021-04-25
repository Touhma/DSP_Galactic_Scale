using System.Collections.Generic;

namespace GalacticScale.Generators
{
    public class Dummy : iGenerator
    {
        string iGenerator.Name => "Dummy";

        string iGenerator.Author => "innominata";

        string iGenerator.Description => "The most basic generator. Simply to test";

        string iGenerator.Version => "0.0";

        string iGenerator.GUID => "space.customizing.generators.dummy";


        GSSettings iGenerator.Generate(int starCount)
        {
            GS2.Log("Wow, this barely worked");
            GS2.Log("Creating New Settings");
            GSSettings.Stars.Clear();
            List<GSplanet> p = new List<GSplanet>
            {
                new GSplanet("Urf")
            };
            GSSettings.Stars.Add(new GSStar(1, "BeatleJooce", ESpectrType.O, EStarType.MainSeqStar, p));
            for (var i = 1; i < starCount; i++)
            {
                GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.X, EStarType.BlackHole, new List<GSplanet>()));
            }
            GSSettings.GalaxyParams = new galaxyParams();
            GSSettings.GalaxyParams.iterations = 4;
            GSSettings.GalaxyParams.flatten = 0.18;
            GSSettings.GalaxyParams.minDistance = 2;
            GSSettings.GalaxyParams.minStepLength = 2.3;
            GSSettings.GalaxyParams.maxStepLength = 3.5;
            return GSSettings.Instance;
        }
    }
}