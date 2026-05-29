using FluentAssertions;
using SumX.Domain.Exceptions;
using SumX.Domain.ValueObjects;

namespace SumX.Domain.Tests.ValueObjects;

public class EmailAddressTests
{
    [Fact]
    public void Create_ShouldThrow_WhenEmailIsEmpty()
    {
        var act = () => EmailAddress.Create(" ");

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be empty*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenEmailIsInvalid()
    {
        var act = () => EmailAddress.Create("invalid-email");

        act.Should().Throw<DomainException>()
            .WithMessage("*is invalid*");
    }

    [Fact]
    public void Create_ShouldNormalizeEmail()
    {
        var email = EmailAddress.Create("  Admin@Tenant.COM ");

        email.Value.Should().Be("admin@tenant.com");
    }

    [Fact]
    public void Create_ShouldSupportValueObjectEquality()
    {
        var first = EmailAddress.Create("Admin@Tenant.COM");
        var second = EmailAddress.Create("admin@tenant.com");

        first.Should().Be(second);
    }
}
