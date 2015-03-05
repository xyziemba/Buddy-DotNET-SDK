
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BuddySDK.Models
{
    public class BinaryModelBase : ModelBase
    {
        [JsonProperty("contentType")]
        public string ContentType { get; set; }

         [JsonProperty("contentLength")]
        public long ContentLength { get; set; }

         [JsonProperty("signedUrl")]
        public Uri SignedUrl { get; set; }

    }
}

