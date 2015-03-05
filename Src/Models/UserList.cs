using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace BuddySDK.Models
{
    public class UserList : ModelBase
    {

        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }


    }
}