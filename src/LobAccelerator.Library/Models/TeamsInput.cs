using System.Collections.Generic;

namespace LobAccelerator.Library.Models
{

    public abstract class TeamsObjectInput
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ChannelInput : TeamsObjectInput
    {
        public string FilesAzstorageFolder { get; set; }
        public List<string> Members { get; set; }
    }

    public class TeamsInput : TeamsObjectInput
    {
        public List<string> Members { get; set; }
        public List<ChannelInput> Channels { get; set; }
    }
}
