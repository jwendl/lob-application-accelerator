using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams.Teams
{
    public class TeamBody
    {
        [DisplayName("Member Settings")]
        public MemberSettings MemberSettings { get; set; }

        [DisplayName("Messaging Settings")]
        public MessagingSettings MessagingSettings { get; set; }

        [DisplayName("Fun Settings")]
        public FunSettings FunSettings { get; set; }
    }
}
