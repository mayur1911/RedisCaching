namespace RedisCachingWebApi.Application.Models
{
    public class EmployeeModel
    {
        // Primary Key with Identity
        public int EmployeeID { get; set; }

        // Employee Name (Required, max length 20)
        public string EmpName { get; set; }

        public string ManagerId { get; set; }

        // Employee Designation (Optional, max length 20)
        public string EmpDesignation { get; set; }

        // Project Name (Optional, max length 20)
        public string ProjectName { get; set; }

        // Skill (Optional, max length 20)
        public string Skill { get; set; }
    }
}