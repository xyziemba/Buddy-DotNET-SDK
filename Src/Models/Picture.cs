using BuddySDK.BuddyServiceClient;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class Picture : ModelBase
    {
        [Newtonsoft.Json.JsonProperty("caption")]
        public string Caption
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("signedUrl")]
        public string SignedUrl
        {
            get;
            set;
        }

    }
}
