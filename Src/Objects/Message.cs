using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK
{
    [BuddyObjectPath("/messages")]
    public class Message : BuddyBase
    {


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


        [Newtonsoft.Json.JsonProperty("send")]
        public DateTime Sent
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("addressees")]
        public IEnumerable<string> Recipients
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("messageType")]
        public MessageType MessageType
        {
            get;
            internal set;
        }

        [Newtonsoft.Json.JsonProperty("isNew")]
        public bool IsNew
        {
            get;
            set;
        }

        internal Message()
            : base()
        {

        }

        public Message(string id)
            : base(id)
        {

        }
    }
}
