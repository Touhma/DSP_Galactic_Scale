using System;
using FullSerializer;
namespace GalacticScale
{
    // We're going to serialize MyType directly to/from a string.
    [fsObject(Converter = typeof(GSPreferencesConverter))]
    public class MyType
    {
        public string Value;
    }

    public class GSPreferencesConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            // CanProcess will be called over every type that Full Serializer
            // attempts to serialize. If this converter should be used, return true
            // in this function.
            return (type.GetType() != null);
        }

        public override fsResult TrySerialize(object instance,
            out fsData serialized, Type storageType)
        {

            // Serialize the data into the serialized parameter. fsData is a
            // strongly typed object store that maps directly to a JSON object model.
            // It's really easy to use.

            var myType = (MyType)instance;
            serialized = new fsData(myType.Value);
            return fsResult.Success;
        }

        public override fsResult TryDeserialize(fsData storage,
            ref object instance, Type storageType)
        {

            // Always make to sure to verify that the deserialized data is the of
            // the expected type. Otherwise, on platforms where exceptions are
            // disabled bad things can happen (if the data was actually an object
            // and you try to access a string, an exception will be thrown).
            if (storage.Type != fsDataType.String)
            {
                return fsResult.Fail("Expected string fsData type but got " + storage.Type);
            }

            // We just want to deserialize into the existing object instance. If
            // instance is a value type, then we can assign directly into instance
            // to update the value.

            var myType = (MyType)instance;
            myType.Value = storage.AsString;
            return fsResult.Success;
        }

        // Object instance construction is separated from deserialization so that
        // cycles can be correctly handled. If it's not possible to construct an
        // instance of the expected type here, then just return any non-null value
        // and construct the proper instance in TryDeserialize (though cycles will
        // *not* be handled properly).
        //
        // You do not need to override this method if your converted type is a
        // struct.
        public override object CreateInstance(fsData data, Type storageType)
        {
            return new MyType();
        }
    }
}