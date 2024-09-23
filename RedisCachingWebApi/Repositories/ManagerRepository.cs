using Dapper;
using RedisCachingWebApi.Domain;
using RedisCachingWebApi.Interface;
using System.Data;

namespace RedisCachingWebApi.Repositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly IDbConnection _dbConnection;

        public ManagerRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Add a new manager
        public async Task<int> AddManagerDataAsync(ManagerData manager)
        {
            var parameters = new DynamicParameters();

            // Set input parameters
            parameters.Add("@ManagerID", manager.ManagerID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ManagerName", manager.ManagerName);
            parameters.Add("@ManagerDesignation", manager.ManagerDesignation);
            parameters.Add("@ProjectName", manager.ProjectName);

            // Set the output parameter
            parameters.Add("@newId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Execute the stored procedure
            await _dbConnection.ExecuteAsync("sp_AddOrUpdateManager", parameters, commandType: CommandType.StoredProcedure);

            // Retrieve the output value (new ManagerID)
            return parameters.Get<int>("@newId");
        }

        public async Task<ManagerData> GetManagerDataByIdAsync(int managerDataId)
        {
            var parameter = new DynamicParameters();

            parameter.Add("@ManagerID", managerDataId, DbType.Int32, ParameterDirection.Input);
            return await _dbConnection.QueryFirstOrDefaultAsync<ManagerData>("sp_GetManagerById", parameter, commandType: CommandType.StoredProcedure);
        }
    }
}