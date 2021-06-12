#if !NO_UNITY
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GSFullSerializer {
    public partial class fsConverterRegistrar {
        public static Internal.DirectConverters.Keyframe_DirectConverter Register_Keyframe_DirectConverter;
    }
}

namespace GSFullSerializer.Internal.DirectConverters {
    public class Keyframe_DirectConverter : fsDirectConverter<Keyframe> {
        protected override fsResult DoSerialize(Keyframe model, Dictionary<string, fsData> serialized) {
            var result = fsResult.Success;

            result += SerializeMember(serialized, null, "time", model.time);
            result += SerializeMember(serialized, null, "value", model.value);
#pragma warning disable CS0618 // Type or member is obsolete
            result += SerializeMember(serialized, null, "tangentMode", model.tangentMode);
#pragma warning restore CS0618 // Type or member is obsolete
            result += SerializeMember(serialized, null, "inTangent", model.inTangent);
            result += SerializeMember(serialized, null, "outTangent", model.outTangent);

            return result;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Keyframe model) {
            var result = fsResult.Success;

            var t0 = model.time;
            result += DeserializeMember(data, null, "time", out t0);
            model.time = t0;

            var t1 = model.value;
            result += DeserializeMember(data, null, "value", out t1);
            model.value = t1;

#pragma warning disable CS0618 // Type or member is obsolete
            var t2 = model.tangentMode;
#pragma warning restore CS0618 // Type or member is obsolete
            result += DeserializeMember(data, null, "tangentMode", out t2);
#pragma warning disable CS0618 // Type or member is obsolete
            model.tangentMode = t2;
#pragma warning restore CS0618 // Type or member is obsolete

            var t3 = model.inTangent;
            result += DeserializeMember(data, null, "inTangent", out t3);
            model.inTangent = t3;

            var t4 = model.outTangent;
            result += DeserializeMember(data, null, "outTangent", out t4);
            model.outTangent = t4;

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) => new Keyframe();
    }
}
#endif