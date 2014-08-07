using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BuddySDK.Models
{
    public class Checkin : ModelBase
    {

        public class BuddyCheckinLocation : BuddyGeoLocation {
            [JsonProperty("name")]
            public string Name { get; set; }
        }


        [Newtonsoft.Json.JsonProperty("comment")]
        public string Comment
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("description")]
        public string Description
        {
            get;
            set;
        }


    }    
}
