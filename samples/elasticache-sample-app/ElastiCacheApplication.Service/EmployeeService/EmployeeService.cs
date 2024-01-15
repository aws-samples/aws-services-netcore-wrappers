using Amazon.ElastiCache.Wrapper.Interfaces;
using ElasticCacheApplication.Entities;
using System.Net;
using System.Text.Json;

namespace ElasticCacheApplication.Service.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRedisCacheRepository _cache;

        public EmployeeService(IRedisCacheRepository redisCacheRepository)
        {
            _cache = redisCacheRepository;
        }

        public async Task AddEmployee(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid();

            List<Employee>? employees = await GetEmployeesFromCache();

            if (employees != null)
                employees.Add(employee);

            await _cache.SetStringAsync("employees", JsonSerializer.Serialize(employees));
        }

        public async Task DeleteAllEmployee()
        {
            await _cache.RemoveAsync("employees");
        }

        public async Task DeleteEmployee(Guid guid)
        {
            List<Employee>? employees = await GetEmployeesFromCache();

            if (employees != null)
            {
                var employee = employees.First(x => x.EmployeeId == guid);

                if (employee != null)
                {
                    employees.Remove(employee);
                    await _cache.SetStringAsync("employees", JsonSerializer.Serialize(employees));
                }
            }
        }

        public async Task<Employee?> GetEmployeeById(Guid guid)
        {
            List<Employee>? employees = await GetEmployeesFromCache();

            var employee = new Employee();
            if (employees != null)
                employee = employees.FirstOrDefault(x => x.EmployeeId == guid);
            return employee;
        }

        public async Task<IEnumerable<Employee>?> GetEmployees()
        {
            return await GetEmployeesFromCache();
        }

        private async Task<List<Employee>?> GetEmployeesFromCache()
        {
            var employeesCached = await _cache.GetStringAsync("employees");

            List<Employee>? employees;
            if (!string.IsNullOrEmpty(employeesCached))
                employees = JsonSerializer.Deserialize<List<Employee>>(employeesCached);
            else
                employees = new List<Employee>();
            return employees;
        }
    }
}
