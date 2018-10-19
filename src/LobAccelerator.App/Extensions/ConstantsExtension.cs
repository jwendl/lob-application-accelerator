namespace LobAccelerator.App.Extensions
{
    public static class ConstantsExtension
    {
        public const string PARAM_TABLE = "parameters";
        public const string PARAM_PARTITION_KEY = "Authorization";
        public const string PARAM_TOKEN_ROW = "refresh-token";
        public const string RESOURCES_TABLE = "resources";
        public const string RESOURCES_PARTITION_KEY = "Name";
        public const string RESOURCES_TOKEN_ROW = "resource-name";
        public const string TEAMS_REQUEST_QUEUE = "teams-requested-tasks";
        public const string USERS_REQUEST_QUEUE = "users-requested-tasks";
        public const string ARM_REQUEST_QUEUE = "arm-requested-tasks";
    }
}
