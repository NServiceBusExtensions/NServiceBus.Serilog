using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ObjectApproval;

class CustomContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        property.SkipEmptyCollections(member);
        return property;
    }
}