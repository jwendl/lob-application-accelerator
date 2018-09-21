namespace LobAccelerator.Client.Models
{
    public class Configuration
    {
        public string Endpoint { get; set; }
        public AzureAd AzureAd { get; set; }
    }

    public class AzureAd
    {
        public string Resource { get; set; }
        public string ClientId { get; set; }
    }
}
