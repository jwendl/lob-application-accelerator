using System.ComponentModel;

namespace LobAccelerator.Library.Models.Teams
{
    public class FunSettings
    {
        [DisplayName("Allow Giphy")]
        [Description("Whether or not giphy gifs can be used in the team.")]
        public bool AllowGiphy { get; set; }

        [DisplayName("Giphy Content Rating")]
        [Description("What content rating to set for the giphy service.")]
        public string GiphyContentRating { get; set; }
    }
}
