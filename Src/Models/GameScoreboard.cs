using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class GameScoreboard
    {

        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("direction")]
        public string Direction
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("scoreCount")]
        public int MaximumScores
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("scores")]
        public IEnumerable<GameScore> Scores
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("oneScorePerPlayer")]
        public bool OneScorePerPlayer
        {
            get;
            set;
        }
    }
}
