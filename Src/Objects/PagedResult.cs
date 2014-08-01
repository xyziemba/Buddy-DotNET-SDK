using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Objects
{
    public class PagedResult<T>
    {
        [JsonProperty("previousToken")]
        public string NextToken
        {
            get;
            set;
        }

        [JsonProperty("currentToken")]
        public string CurrentToken
        {
            get;
            set;
        }
        
        [JsonProperty("previousToken")]
        public string PreviousToken
        {
            get;
            set;
        }

        [JsonProperty("pageResults")]
        public IEnumerable<T> PageResults
        {
            get;
            set;
        }
    }
}
