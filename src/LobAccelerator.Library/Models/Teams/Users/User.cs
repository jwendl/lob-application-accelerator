using Newtonsoft.Json;

namespace LobAccelerator.Library.Models.Teams.Members
{
    public class User
    {
        [JsonProperty("value")]
        public Value[] Value { get; set; }
    }

    public class Value
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
