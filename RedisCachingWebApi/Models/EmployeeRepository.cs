using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace RedisCachingWebApi.Models
{
    public class EmployeeRepository
    {
        private readonly string _conStr;

        public EmployeeRepository(IConfiguration configuration)
        {
            _conStr = configuration.GetConnectionString("Employee");
        }

        public IDbConnection Connection => new SqlConnection(_conStr);

        // Insert
        public async Task<int> AddAsync(Employee employee)
        {
            using (IDbConnection connection = Connection)
            {
                string sql = @"INSERT INTO Employee (EmpName, EmpDesignation, ProjectName, Skill) 
                               VALUES (@EmpName, @EmpDesignation, @ProjectName, @Skill);
                               SELECT CAST(SCOPE_IDENTITY() as int);";
                connection.Open();
                var newEmpId = await connection.QuerySingleAsync<int>(sql, employee);
                return newEmpId;
            }
        }

        // Get all
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            using (IDbConnection connection = Connection)
            {
                string sql = @"SELECT EmployeeID, EmpName, EmpDesignation, ProjectName, Skill FROM Employee;";
                connection.Open();
                return await connection.QueryAsync<Employee>(sql);
            }
        }

        // Get by ID
        public async Task<Employee> GetByIdAsync(int id)
        {
            using (IDbConnection connection = Connection)
            {
                string sql = @"SELECT EmployeeID, EmpName, EmpDesignation, ProjectName, Skill 
                               FROM Employee WHERE EmployeeID = @EmployeeID;";
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { EmployeeID = id });
            }
        }

        // Update
        public async Task UpdateAsync(Employee employee)
        {
            using (IDbConnection connection = Connection)
            {
                string sql = @"UPDATE Employee 
                               SET EmpName = @EmpName, 
                                   EmpDesignation = @EmpDesignation, 
                                   ProjectName = @ProjectName, 
                                   Skill = @Skill 
                               WHERE EmployeeID = @EmployeeID;";
                connection.Open();
                await connection.ExecuteAsync(sql, employee);
            }
        }

        // Delete by ID
        public async Task DeleteByIdAsync(int id)
        {
            using (IDbConnection connection = Connection)
            {
                string sql = @"DELETE FROM Employee WHERE EmployeeID = @EmployeeID;";
                connection.Open();
                await connection.ExecuteAsync(sql, new { EmployeeID = id });
            }
        }

        // DELETE all employee data
        public async Task DeleteAllAsync()
        {
            using (IDbConnection connection = Connection)
            {
                string sql = @"DELETE FROM Employee;";
                connection.Open();
                await connection.ExecuteAsync(sql);
            }
        }
    }
}
