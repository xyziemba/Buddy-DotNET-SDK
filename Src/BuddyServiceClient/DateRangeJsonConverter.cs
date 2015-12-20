using System;
using System.Globalization;

namespace BuddySDK.BuddyServiceClient
{
    internal class DateRangeJsonConverter : Newtonsoft.Json.JsonConverter
    {
        static readonly DateTime UnixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static long ToUnixTicks(DateTime dt)
        {
            return (long)dt.ToUniversalTime().Subtract(UnixStart).TotalMilliseconds;
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var dr = (DateRange)value;
            var val = "";
            if (dr.StartDate.HasValue) {
                val += GetUnixTicksString(dr.StartDate.Value);
            }
            val += "-";

            if (dr.EndDate.HasValue) {
                val += GetUnixTicksString(dr.EndDate.Value);
            }
            writer.WriteValue(val);
        }

        private string GetUnixTicksString(DateTime dateTime)
        {
            return string.Format(CultureInfo.InvariantCulture, "/Date({0})/", ToUnixTicks(dateTime));
        }
    }
}
