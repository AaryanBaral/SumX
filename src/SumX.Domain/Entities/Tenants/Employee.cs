using SumX.Domain.Exceptions;

namespace SumX.Domain.Entities.Tenants;

public sealed class Employee
{
    private Employee(
        Guid id,
        string fullName,
        string email)
    {
        Id = ValidateId(id);
        FullName = ValidateRequired(fullName, "Employee full name");
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public Guid Id { get; }

    public string FullName { get; private set; }

    public string Email { get; private set; }

    public static Employee Create(
        Guid id,
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

    private static Guid ValidateId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Employee id cannot be empty.");
        }
        return id;
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
