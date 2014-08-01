using BuddySDK.BuddyServiceClient;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace BuddySDK
{
    [BuddyObjectPath("/pictures")]
    public class Picture : BuddyBase
    {
        [Newtonsoft.Json.JsonProperty("caption")]
        public string Caption
        {
            get;
            set;
        }

        [JsonIgnore]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BuddyFile Data
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("signedUrl")]
        public string SignedUrl
        {
            get;
            private set;
        }

        internal Picture() : base()
        {
        }

        public Picture(string id)
            : base(id)
        {
        }

        public Picture(string id, string signedUrl)
            : this(id)
        {
            SignedUrl = signedUrl;
        }
    }
}
