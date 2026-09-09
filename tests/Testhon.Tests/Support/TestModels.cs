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

public sealed class GajabUser
{
    public string MobileNumber { get; set; } = string.Empty;

    /// <summary>Staging accepts a fixed OTP for every mobile number - no real SMS is sent.</summary>
    public string Otp { get; set; } = string.Empty;
}

/// <summary>Shape of TestData/gajab_users.json.</summary>
public sealed class GajabUsers
{
    public GajabUser StandardUser { get; set; } = new();
}
