using System.ComponentModel;

namespace LobAccelerator.Library.Models.Users
{
    public class UserBody
    {
        [DisplayName("Account Enabled")]
        public bool AccountEnabled { get; set; }

        [DisplayName("Display Name")]
        public string DisplayName { get; set; }

        [DisplayName("Mail Nickname")]
        public string MailNickname { get; set; }

        [DisplayName("User Principal Name")]
        public string UserPrincipalName { get; set; }

        [DisplayName("Password Profile")]
        public PasswordProfile PasswordProfile { get; set; }

        [DisplayName("Usage Location")]
        public string UsageLocation { get; set; }
    }

    public class PasswordProfile
    {
        [DisplayName("Force Change Password Next Sign In")]
        public bool ForceChangePasswordNextSignIn { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
