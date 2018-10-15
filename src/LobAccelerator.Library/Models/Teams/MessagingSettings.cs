using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams
{
    public class MessagingSettings
    {
        [DisplayName("Allow User Edit Messages")]
        [Description("")]
        public bool AllowUserEditMessages { get; set; }

        [DisplayName("Allow User Delete Messages")]
        [Description("")]
        public bool AllowUserDeleteMessages { get; set; }
    }
}
