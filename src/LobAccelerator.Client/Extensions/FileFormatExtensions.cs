namespace LobAccelerator.Client.Extensions
{
    public static class FileFormatExtensions
    {
        public static bool IsYaml(this string fileExtension)
        {
            return fileExtension == ".yaml" || fileExtension == ".yml";
        }
    }
}
