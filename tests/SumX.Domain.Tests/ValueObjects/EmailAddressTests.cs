using FluentAssertions;
using SumX.Domain.Exceptions;

namespace SumX.Domain.Tests.ValueObjects;

public class EmailAddressTests
{
    [Fact]
    public void Create_ShouldThrow_WhenEmailIsEmpty()
    {
        var act = () => " ";

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be empty*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenEmailIsInvalid()
    {
        var act = () => "invalid-email";

        act.Should().Throw<DomainException>()
            .WithMessage("*is invalid*");
    }

    [Fact]
    public void Create_ShouldNormalizeEmail()
    {
        var email = "  Admin@Tenant.COM ";

        email.Should().Be("admin@tenant.com");
    }

    [Fact]
    public void Create_ShouldSupportValueObjectEquality()
    {
        var first = "Admin@Tenant.COM";
        var second = "admin@tenant.com";

        first.Should().Be(second);
    }
}
