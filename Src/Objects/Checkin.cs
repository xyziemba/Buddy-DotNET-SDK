using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BuddySDK
{
    [BuddyObjectPath("/checkins")]
    public class Checkin : BuddyBase
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


        internal Checkin() : base()
        {

        }
       
        public Checkin(string id)
            : base(id)
        {

        }
    }    
}
