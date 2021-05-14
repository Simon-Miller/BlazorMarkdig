using Newtonsoft.Json;
using System;

namespace MyOverflow.Shared
{
    public abstract class CosmosDbEntityBase
    {
        /// <summary>
        /// generated when saved for the first time.  Leaving blank means we know when to Create or Replace at point of saving.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        //[JsonPropertyName("_rid")]
        //public string RID { get; set; }

        //[JsonPropertyName("_self")]
        //public string SELF { get; set; }

        //[JsonPropertyName("_etag")]
        //public string ETAG { get; set; }

        //[JsonPropertyName("_attachments")]
        //public string ATTACHMENTS { get; set; }

        //[JsonPropertyName("_ts")]
        //public string TS { get; set; }
    }
}
