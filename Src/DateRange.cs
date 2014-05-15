using System;

namespace BuddySDK
{
    [Newtonsoft.Json.JsonConverter(typeof(BuddySDK.BuddyServiceClient.DateRangeJsonConverter))]
    public class DateRange
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Convert(StartDate), Convert(EndDate));
        }

        private string Convert(DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToUniversalTime().ToString() : "";
        }
    } 
}
