using LobAccelerator.Library.Interfaces;
using LobAccelerator.Library.Models;
using System;
using System.Linq;

namespace LobAccelerator.Library.Validators
{
    public class TeamsInputValidator
    {

        public bool Validate(TeamsJsonConfiguration ti, bool hastoken)
        {
            return Validate(ti, hastoken, out var dummy);
        }

        public bool Validate(TeamsJsonConfiguration ti, bool hastoken, out TeamsInputValidation validation)
        {
            var rta = true;
            validation = new TeamsInputValidation();

            if (!hastoken)
            {
                validation = TeamsInputValidation.NoAuthToken;
                rta = false;
            }
            else if (ti == null)
            {
                validation = TeamsInputValidation.InvalidTeamsConfigObject;
                rta = false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ti.Name))
                {
                    validation = TeamsInputValidation.TeamNameIsNotValid;
                    rta = false;
                }
                else if (ti.Members == null || ti.Members?.Count == 0)
                {
                    validation = TeamsInputValidation.TeamHasntMembers;
                    rta = false;
                }
                else if (ti.Channels == null || ti.Channels?.Count == 0)
                {
                    validation = TeamsInputValidation.TeamHasntChannels;
                    rta = false;
                }
                else
                {
                    rta = ValidateDuplicateUsersOnTeam(ti, ref validation);

                    if (rta)
                    {
                        rta = ValidateChannels(ti, ref validation);
                    }
                }

            }

            return rta;
        }

        private bool ValidateDuplicateUsersOnTeam(TeamsJsonConfiguration ti, ref TeamsInputValidation validation)
        {
            var rta = true;
            var duplicates = (from member in ti.Members
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

        private bool ValidateChannels(TeamsJsonConfiguration ti, ref TeamsInputValidation validation)
        {
            bool rta = true;
            foreach (var channel in ti.Channels)
            {
                if (string.IsNullOrWhiteSpace(channel.Name))
                {
                    rta = false;
                    validation = TeamsInputValidation.InvalidTeamsChannelName;
                    break;
                }
                else
                {
                    rta = ValidateChannelMembers(ti, channel, ref validation);
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

        private bool ValidateDuplicatedusersOnChannel(ChannelJsonConfiguration channel, 
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

        private bool ValidateChannelMembers(TeamsJsonConfiguration ti, ChannelJsonConfiguration channel, ref TeamsInputValidation validation)
        {
            var rta = true;
            var matches = from cmember in channel.Members
                          from tmember in ti.Members
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
