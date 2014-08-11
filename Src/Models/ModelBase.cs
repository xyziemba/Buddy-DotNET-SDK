using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public abstract class ModelBase
    {
        [JsonProperty("id")]
        public string ID
        {
            get;
            set;
        }

        [JsonProperty("location")]
        public virtual BuddyGeoLocation Location { get; set; }

        /* TODO: Make the service return these values */
//
//        [JsonProperty("readPermissions")]
//        public BuddyPermissions ReadPermissions
//        {
//            get;
//            set;
//        }
//
//        [JsonProperty("writePermissions")]
//        public BuddyPermissions WritePermissions
//        {
//            get;
//            set;
//        }

        [JsonProperty("created")]
        public DateTime Created
        {
            get;
            set;
        }

        [JsonProperty("lastModified")]
        public DateTime LastModified
        {
            get;
            set;
        }

        [JsonProperty("tag")]
        public string Tag
        {
            get;
            set;
        }

       
    }

    public class PagingModel<T>
    {

         [JsonProperty("currentToken")]
        public string CurrentToken { get; set; }

         [JsonProperty("nextToken")]
        public string NextToken { get; set; }

         [JsonProperty("previousToken")]
        public string PreviousToken { get; set; }

         [JsonProperty("pageResults")]
        public IEnumerable<T> PageResults { get; set; }

    }

    public class Metric
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("success")]
        public bool success { get; set; }
        internal IBuddyClient _client { get; set; }

        public Metric()
        { }

        public Metric(IBuddyClient client)
        {
            _client = client;
        }

        private class CompleteMetricResult
        {
            public long? elaspedTimeInMs { get; set; }
        }
        public Task<BuddyResult<TimeSpan?>> FinishAsync()
        {
            if (string.IsNullOrEmpty(ID) || _client == null)
            {
                throw new InvalidOperationException("Can't call finish on a metric that's missing an ID.");
            }
            var r = _client.DeleteAsync<CompleteMetricResult>(String.Format(CultureInfo.InvariantCulture, "/metrics/events/{0}", Uri.EscapeDataString(ID)), null);
            return r.WrapResult<CompleteMetricResult, TimeSpan?>((r1) =>
            {

                var cmr = r1.Value;

                TimeSpan? elapsedTime = null;

                if (cmr.elaspedTimeInMs != null)
                {
                    elapsedTime = TimeSpan.FromMilliseconds(cmr.elaspedTimeInMs.Value);
                }

                return elapsedTime;

            });
        }
    }
}
