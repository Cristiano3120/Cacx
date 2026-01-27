namespace CacxShared;

public static class Endpoints
{
    public const string Base = "api";

    public static class AuthEndpoints
    {
        public const string BaseAuth = "auth";
        public const string Register = "register";
        public const string Login = "login";
        public const string RequestVerificationEmail = "requestVerificationEmail";

        public static string BaseAuthEndpoint => $"{Base}/{BaseAuth}";
        public static string RegisterEndpoint => $"{BaseAuthEndpoint}/{Register}";
        public static string LoginEndpoint => $"{BaseAuthEndpoint}/{Login}";
        public static string RequestVerificationEmailEndpoint => $"{BaseAuthEndpoint}/{RequestVerificationEmail}";
    }
}
