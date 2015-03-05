using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class Message : ModelBase
    {
        public enum MessageType
        {
            Sent,
            Received
        }

        [Newtonsoft.Json.JsonProperty("subject")]
        public string Subject
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("body")]
        public string Body
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("thread")]
        public string ThreadId
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("from")]
        public string FromUserId
        {
            get;
            internal set;
        }

        [Newtonsoft.Json.JsonProperty("fromName")]
        public string FromUserName
        {
            get;
            internal set;
        }

        [Newtonsoft.Json.JsonProperty("to")]
        public string ToUserId
        {
            get;
            internal set;
        }

        [Newtonsoft.Json.JsonProperty("toName")]
        public string ToUserName
        {
            get;
            internal set;
        }


        [Newtonsoft.Json.JsonProperty("sent")]
        public DateTime Sent
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("recipients")]
        public IEnumerable<string> Recipients
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("messageType")]
        public BuddySDK.Models.Message.MessageType Type
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("isNew")]
        public bool IsNew
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("warnings")]
        public IEnumerable<IDictionary<string, object>> Warnings
        {
            get;
            set;
        }
    }
}
