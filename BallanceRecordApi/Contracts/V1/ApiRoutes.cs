namespace BallanceRecordApi.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Version = "v1";
        public const string Root = "api";
        public const string Prefix = Root + "/" + Version;
        public static class Records
        {
            public const string RecordsRoot = Prefix + "/records";
            public const string GetAll = RecordsRoot;
            public const string Get = RecordsRoot + "/{recordId}";
            public const string GetByRoom = RecordsRoot + "/rooms/{roomId}";
            public const string Update = RecordsRoot + "/{recordId}";
            public const string Delete = RecordsRoot + "/{recordId}";
            public const string Create = RecordsRoot;
            public const string CreateDefiningUser = RecordsRoot + "/users/{userId}";
            public const string GetFile = RecordsRoot + "/file";
            public const string PostFile = RecordsRoot + "/file";
        }

        public static class Identity
        {
            public const string IdentityRoot = Prefix + "/users";
            public const string Get = IdentityRoot + "/{userId}";
            public const string GetSelf = IdentityRoot;
            public const string Login = IdentityRoot + "/login";
            public const string Register = IdentityRoot + "/register";
            public const string Refresh = IdentityRoot + "/refresh";
            public const string Email = IdentityRoot + "/email";
            public const string Confirmation = IdentityRoot + "/confirmation";
        }

        public static class Room
        {
            public const string RoomRoot = Prefix + "/rooms";
            public const string GetAll = RoomRoot;
            public const string Get = RoomRoot + "/{roomId}";
            public const string Update = RoomRoot + "/{roomId}";
            public const string Create = RoomRoot;
            public const string StatusTransition = RoomRoot + "/{roomId}/status";
            public const string Events = RoomRoot + "/{roomId}/events";
        }
    }
}