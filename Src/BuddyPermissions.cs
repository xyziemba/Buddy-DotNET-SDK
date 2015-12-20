using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuddySDK
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BuddyPermissions
    {
        User,
        App,
        Default = User
    }
}
