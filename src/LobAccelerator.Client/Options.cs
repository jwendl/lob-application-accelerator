using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace LobAccelerator.Client
{
    public class Options
    {
        [Option('i', "input", Default = InputFormats.yaml, HelpText = "Input files format. Supported: yaml | yml | json.")]
        public InputFormats InputFormat { get; set; }

        [Option('d', "definitions", Required = true, HelpText = "Input files to be processed by Microsoft Graph.")]
        public IEnumerable<string> DefinitionsFiles { get; set; }

        [Option('c', "config", Default = "config.json", HelpText = "Configuration file for the LOB Client.")]
        public string ConfigurationFile { get; set; }

        [Option('z', "zip", Required = false, HelpText = "Zip file with dependencies.")]
        public string ZipFile { get; set; }


        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }


        [Usage(ApplicationAlias = "lobctl")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Process yaml files", new Options { DefinitionsFiles = new string[] { "teams.yaml", "onedrive.yaml" } });
                yield return new Example("Process json files", new Options { DefinitionsFiles = new string[] { "teams.json", "sharepoint.json" }, InputFormat = InputFormats.json });
                yield return new Example("Process files and dependencies", new Options { DefinitionsFiles = new string[] { "teams.json", "sharepoint.json" }, InputFormat = InputFormats.json,  ZipFile = "mydep.zip"});
                yield return new Example("Process files with specific config file", new Options { DefinitionsFiles = new string[] { "teams.json", "sharepoint.json" }, InputFormat = InputFormats.json, ConfigurationFile = "config.prod.json"});
            }
        }
    }
}
