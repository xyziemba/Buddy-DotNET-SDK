using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BuddySDK.Models
{
    public class PagedResult<T>
    {
        [JsonProperty("nextToken")]
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

