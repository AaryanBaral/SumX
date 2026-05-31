using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SumX.Application.Common.Abstractions.Persistence.Tenants;
using SumX.Domain.Entities.Tenants;

namespace SumX.Infrastructure.Persistence.Tenants.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly TenantDbContext _context;

        public EmployeeRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(Guid id, bool trackChanges = false)
        {
            return trackChanges
                ? await _context.Employees.FirstOrDefaultAsync(e => e.Id == id)
                : await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee?> GetByEmailAsync(string email, bool trackChanges = false)
        {
            return trackChanges
                ? await _context.Employees.FirstOrDefaultAsync(e => e.Email == email)
                : await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(bool trackChanges = false)
        {
            return trackChanges
                ? await _context.Employees.ToListAsync()
                : await _context.Employees.AsNoTracking().ToListAsync();
        }

        public async Task CreateAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}
