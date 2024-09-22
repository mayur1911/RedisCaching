using System.Data.SqlClient;
using System.Data;
using Dapper;
using RedisCachingWebApi.Application.Models;

namespace RedisCachingWebApi.Repositories
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
        public async Task<int> AddAsync(EmployeeModel employee)
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"INSERT INTO Employee (ManagerId, EmpName, EmpDesignation, ProjectName, Skill) 
                                   VALUES (@ManagerId, @EmpName, @EmpDesignation, @ProjectName, @Skill);
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
        public async Task<IEnumerable<EmployeeModel>> GetAllAsync()
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"SELECT EmployeeID, ManagerId, EmpName, EmpDesignation, ProjectName, Skill 
                                   FROM Employee;";
                    return await connection.QueryAsync<EmployeeModel>(sql);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error while fetching all employees", ex);
            }
        }

        // Get by ID
        public async Task<EmployeeModel> GetByIdAsync(int id)
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"SELECT EmployeeID, ManagerId, EmpName, EmpDesignation, ProjectName, Skill 
                                   FROM Employee WHERE EmployeeID = @EmployeeID;";
                    return await connection.QueryFirstOrDefaultAsync<EmployeeModel>(sql, new { EmployeeID = id });
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Error while fetching employee with ID {id}", ex);
            }
        }

        // Update
        public async Task UpdateAsync(EmployeeModel employee)
        {
            try
            {
                using (IDbConnection connection = Connection)
                {
                    string sql = @"UPDATE Employee 
                                   SET ManagerId = @ManagerId,
                                       EmpName = @EmpName, 
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
