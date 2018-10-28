using Newtonsoft.Json;
using System.ComponentModel;

namespace LobAccelerator.Library.Models.Users
{
    public class UserResource
    {
        [DisplayName("Display Name")]
        [JsonProperty("displayName")]
        [Description("The name to display in the address book for the user.")]
        public string DisplayName { get; set; }

        [DisplayName("Mail Nickname")]
        [JsonProperty("mailNickname")]
        [Description("The mail alias for the user.")]
        public string MailNickname { get; set; }

        [DisplayName("User Principal Name")]
        [JsonProperty("userPrincipalName")]
        [Description("The user principal name (someuser@contoso.com).")]
        public string UserPrincipalName { get; set; }

        [DisplayName("Password")]
        [JsonProperty("password")]
        [Description("The password for the user.")]
        public string Password { get; set; }

        [DisplayName("Force Change Password Next Sign In")]
        [JsonProperty("forceChangePasswordNextSignIn")]
        [Description("true if the user must change her password on the next login; otherwise false.")]
        public bool ForceChangePasswordNextSignIn { get; set; }

        [DisplayName("Usage Location")]
        [JsonProperty("usageLocation")]
        [Description("A two letter country code (ISO standard 3166). Required for users that will be assigned licenses due to legal requirement to check for availability of services in countries.")]
        public string UsageLocation { get; set; }

        [DisplayName("License Name")]
        [JsonProperty("licenseName")]
        [Description("The license (unique SKU display name) to assign to the user.")]
        public string LicenseName { get; set; }
    }
}
