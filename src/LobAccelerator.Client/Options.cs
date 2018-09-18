using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace LobAccelerator.Client
{
    public class Options
    {
        [Option('i', "input", Default = "yaml", HelpText = "Input files format. Supported: yaml | yml | json.")]
        public InputFormats InputFormat { get; set; }

        [Option('d', "definitions", Required = true, HelpText = "Input files to be processed by Microsoft Graph.")]
        public IEnumerable<string> DefinitionsFiles { get; set; }

        [Option('c', "client-id", Required = true, HelpText = "Client Id from Azure Active Directory application.")]
        public string ClientId { get; set; }

        [Option('u', "url", Required = true, HelpText = "LOB engine endpoint.")]
        public string LobURL { get; set; }

        [Option('z', "zip", Required = false, HelpText = "Zip file with dependencies")]
        public string ZipFile { get; set; }

        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }


        [Usage(ApplicationAlias = "lobctl")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Process yaml files", new Options { DefinitionsFiles = new string[] { "teams.yaml", "onedrive.yaml" }, ClientId = "398917db-d35d-4bd9-81cf-c3ff85c60e12", LobURL = "https://anylobproc.com/engine" });
                yield return new Example("Process json files", new Options { DefinitionsFiles = new string[] { "teams.json", "sharepoint.json" }, ClientId = "398917db-d35d-4bd9-81cf-c3ff85c60e12", InputFormat = InputFormats.json, LobURL = "https://anylobproc.com/engine" });
                yield return new Example("Process files and dependencies", new Options { DefinitionsFiles = new string[] { "teams.json", "sharepoint.json" }, ClientId = "398917db-d35d-4bd9-81cf-c3ff85c60e12", InputFormat = InputFormats.json, LobURL = "https://anylobproc.com/engine", ZipFile = "mydep.zip"});
            }
        }
    }
}
