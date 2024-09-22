namespace RedisCachingWebApi.Domain
{
    public class ManagerData
    {
        // Primary Key with Identity
        public int ManagerID { get; set; }

        // Manager Name (Required, max length 20)
        public string ManagerName { get; set; }

        // Manager Designation (Optional, max length 20)
        public string ManagerDesignation { get; set; }

        // Project Name (Optional, max length 20)
        public string ProjectName { get; set; }

        // newId output parameter for the stored procedure
        public int NewId { get; set; }  // Dapper will automatically treat this as an output parameter
    }
}