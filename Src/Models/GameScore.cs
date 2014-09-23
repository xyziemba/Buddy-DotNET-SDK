using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class GameScore : ModelBase
    {
        [Newtonsoft.Json.JsonProperty("game")]
        public string GameID
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("score")]
        public double Score
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("session")]
        public string SessionID
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("player")]
        public string PlayerID
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("playerName")]
        public int Rank
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("award")]
        public string Award
        {
            get;
            set;
        }
    }
}
