using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams
{
    public class MemberSettings
    {
        [DisplayName("Allow Create Update Channels")]
        public bool AllowCreateUpdateChannels { get; set; }
    }
}
