using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using System.Linq;

namespace LobAccelerator.Library.Validators
{
    public class TeamsInputValidator
    {

        public bool Validate(TeamResource teamsJsonConfiguration)
        {
            return Validate(teamsJsonConfiguration, out var dummy);
        }

        public bool Validate(TeamResource teamsJsonConfiguration,  out TeamsInputValidation validation)
        {
            var rta = true;
            validation = new TeamsInputValidation();

           if (teamsJsonConfiguration == null)
            {
                validation = TeamsInputValidation.InvalidTeamsConfigObject;
                rta = false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(teamsJsonConfiguration.DisplayName))
                {
                    validation = TeamsInputValidation.TeamNameIsNotValid;
                    rta = false;
                }
                else if (teamsJsonConfiguration.Members == null 
                    || teamsJsonConfiguration.Members?.Count() == 0)
                {
                    validation = TeamsInputValidation.TeamHasntMembers;
                    rta = false;
                }
                else if (teamsJsonConfiguration.Channels == null 
                    || teamsJsonConfiguration.Channels?.Count() == 0)
                {
                    validation = TeamsInputValidation.TeamHasntChannels;
                    rta = false;
                }
                else
                {
                    rta = ValidateDuplicateUsersOnTeam(teamsJsonConfiguration, ref validation);

                    if (rta)
                    {
                        rta = ValidateChannels(teamsJsonConfiguration, ref validation);
                    }
                }

            }

            return rta;
        }

        private bool ValidateDuplicateUsersOnTeam(TeamResource teamsJsonConfiguration, 
            ref TeamsInputValidation validation)
        {
            var rta = true;
            var duplicates = (from member in teamsJsonConfiguration.Members
                              group member by member
                             into grpmember
                              where grpmember.Count() > 1
                              select true).Any();

            if (duplicates)
            {
                rta = false;
                validation = TeamsInputValidation.DuplicatedUsersOnTeam;
            }

            return rta;
        }

        public string GetVerboseValitadion(TeamsInputValidation validation)
        {
            string verbose = string.Empty;

            switch (validation)
            {
                case TeamsInputValidation.InvalidTeamsConfigObject:
                    verbose = "The configuration object is invalid";

                    break;
                case TeamsInputValidation.TeamNameIsNotValid:
                    verbose = "Team name shouldn't be empty";

                    break;
                case TeamsInputValidation.TeamHasntMembers:
                    verbose = "Team should have at least one member";

                    break;
                case TeamsInputValidation.TeamHasntChannels:
                    verbose = "Team should have at least one channel";
                    break;
                case TeamsInputValidation.InvalidTeamsChannelName:
                    verbose = "Channel name couldn't be empty";
                    break;
                case TeamsInputValidation.ChannelMembersDontMatchTeamsMembers:
                    verbose = "Channel members should exist firts as Teams members";
                    break;
                case TeamsInputValidation.NoAuthToken:
                    verbose = "This function requires an Authorization header in the request";
                    break;
                case TeamsInputValidation.DuplicatedUsersOnTeam:
                    verbose = "The team includes duplicated users";
                    break;
                case TeamsInputValidation.DuplicatedUsersOnChannel:
                    verbose = "Some channels include duplicated users";
                    break;
                default:
                    verbose = "OK";
                    break;
            }

            return verbose;
        }

        private bool ValidateChannels(TeamResource teamsJsonConfiguration, ref TeamsInputValidation validation)
        {
            bool rta = true;
            foreach (var channel in teamsJsonConfiguration.Channels)
            {
                if (string.IsNullOrWhiteSpace(channel.DisplayName))
                {
                    rta = false;
                    validation = TeamsInputValidation.InvalidTeamsChannelName;
                    break;
                }
                else
                {
                    rta = ValidateChannelMembers(teamsJsonConfiguration, channel, ref validation);
                    if (!rta)
                    {
                        break;
                    }
                    else
                    {
                        rta = ValidateDuplicatedusersOnChannel(channel, ref validation);
                        if (!rta)
                        {
                            break;
                        }
                    }
                }
            }

            return rta;
        }

        private bool ValidateDuplicatedusersOnChannel(ChannelResource channel,
            ref TeamsInputValidation validation)
        {
            var rta = true;
            var duplicates = (from member in channel.Members
                              group member by member
                             into grpmember
                              where grpmember.Count() > 1
                              select true).Any();

            if (duplicates)
            {
                rta = false;
                validation = TeamsInputValidation.DuplicatedUsersOnChannel;
            }

            return rta;
        }

        private bool ValidateChannelMembers(TeamResource teamsJsonConfiguration,
            ChannelResource channel, ref TeamsInputValidation validation)
        {
            var rta = true;
            var matches = from cmember in channel.Members
                          from tmember in teamsJsonConfiguration.Members
                          where tmember == cmember
                          select cmember;

            foreach (var member in channel.Members)
            {
                var matched = (from matchedMember in matches
                               where matchedMember == member
                               select matchedMember).Any();

                if (!matched)
                {
                    rta = false;
                    validation = TeamsInputValidation.ChannelMembersDontMatchTeamsMembers;
                    break;
                }
            }

            return rta;
        }
    }
}
