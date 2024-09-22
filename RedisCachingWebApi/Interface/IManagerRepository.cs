using RedisCachingWebApi.Domain;

namespace RedisCachingWebApi.Interface
{
    public interface IManagerRepository
    {
        // Method to add a new ManagerData
        Task<int> AddManagerDataAsync(ManagerData ManagerData);

        // Method to get a ManagerData by ID
        //Task<ManagerData> GetManagerDataByIdAsync(int ManagerDataId);

        //// Method to delete a ManagerData by ID
        //Task DeleteManagerDataByIdAsync(int ManagerDataId);

        //// Method to get all ManagerDatas
        //Task<IEnumerable<ManagerData>> GetAllManagerDatasAsync();
    }
}