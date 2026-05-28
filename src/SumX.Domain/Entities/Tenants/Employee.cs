using SumX.Domain.Exceptions;

namespace SumX.Domain.Entities.Tenants;

public sealed class Employee
{
    private Employee(
        string id,
        string fullName,
        string email)
    {
        Id = ValidateRequired(id, "Employee id");
        FullName = ValidateRequired(fullName, "Employee full name");
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public string Id { get; }

    public string FullName { get; private set; }

    public string Email { get; private set; }

    public static Employee Create(
        string id,
        string fullName,
        string email) =>
        new(id, fullName, email);

    public void Rename(string fullName)
    {
        FullName = ValidateRequired(fullName, "Employee full name");
    }

    public void ChangeEmail(string email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    private static string ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        return value.Trim();
    }
}
