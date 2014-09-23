using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class GameSession : ModelBase
    {
        [Newtonsoft.Json.JsonProperty("game")]
        public string GameID
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("players")]
        public IEnumerable<string> Players
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("expiration")]
        public DateTime Expiration
        {
            get;
            set;
        }
    }
}
