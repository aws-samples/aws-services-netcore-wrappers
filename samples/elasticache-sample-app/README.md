# Sample Employee API using ElastiCache Wrapper

This is a sample Employee API application written in ASP.NET 6 to explain how to use ElastiCache repository in real-world application.

## Data Access Patterns
1. Get all employees
2. Get employees by id
3. Add employee
4. Delete employee
5. Delete all employees


## API Endpoints
| Operation      | Endpoint | Type | DynamoDB Operation|
| ----------- | ----------- | ----------- | ----------- |
| Get Employee List | /api/employee/list | GET| Get all employee list |
| Get Employee | /api/employee/\{id} | GET | Get employee details|
| Add Employee | /api/employee | POST |
| Delete Employee | /api/employee/\{id} | DELETE| Delete employee |
| Delete Employee list | /api/employee/all | DELETE| Delete employee list|

## How to test endpoints
1. Set **ElastiCacheApplication.API** as a startup project and hit F5.
2. This launches the application and opens a swagger UI page.
3. First create a employees records by hitting the POST endpoints of API using swagger UI.
4. Finally, test GET and DELETE endpoints by selecting and deleting records.
