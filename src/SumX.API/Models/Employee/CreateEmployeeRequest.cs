namespace SumX.API.Models.Employee
{
    public sealed record CreateEmployeeRequest(
        string FullName,
        string Email);
}
