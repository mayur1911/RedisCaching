
# RedisCachingWebApi

## Introduction
RedisCachingWebApi is a .NET 7.0 Web API project that demonstrates how to implement caching using Redis. The API handles employee data and supports standard CRUD operations. The project runs inside Docker containers for seamless deployment and scalability, using SQL Server for data storage and Redis for caching.

## Technologies Used
- **.NET 7.0**: Framework for building Web APIs.
- **SQL Server**: Relational database used to store employee data.
- **Redis**: Used for distributed caching to improve application performance.
- **Docker**: Containers for Redis, SQL Server, and the Web API.
- **Dapper**: Lightweight ORM for database interactions.
- **StackExchange.Redis**: Library to interact with Redis.
- **SQL Server Management Studio (SSMS)**: For managing SQL Server.

## Architecture
- **Web API**: Handles HTTP requests and performs CRUD operations on employee data.
- **SQL Server**: Stores employee data in the database.
- **Redis**: Caches employee data to improve performance and reduce load on the database.
- **Docker Compose**: Manages the containers for the Web API, Redis, and SQL Server.

## Project Structure
```
RedisCachingWebApi/
├── Controllers/
│   └── EmployeeController.cs   # API controller for managing employees
├── Models/
│   └── Employee.cs             # Employee model
│   └── EmployeeRepository.cs   # Repository for database operations
├── sql-scripts/
│   └── init.sql                # SQL script to create Employee database and table
├── Dockerfile                  # Dockerfile for building Web API container
├── docker-compose.yml          # Docker Compose configuration for all services
├── appsettings.json            # Configuration for connection strings, Redis, etc.
├── Program.cs                  # Main entry point for the Web API
└── README.md                   # Project documentation
```

## Setup and Installation

### Prerequisites
- **Docker**: Make sure Docker and Docker Compose are installed.
- **.NET 7.0 SDK**: For running the application locally (optional if using Docker).
- **SQL Server Management Studio (SSMS)**: To manage the SQL Server (optional).

### Steps to Run the Project
1. **Clone the Repository**:
   ```bash
   https://github.com/mayur1911/RedisCaching.git
   cd RedisCachingWebApi
   ```

2. **Running with Docker**:
   Build and run the containers using Docker Compose:
   ```bash
   docker-compose up --build
   ```
   This will start the following services:
   - **Web API**: Available at `http://localhost:8080` and `https://localhost:8443`.
   - **Redis**: Available at `localhost:6379`.
   - **SQL Server**: Available at `localhost:1433`.

### Environment Variables
- **RedisConnection**: Connection string for Redis (set to `redis:6379` in Docker).
- **Employee Database Connection String**:
  ```json
  "ConnectionStrings": {
    "Employee": "Server=sqlserver;Database=Employee;User=sa;Password=YourPassword123!"
  }
  ```

## API Endpoints

### Base URL:
- **HTTP**: `http://localhost:8080/api/employee`
- **HTTPS**: `https://localhost:8443/api/employee`

### CRUD Endpoints:
1. **Get All Employees**:
   - **Method**: GET
   - **URL**: `/api/employee/get`
   - **Description**: Fetches all employees from the cache or database.
   - **Response**:
     ```json
     [
       {
         "EmployeeID": 1,
         "EmpName": "John Doe",
         "EmpDesignation": "Developer",
         "ProjectName": "Project Alpha",
         "Skill": "C#"
       }
     ]
     ```

2. **Get Employee by ID**:
   - **Method**: GET
   - **URL**: `/api/employee/get/{id}`
   - **Description**: Fetches an employee by ID from the cache or database.

3. **Add New Employee**:
   - **Method**: POST
   - **URL**: `/api/employee`
   - **Description**: Adds a new employee to the database.
   - **Request Body**:
     ```json
     {
       "EmpName": "Jane Smith",
       "EmpDesignation": "Manager",
       "ProjectName": "Project Beta",
       "Skill": "Management"
     }
     ```

4. **Update Employee**:
   - **Method**: PUT
   - **URL**: `/api/employee/{id}`
   - **Description**: Updates an existing employee in the database.

5. **Delete Employee by ID**:
   - **Method**: DELETE
   - **URL**: `/api/employee/{id}`
   - **Description**: Deletes an employee by ID from the database.

6. **Delete All Employees**:
   - **Method**: DELETE
   - **URL**: `/api/employee/deleteAll`
   - **Description**: Deletes all employee records from the database.

## Docker Setup

### Docker Services:
- **rediscachingwebapi**: The Web API service.
- **redis**: Redis cache server.
- **sqlserver**: SQL Server for database operations.
- **sql-init**: Initialization service to set up the SQL database schema.

### Docker Compose Commands:
- **Start Services**: `docker-compose up --build`
- **Stop Services**: `docker-compose down`
- **View Logs**: `docker-compose logs`

### Ports:
- **Web API**: 
  - HTTP: `8080`
  - HTTPS: `8443`
- **SQL Server**: `1433`
- **Redis**: `6379`

## Redis Caching
The application uses **Redis** to cache employee data to minimize calls to the SQL Server. The cache is configured with a TTL (time to live) of **600 seconds**.

### Cache Configuration:
In **appsettings.json**:
```json
"CacheSettings": {
  "EmployeeTtlSeconds": 600
}
```

The cache is implemented in the `EmployeeController` to retrieve employee data. If the data is found in the Redis cache, it is returned from there. If not, it queries the database, caches the result, and returns it.

## Testing
You can test the APIs using:
- **Postman**: Use the API endpoints to make GET, POST, PUT, and DELETE requests.
- **SQL Server Management Studio (SSMS)**: Connect to the SQL Server container (`localhost,1433`) to view the **Employee** database.

## License
This project is licensed under the MIT License.
