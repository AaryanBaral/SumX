namespace SumX.Domain.Constants;

public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Employee = "Employee";

    public static readonly IReadOnlySet<string> AllRoles = new HashSet<string>(StringComparer.Ordinal)
    {
        SuperAdmin,
        Admin,
        Employee
    };

    public static bool IsValid(string role) =>
        !string.IsNullOrWhiteSpace(role) && AllRoles.Contains(role);
}
