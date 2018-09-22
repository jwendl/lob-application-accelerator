using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace LobAccelerator.Client.Extensions
{
    public static class FileExtensions
    {
        public static bool IsYaml(this string fileExtension)
        {
            return fileExtension == ".yaml" || fileExtension == ".yml";
        }

        public static async Task<string> GetFileContentAsync(this string file)
        {
            var fileExtension = Path.GetExtension(file);
            var fileContent = await File.ReadAllTextAsync(file);

            return fileExtension.IsYaml()
                ? ConvertYamlToJson(fileContent)
                : fileContent;
        }

        private static string ConvertYamlToJson(string fileContent)
        {
            var reader = new StringReader(fileContent);

            var deserializer = new Deserializer();
            var yamlObject = deserializer.Deserialize(reader);

            var serializer = new JsonSerializer();
            var stringWriter = new StringWriter();

            serializer.Serialize(stringWriter, yamlObject);

            return stringWriter.ToString();
        }
    }
}
