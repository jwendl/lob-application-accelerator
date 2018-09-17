using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using System;
using System.Collections.Generic;
using Xunit;

namespace LobAccelerator.Library.Tests
{
    public class TeamsTests
    {
        public static Workflow ExampleWorkflow => new Workflow()
        {
            Teams = new List<Team>()
                {
                    new Team()
                    {
                        DisplayName = "New Teams Team",
                        Description = "This is a team for teams.",
                        MailNickname = "group",
                        Members = new List<string>()
                        {
                            "juswen@microsoft.com",
                            "ridize@microsoft.com",
                        },
                        MemberSettings = new MemberSettings()
                        {
                            AllowCreateUpdateChannels = true,
                        },
                        MessagingSettings = new MessagingSettings()
                        {
                            AllowUserEditMessages = true,
                            AllowUserDeleteMessages = false,
                        },
                        FunSettings = new FunSettings()
                        {
                            AllowGiphy = true,
                            GiphyContentRating = "strict",
                        },
                        Channels = new List<Channel>()
                        {
                            new Channel()
                            {
                                DisplayName = "New Teams Channel",
                                Description = "A new channel for the teams team."
                            }
                        }
                    }
                }
        };


        [Fact]
        public void AddPeopleToChannel()
        {
            //Arrange
            var workflow = ExampleWorkflow;

            //Act
            

            //Assert
        }
    }
}
