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
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"INSERT INTO Employee (EmpName, EmpDesignation, ProjectName, Skill) 
                                   VALUES (@EmpName, @EmpDesignation, @ProjectName, @Skill);
                                   SELECT CAST(SCOPE_IDENTITY() as int);";
                    return await connection.QuerySingleAsync<int>(sql, employee);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error while adding employee", ex);
            }
        }

        // Get all
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"SELECT EmployeeID, EmpName, EmpDesignation, ProjectName, Skill FROM Employee;";
                    return await connection.QueryAsync<Employee>(sql);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error while fetching all employees", ex);
            }
        }

        // Get by ID
        public async Task<Employee> GetByIdAsync(int id)
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"SELECT EmployeeID, EmpName, EmpDesignation, ProjectName, Skill 
                                   FROM Employee WHERE EmployeeID = @EmployeeID;";
                    return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { EmployeeID = id });
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Error while fetching employee with ID {id}", ex);
            }
        }

        // Update
        public async Task UpdateAsync(Employee employee)
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"UPDATE Employee 
                                   SET EmpName = @EmpName, 
                                       EmpDesignation = @EmpDesignation, 
                                       ProjectName = @ProjectName, 
                                       Skill = @Skill 
                                   WHERE EmployeeID = @EmployeeID;";
                    await connection.ExecuteAsync(sql, employee);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Error while updating employee with ID {employee.EmployeeID}", ex);
            }
        }

        // Delete by ID
        public async Task DeleteByIdAsync(int id)
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"DELETE FROM Employee WHERE EmployeeID = @EmployeeID;";
                    await connection.ExecuteAsync(sql, new { EmployeeID = id });
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Error while deleting employee with ID {id}", ex);
            }
        }

        // DELETE all employee data
        public async Task DeleteAllAsync()
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"DELETE FROM Employee;";
                    await connection.ExecuteAsync(sql);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error while deleting all employees", ex);
            }
        }
    }
}
