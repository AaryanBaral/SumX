using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SumX.Domain.Entities.Tenants;

namespace SumX.Application.Common.Abstractions.Persistence.Tenants
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(Guid id, bool trackChanges = false);
        Task<Employee?> GetByEmailAsync(string email, bool trackChanges = false);
        Task<IEnumerable<Employee>> GetAllAsync(bool trackChanges = false);
        Task CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Guid id);
    }
}
