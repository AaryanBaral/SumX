namespace SumX.API.Models.Employee
{
    public sealed record UpdateEmployeeRequest(
        string FullName,
        string Email);
}
