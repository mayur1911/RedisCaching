using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisCachingWebApi.Models;
using StackExchange.Redis;

namespace RedisCachingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly string cacheKey = "employeeData";
        private readonly IDistributedCache _distributedCache;
        private readonly EmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IDatabase _redisDb;

        private readonly int ttlTimeInSeconds; // Set via constructor
        public EmployeeController(
            ILogger<EmployeeController> logger,
            IDistributedCache distributedCache,
            IConnectionMultiplexer redis,
            EmployeeRepository employeeRepository,
            IConfiguration configuration)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _redisDb = redis.GetDatabase();
            _employeeRepository = employeeRepository;
            ttlTimeInSeconds = configuration.GetValue<int>("CacheSettings:EmployeeTtlSeconds");
        }

        // Get all employees
        [HttpGet("get")]
        public async Task<IActionResult> GetAll()
        {
            var employees = new List<Employee>();
            try
            {
                var serializedList = await _redisDb.StringGetAsync(cacheKey);
                if (!serializedList.IsNullOrEmpty)
                {
                    employees = JsonConvert.DeserializeObject<List<Employee>>(serializedList);
                }
                else
                {
                    employees = (await _employeeRepository.GetAllAsync()).ToList();
                    serializedList = JsonConvert.SerializeObject(employees);
                    await _redisDb.StringSetAsync(cacheKey, serializedList, TimeSpan.FromSeconds(ttlTimeInSeconds));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while fetching employees.");
                return StatusCode(500, "Internal server error");
            }
            return Ok(employees);
        }

        // Get employee by ID
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            _logger.LogInformation("Get employee details for ID: {id}", id);
            if (id <= 0)
            {
                return BadRequest("Invalid ID entered.");
            }

            var employeeCacheKey = $"{cacheKey}{id}";
            Employee employee = null;
            try
            {
                // Try to get the employee from Redis cache
                var serializedEmployee = await _redisDb.StringGetAsync(employeeCacheKey);
                if (!serializedEmployee.IsNullOrEmpty)
                {
                    employee = JsonConvert.DeserializeObject<Employee>(serializedEmployee);
                }
                else
                {
                    employee = await _employeeRepository.GetByIdAsync(id);
                    if (employee == null)
                    {
                        return NotFound($"Employee with ID {id} not found.");
                    }
                    // Cache the employee data in Redis
                    serializedEmployee = JsonConvert.SerializeObject(employee);
                    await _redisDb.StringSetAsync(employeeCacheKey, serializedEmployee, TimeSpan.FromSeconds(ttlTimeInSeconds));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while fetching employee with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
            return Ok(employee);
        }

        // Insert a new employee
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newEmpId = await _employeeRepository.AddAsync(employee);
                employee.EmployeeID = newEmpId;

                // Invalidate cache
                await InvalidateCache();

                // Cache the newly added employee
                var employeeCacheKey = $"{cacheKey}{newEmpId}";
                var serializedEmployee = JsonConvert.SerializeObject(employee);
                await _redisDb.StringSetAsync(employeeCacheKey, serializedEmployee, TimeSpan.FromSeconds(ttlTimeInSeconds));

                return Ok(new { Message = "Employee added successfully.", EmployeeID = newEmpId });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while adding a new employee.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Update an employee
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            _logger.LogInformation("Updating employee details for ID: {id}", id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingEmployee = await _employeeRepository.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    return NotFound($"Employee with ID {id} not found.");
                }

                employee.EmployeeID = id;
                await _employeeRepository.UpdateAsync(employee);

                // Invalidate cache
                await InvalidateCache(id);

                // Cache the updated employee
                var serializedEmployee = JsonConvert.SerializeObject(employee);
                await _redisDb.StringSetAsync($"{cacheKey}{id}", serializedEmployee, TimeSpan.FromSeconds(ttlTimeInSeconds));

                return Ok("Employee updated successfully.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while updating employee with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Delete an employee by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting employee details for ID: {id}", id);
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                {
                    return NotFound($"Employee with ID {id} not found.");
                }

                await _employeeRepository.DeleteByIdAsync(id);

                // Invalidate cache
                await InvalidateCache(id);

                return Ok("Employee deleted successfully.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while deleting employee with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Delete all employees
        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            _logger.LogInformation("Deleting all employee details");
            try
            {
                await _employeeRepository.DeleteAllAsync();

                // Clear the cache
                await _redisDb.KeyDeleteAsync(cacheKey);

                // Optionally, remove all individual employee caches
                var keys = _redisDb.Multiplexer.GetServer(_redisDb.Multiplexer.GetEndPoints().First()).Keys(pattern: $"{cacheKey}*");
                foreach (var key in keys)
                {
                    await _redisDb.KeyDeleteAsync(key);
                }

                return Ok("All employee data deleted successfully.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while deleting all employees.");
                return StatusCode(500, "Internal server error");
            }
        }

        // Private method to invalidate cache
        private async Task InvalidateCache(int? id = null)
        {
            if (id.HasValue)
            {
                // Invalidate specific employee cache
                var employeeCacheKey = $"{cacheKey}{id.Value}";
                await _redisDb.KeyDeleteAsync(employeeCacheKey);
            }
            // Invalidate entire employee list cache
            await _redisDb.KeyDeleteAsync(cacheKey);
        }
    }
}