namespace Testhon.Tests.Support;

public sealed class UserCredential
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>Shape of TestData/users.json.</summary>
public sealed class LoginUsers
{
    public UserCredential StandardUser { get; set; } = new();
    public UserCredential LockedOutUser { get; set; } = new();
}
