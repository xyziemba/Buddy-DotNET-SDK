using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuddySDK.BuddyServiceClient;
using Newtonsoft.Json;
using System.IO;

namespace BuddySDK
{
    [BuddyObjectPath("/blobs")]
    public class Blob : BuddyBase
    {
        [Newtonsoft.Json.JsonProperty("friendlyName")]
        public string FriendlyName
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

        internal Blob() : base()
        {
        }

        public Blob(string id)
            : base(id)
        {
        }

        public Blob(string id, string friendlyName)
            : this(id)
        {
            FriendlyName = friendlyName;
        }
    }
}
