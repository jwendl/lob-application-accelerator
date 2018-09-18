using System.Collections.Generic;

namespace LobAccelerator.Library.Models
{

    public abstract class TeamsJsonConfigObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ChannelJsonConfiguration : TeamsJsonConfigObject
    {
        public string FilesAzstorageFolder { get; set; }
        public List<string> Members { get; set; }
    }

    public class TeamsJsonConfiguration : TeamsJsonConfigObject
    {
        public List<string> Members { get; set; }
        public List<ChannelJsonConfiguration> Channels { get; set; }
    }
}
