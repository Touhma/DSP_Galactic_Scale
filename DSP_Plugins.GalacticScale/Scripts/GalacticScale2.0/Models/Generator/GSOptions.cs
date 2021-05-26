﻿namespace GalacticScale
{
    /// <summary>
    /// Custom List<GSUI> class that returns the GSUI when adding to the list, for saving as a local variable for future use
    /// </summary>
    public class GSOptions : System.Collections.Generic.List<GSUI>
    {
        public new GSUI Add(GSUI item)
        {
            base.Add(item);
            return item;
        }
    }

}