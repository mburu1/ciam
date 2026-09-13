namespace Ciam.Domain.Enums;

public enum AuthenticationMethod
{
    UsernamePassword = 1,
    SecurityQuestion = 2,
    Passkey = 3,
    Totp = 4,
    EmailOtp = 5,
    MagicLink = 6
}
