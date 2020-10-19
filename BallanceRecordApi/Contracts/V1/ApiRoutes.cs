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
            public const string Delete = Prefix + "/records/{recordId}";
            public const string Create = Prefix + "/records";
        }

        public static class Identity
        {
            public const string Login = Prefix + "/identity/login";
            public const string Register = Prefix + "/identity/register";
            public const string Refresh = Prefix + "/identity/refresh";
            public const string ConfirmEmail = Prefix + "/identity/confirmEmail";
        }
    }
}