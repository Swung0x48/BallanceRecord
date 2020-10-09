namespace BallanceRecordApi.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Version = "v1";
        public const string Root = "api";
        public const string Prefix = Root + "/" + Version;
        public static class Records
        {
            public const string GetAll = Prefix + "/records";
            public const string Get = Prefix + "/records/{recordId}";
            public const string Update = Prefix + "/records/{recordId}";
            public const string Create = Prefix + "/records";
        }
    }
}