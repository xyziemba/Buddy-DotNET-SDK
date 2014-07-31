using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddySDK
{
    [BuddyObjectPath("/notifications")]
    public class Notification : BuddyBase
    {
        [Newtonsoft.Json.JsonProperty("sentByPlatform")]
        public IDictionary<string,int> SentByPlatform { get; set; }

        internal Notification() : base(){
        }

        public Notification(string id) : base(id){
        }
    }
}

