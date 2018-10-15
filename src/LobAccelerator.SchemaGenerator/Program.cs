using LobAccelerator.Library.Models;
using LobAccelerator.Library.Models.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace LobAccelerator.SchemaGenerator
{
    static class Program
    {
        static void Main(string[] args)
        {
            var generator = new JSchemaGenerator()
            {
                DefaultRequired = Required.AllowNull,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            var schema = generator.Generate(typeof(Workflow));
            using (var streamWriter = File.CreateText(@"workflow.schema.json"))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                schema.WriteTo(jsonWriter);
            }

            var exampleWorkflow = new Workflow()
            {
                Teams = new List<TeamResource>()
                {
                    new TeamResource()
                    {
                        DisplayName = "New Teams Team",
                        Description = "This is a team for teams.",
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
                        Channels = new List<ChannelResource>()
                        {
                            new ChannelResource
                            {
                                DisplayName = "New Teams Channel",
                                Description = "A new channel for the teams team."
                            }
                        }
                    }
                }
            };

            var jsonOutput = JsonConvert.SerializeObject(exampleWorkflow);
            var jObject = JObject.Parse(jsonOutput);

            var isValid = jObject.IsValid(schema);
            var validOutput = isValid ? "valid" : "invalid";

            Console.WriteLine($"The json is {validOutput}");
        }
    }
}
