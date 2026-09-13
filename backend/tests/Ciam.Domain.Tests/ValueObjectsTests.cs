using Ciam.Domain.Exceptions;
using Ciam.Domain.ValueObjects;

namespace Ciam.Domain.Tests;

public sealed class ValueObjectsTests
{
    [Theory]
    [InlineData(" person@example.com ", "person@example.com")]
    [InlineData("person+tag@example.co.uk", "person+tag@example.co.uk")]
    public void EmailAddress_Create_normalizes_valid_values(string input, string expected)
    {
        var email = EmailAddress.Create(input);

        Assert.Equal(expected, email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("person@")]
    public void EmailAddress_Create_rejects_invalid_values(string input)
    {
        Assert.Throws<InvalidEmailAddressException>(() => EmailAddress.Create(input));
    }

    [Fact]
    public void FullName_Create_trims_parts_and_builds_display_name()
    {
        var name = FullName.Create(" Ada ", " Lovelace ");

        Assert.Equal("Ada", name.FirstName);
        Assert.Equal("Lovelace", name.LastName);
        Assert.Equal("Ada Lovelace", name.DisplayName);
    }

    [Fact]
    public void PreferredUsername_Create_rejects_unsupported_characters()
    {
        Assert.Throws<InvalidUsernameException>(() => PreferredUsername.Create("user name"));
    }

    [Theory]
    [InlineData("+254712345678")]
    [InlineData("254712345678")]
    public void PhoneNumber_Create_accepts_e164_values(string input)
    {
        var phone = PhoneNumber.Create(input);

        Assert.Equal(input, phone.Value);
    }
}
