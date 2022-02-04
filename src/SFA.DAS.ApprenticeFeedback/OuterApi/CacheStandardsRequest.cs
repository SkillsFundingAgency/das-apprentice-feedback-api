using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ApprenticeFeedback.OuterApi
{
    public class CacheStandardsRequest
    {
        [JsonProperty("standards")]
        public List<Standard> Standards { get; set; }
    }

    public class Standard
    {
        [JsonProperty("standardUId")]
        public string StandardUId { get; set; }
        [JsonProperty("standardReference")]
        public string StandardReference { get; set; }
        [JsonProperty("")]
        public string StandardName{ get; set; }
        public int LarsCode { get; set; }
    }
}
