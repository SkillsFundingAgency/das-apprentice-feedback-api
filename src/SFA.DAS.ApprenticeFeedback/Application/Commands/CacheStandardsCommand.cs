using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.OuterApi
{
    public class CacheStandardsCommand
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
        [JsonProperty("standardName")]
        public string StandardName{ get; set; }
        [JsonProperty("larsCode")]
        public int LarsCode { get; set; }
    }
}
