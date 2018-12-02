using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ObjectApproval;

public class CustomContractResolverEx : CustomContractResolver
{
    public CustomContractResolverEx() : base(true)
    {
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyName == "HandleCurrentMessageLaterWasCalled")
        {
            property.ShouldSerialize = instance => false;
        }

        return property;
    }
}