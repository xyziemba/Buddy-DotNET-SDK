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
            get { return GetValueOrDefault<string>("FriendlyName"); }
            set { SetValue<string>("FriendlyName", value, checkIsProp: false); }
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

        internal Blob(BuddyClient client = null) : base(client)
        {
        }

        public Blob(string id, BuddyClient client = null)
            : base(id, client)
        {
        }

        public Blob(string id, string friendlyName, BuddyClient client = null)
            : this(id, client)
        {
            SetValue<string>("friendlyName", friendlyName, checkIsProp: false);
        }

        public Task<BuddyResult<Stream>> GetFileAsync(string friendlyName = null)
        {
            return base.GetFileCoreAsync (GetObjectPath() + "/file", new { friendlyName = friendlyName });
        }
    }
}
