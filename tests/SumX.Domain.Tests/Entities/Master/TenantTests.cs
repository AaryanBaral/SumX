using FluentAssertions;
using SumX.Domain.Exceptions;
using MasterTenant = SumX.Domain.Entities.Master.Tenant;

namespace SumX.Domain.Tests.Entities.Master;

public class TenantTests
{
    [Fact]
    public void Create_ShouldSucceed_WhenTenantIsValid()
    {
        var tenant = MasterTenant.Create(
            "tenant-id-1",
            "Acme Inc",
            "admin@acme.com",
            "acme",
            "Host=localhost;Database=ACME;Username=postgres;Password=postgres");

        tenant.Id.Should().Be("tenant-id-1");
        tenant.Name.Should().Be("Acme Inc");
        tenant.Email.Should().Be("admin@acme.com");
        tenant.TenantId.Should().Be("ACME");
        tenant.DatabaseConnectionString.Should().Contain("Database=ACME");
        tenant.GetDatabaseName().Should().Be("ACME");
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC")]
    [InlineData("ABCDE")]
    public void Create_ShouldThrow_WhenTenantIdIsNotFourCharacters(string tenantId)
    {
        var act = () => MasterTenant.Create(
            "tenant-id-1",
            "Acme Inc",
            "admin@acme.com",
            tenantId,
            "Host=localhost;Database=ACME;Username=postgres;Password=postgres");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_ShouldThrow_WhenConnectionStringIsEmpty()
    {
        var act = () => MasterTenant.Create(
            "tenant-id-1",
            "Acme Inc",
            "admin@acme.com",
            "ACME",
            "");

        act.Should().Throw<DomainException>()
            .WithMessage("*connection string is required*");
    }
}
