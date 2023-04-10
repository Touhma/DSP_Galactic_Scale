using System.Collections.Generic;
using GSSerializer;
using System.Globalization;
using System;

namespace GalacticScale
{
    public class FloatPairConverter : fsDirectConverter<FloatPair>
    {
        private static readonly CultureInfo UsEnglishCulture = new CultureInfo("en-US");

        protected override fsResult DoSerialize(FloatPair model, Dictionary<string, fsData> serialized)
        {
            serialized["low"] = new fsData(model.low.ToString(UsEnglishCulture));
            serialized["high"] = new fsData(model.high.ToString(UsEnglishCulture));
            return fsResult.Success;
        }



        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref FloatPair model)
        {
            fsData lowData = data["low"];
            fsData highData = data["high"];

            model.low = float.Parse(lowData.AsString, UsEnglishCulture);
            model.high = float.Parse(highData.AsString, UsEnglishCulture);

            return fsResult.Success;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new FloatPair();
        }


    }
}