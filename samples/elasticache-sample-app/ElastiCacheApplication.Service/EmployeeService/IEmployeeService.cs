using ElasticCacheApplication.Entities;

namespace ElasticCacheApplication.Service.EmployeeService
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<Employee>?> GetEmployees();

        public Task<Employee?> GetEmployeeById(Guid guid);

        public Task AddEmployee(Employee employee);

        public Task DeleteEmployee(Guid guid);

        public Task DeleteAllEmployee();


    }
}
