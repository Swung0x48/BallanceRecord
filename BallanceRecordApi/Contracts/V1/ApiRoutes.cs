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
            public const string GetByRoom = Prefix + "/records/rooms/{roomId}";
            public const string Update = Prefix + "/records/{recordId}";
            public const string Delete = Prefix + "/records/{recordId}";
            public const string Create = Prefix + "/records";
            public const string CreateDefiningUser = Prefix + "/records/{userId}";
            public const string GetFile = Prefix + "/records/file";
            public const string PostFile = Prefix + "/records/file";
        }

        public static class Identity
        {
            public const string Get = Prefix + "/user/{userId}";
            public const string GetSelf = Prefix + "/user";
            public const string Login = Prefix + "/user/login";
            public const string Register = Prefix + "/user/register";
            public const string Refresh = Prefix + "/user/refresh";
            public const string Email = Prefix + "/user/email";
            public const string Confirmation = Prefix + "/user/confirmation";
        }

        public static class Room
        {
            public const string GetAll = Prefix + "/rooms";
            public const string Get = Prefix + "/rooms/{roomId}";
            public const string Update = Prefix + "/rooms/{roomId}";
            public const string Create = Prefix + "/rooms";
            public const string StatusTransition = Prefix + "/rooms/{roomId}/status";
            public const string Events = Prefix + "/rooms/{roomId}/events";
        }
    }
}