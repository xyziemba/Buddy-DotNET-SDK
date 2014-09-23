using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class Game : ModelBase
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
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

        [Newtonsoft.Json.JsonProperty("minPlayers")]
        public int MinimumPlayers
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("maxPlayers")]
        public int MaximumPlayers
        {
            get;
            set;
        }
    }
}
