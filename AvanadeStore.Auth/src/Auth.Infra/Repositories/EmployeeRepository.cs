using Auth.Domain.Entities;
using Auth.Domain.Interfaces;
using Auth.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infra.Repositories;
internal class EmployeeRepository : IEmployeeRepository
{
    private readonly AuthDbContext _context;

    public EmployeeRepository(AuthDbContext context)
    {
        _context = context;
    }
    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees.FindAsync(id);
    }
    public async Task<Employee?> GetByEmailAndPasswordAsync(string email, string passwordHash)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.IsActive && e.Email.Equals(email) && e.PasswordHash.Equals(passwordHash));
    }
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Employees.AnyAsync(e => e.Email.Equals(email));
    }
    public async Task<Employee?> AddAsync(Employee employee)
    {
       var employeeCreated = await _context.Employees.AddAsync(employee);
        return await Task.FromResult(employeeCreated.Entity);
    }
    public async Task<bool> UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        return await Task.FromResult(true);
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee is null)
            return false;
        else
        {
            employee.Deactivate();
            _context.Entry(employee).Property(x => x.IsActive).IsModified = true;
            return true;
        }
    }
}
