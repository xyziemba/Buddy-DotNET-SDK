using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class SocialNetworkUser : User
    {
        [Newtonsoft.Json.JsonProperty("isNew")]
        public bool IsNew { get; set; }

    }
}
