namespace SumX.Application.Common.Abstractions;

public interface IMasterDbSeeder
{
    /// <returns>True when seeding ran; false when data already exists.</returns>
    Task<bool> SeedAsync(CancellationToken cancellationToken = default);
}
