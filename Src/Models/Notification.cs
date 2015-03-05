using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddySDK.Models
{
    public class NotificationResult
    {
        [Newtonsoft.Json.JsonProperty("sentByPlatform")]
        public IDictionary<string,int> SentByPlatform { get; set; }

    }
}

