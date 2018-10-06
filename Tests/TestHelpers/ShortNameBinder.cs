using System;
using Newtonsoft.Json.Serialization;

class ShortNameBinder : ISerializationBinder
{
    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;
        typeName = serializedType.Name;
    }

    public Type BindToType(string assemblyName, string typeName)
    {
        throw new Exception();
    }
}