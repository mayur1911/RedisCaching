namespace RedisCachingWebApi.Application.Models
{
    public class ManagerModel
    {
        // Primary Key with Identity
        public int ManagerID { get; set; }

        // Manager Name (Required, max length 20)
        public string ManagerName { get; set; }

        // Manager Designation (Optional, max length 20)
        public string ManagerDesignation { get; set; }

        // Project Name (Optional, max length 20)
        public string ProjectName { get; set; }
    }
}