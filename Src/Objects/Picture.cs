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
            get
            {
                return GetValueOrDefault<string>("Caption");
            }
            set
            {
                SetValue<string>("Caption", value, checkIsProp: false);
            }
        }

        [JsonIgnore]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal BuddyFile Data
        {
            get
            {
                return GetValueOrDefault<BuddyFile>("Data");
            }
            set
            {
                SetValue<BuddyFile>("Data", value, checkIsProp: false);
            }
        }

        [Newtonsoft.Json.JsonProperty("signedUrl")]
        public string SignedUrl
        {
            get
            {
                return GetValueOrDefault<string>("signedUrl");
            }
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
            SetValue<string>("signedUrl", signedUrl, checkIsProp: false);
        }
    }
}
