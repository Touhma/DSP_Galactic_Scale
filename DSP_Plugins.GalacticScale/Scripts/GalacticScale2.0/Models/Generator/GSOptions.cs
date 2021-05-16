namespace GalacticScale
{
    public class GSOptions : System.Collections.Generic.List<GSUI>
    {
        public new GSUI Add(GSUI item)
        {
            base.Add(item);
            return item;
        }
    }

}