using Newtonsoft.Json;
using System;

namespace BuddySDK.Models
{
    public class Video : BinaryModelBase
    {
        [JsonProperty("encoding")]
        public string Encoding { get; set; }

         [JsonProperty("bitRate")]
        public int BitRate { get; set; }

         [JsonProperty("lengthInSeconds")]
        public double LengthInSeconds { get; set; }

         [JsonProperty("thumbnailID")]
        public string ThumbnailID { get; set; }
    }
}

