namespace BallanceRecordApi.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Version = "v1";
        public const string Root = "api";
        public const string Prefix = Root + "/" + Version;
        public static class Posts
        {
            public const string GetAll = Prefix + "/posts";
        }
    }
}