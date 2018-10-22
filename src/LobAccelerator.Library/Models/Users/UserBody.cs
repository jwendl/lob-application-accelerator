using Newtonsoft.Json;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Users
{
    public class UserBody
    {
        [DisplayName("ID")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [DisplayName("Account Enabled")]
        [JsonProperty("accountEnabled")]
        public bool AccountEnabled { get; set; }

        [DisplayName("Display Name")]
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [DisplayName("Mail Nickname")]
        [JsonProperty("mailNickname")]
        public string MailNickname { get; set; }

        [DisplayName("User Principal Name")]
        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        [DisplayName("Password Profile")]
        [JsonProperty("passwordProfile")]
        public PasswordProfile PasswordProfile { get; set; }

        [DisplayName("Usage Location")]
        [JsonProperty("usageLocation")]
        public string UsageLocation { get; set; }
    }

    public class PasswordProfile
    {
        [DisplayName("Force Change Password Next Sign In")]
        [JsonProperty("forceChangePasswordNextSignIn")]
        public bool ForceChangePasswordNextSignIn { get; set; }

        [DisplayName("Password")]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
