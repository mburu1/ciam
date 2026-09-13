using System.Security.Claims;
using Ciam.Api;
using Microsoft.AspNetCore.Http;

namespace Ciam.Api.Tests;

public sealed class HttpContextCurrentUserTests
{
    [Fact]
    public void Current_user_reads_authenticated_subject_and_guid()
    {
        var subject = Guid.NewGuid();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim("sub", subject.ToString("D"))],
                authenticationType: "test"))
        };
        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new HttpContextCurrentUser(accessor);

        Assert.True(currentUser.IsAuthenticated);
        Assert.Equal(subject.ToString("D"), currentUser.Subject);
        Assert.Equal(subject, currentUser.UserId);
    }

    [Fact]
    public void Current_user_is_anonymous_without_http_context()
    {
        var currentUser = new HttpContextCurrentUser(new HttpContextAccessor());

        Assert.False(currentUser.IsAuthenticated);
        Assert.Null(currentUser.Subject);
        Assert.Null(currentUser.UserId);
    }
}
