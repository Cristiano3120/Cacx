namespace CacxServer.Abstractions.Auth;

public sealed class RegisterResult
{
    private RegisterResult() { }

    public string? Token { get; init; }
    public RegisterError? Error { get; init; }

    public bool IsSuccess => Token is not null;

    public static RegisterResult Success(string token)
        => new() { Token = token };

    public static RegisterResult Fail(RegisterError error)
        => new() { Error = error };
}
