using System.Collections.Generic;

namespace GalacticScale
{
    public class GSVeinTypes : List<GSVeinType>
    {
        public new GSVeinType Add(GSVeinType vt)
        {
            base.Add(vt);
            return vt;
        }

        public bool ContainsVein(EVeinType type, bool allowRare = false)
        {
            foreach (var vt in this)
                if (vt.type == type && (vt.rare == false || allowRare))
                    return true;
            return false;
        }
    }
}