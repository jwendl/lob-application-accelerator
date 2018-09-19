namespace LobAccelerator.Library.Models
{
    public enum TeamsInputValidation
    {
        OK,
        InvalidTeamsConfigObject,
        TeamNameIsNotValid,
        TeamHasntMembers,
        TeamHasntChannels,
        InvalidTeamsChannelName,
        ChannelMembersDontMatchTeamsMembers,
        NoAuthToken,
        DuplicatedUsersOnTeam,
        DuplicatedUsersOnChannel
    }

}
