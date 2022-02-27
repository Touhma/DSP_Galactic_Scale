using GSSerializer;

namespace GalacticScale
{
    public class GSVeinSettings
    {
        public string Algorithm = "Vanilla";
        public float VeinPadding = 1f;
        public GSVeinTypes VeinTypes = new();
        public bool RequiresConversion => VeinTypes.Count > 0;

        public GSVeinSettings Clone()
        {
            var clone = (GSVeinSettings)MemberwiseClone();
            clone.VeinTypes = new GSVeinTypes();
            for (var i = 0; i < VeinTypes.Count; i++) clone.VeinTypes.Add(VeinTypes[i].Clone());

            return clone;
        }

        public bool Equals(GSVeinSettings other)
        {
            var serializer = new fsSerializer();
            serializer.TrySerialize(this, out var thisData).AssertSuccessWithoutWarnings();
            serializer.TrySerialize(this, out var otherData).AssertSuccessWithoutWarnings();
            return thisData.Equals(otherData);
        }
    }
}