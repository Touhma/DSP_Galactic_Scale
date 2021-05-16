using UnityEngine.Events;
using System.Collections.Generic;
namespace GalacticScale
{
    public interface iConfigurableGenerator : iGenerator
    {
        List<GSUI> Options { get; }
        void Import(GSGenPreferences preferences);
        GSGenPreferences Export();
    }
}