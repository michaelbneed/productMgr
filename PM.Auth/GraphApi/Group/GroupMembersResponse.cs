using System.Collections.Generic;
using Newtonsoft.Json;

namespace PM.Auth.GraphApi.Group
{
    public class GroupMembersResponse
    {
        [JsonProperty("value")]
        public List<MemberUrl> Urls { get; set; }
    }
}
