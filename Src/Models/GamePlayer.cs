using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class GamePlayer : ModelBase
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("pictureId")]
        public string PictureID
        {
            get;
            set;
        }
    }
}
