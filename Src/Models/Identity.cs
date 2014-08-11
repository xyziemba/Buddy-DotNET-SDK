using Newtonsoft.Json;

namespace BuddySDK.Models
{
    public class Identity : ModelBase
    {
        [JsonProperty("identityProviderName")]
        public string ProviderName
        {
            get;
            set;
        }

        [JsonProperty("identityProviderID")]
        public string ProviderID
        {
            get;
            set;
        }
    }
}
