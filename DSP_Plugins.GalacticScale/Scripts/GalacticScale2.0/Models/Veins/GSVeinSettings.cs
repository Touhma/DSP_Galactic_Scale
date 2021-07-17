namespace GalacticScale
{
    public class GSVeinSettings
    {
        public string Algorithm = "Vanilla";
        public float VeinPadding = 1f;
        public GSVeinTypes VeinTypes = new GSVeinTypes();

        public bool RequiresConversion => VeinTypes.Count > 0; //&& Algorithm == "Vanilla"; }

        public GSVeinSettings Clone()
        {
            var clone = (GSVeinSettings) MemberwiseClone();
            clone.VeinTypes = new GSVeinTypes();
            for (var i = 0; i < VeinTypes.Count; i++) clone.VeinTypes.Add(VeinTypes[i].Clone());

            return clone;
        }
    }
}