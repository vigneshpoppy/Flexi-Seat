using System.ComponentModel;
using System.Text.Json;
using FlexiSeat.DbContext;
using FlexiSeat.Models;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly string _connectionString;
        private readonly OpenApiSettings _settings;
        public AIController(FlexiSeatDbContext context, IConfiguration configuration, IOptions<OpenApiSettings> options)
        {
            _settings = options.Value;
            _context = context;
            _connectionString = configuration.GetConnectionString("MyConnectionString");
        }

        [HttpPost("bot")]
        public async Task<IActionResult> GenerateSQLFromPrompt([FromBody] string prompt, [FromQuery] string adid)
        {
            try
            {
                var additionalPrompt = string.Empty;
                bool isManager = await _context.Users
                        .AnyAsync(u => u.ManagerADID == adid);
                string userAdidList = string.Join(",",
                    await _context.Users
                        .Where(u => u.ManagerADID == adid)
                        .Select(u => u.ADID)
                        .ToListAsync());
                additionalPrompt = "You are an AI agent that generates SQL queries for the FlexiSeatsDb database.  \r\nUse the provided schema and follow these access rules strictly:\r\n\r\n1) Users(if they are manager or not) can view their own information such as ADID, Designation, and Manager Name.  \r\n2) Users if they are managers can view their own information as well as the ADID, Name, and Designation of users who report to them.They cannot view info about users not reporting to them.  \r\n3) Users can view available seats (Seat Numbers) for a specific date—these are seats from the Seats table not reserved in the Reservations table on that date.  \r\n4) Users, whether they are manager or not, can view their own reservation details such as ReservedDate and Seat Number.  \r\n5) Managers can  view their reservation details and also for their direct reporting users (UserADID, ReservedDate, Seat Number).  \r\n6) If a user is not a manager they cannot access any details of other ppl information like user details, seat reservation. In that case return plain string as No Access instead of sql query\r\n";


                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.Secret}");

                string schemaFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "schema.txt");
                string schema = await System.IO.File.ReadAllTextAsync(schemaFilePath);

                var request = new
                {
                    model = "gpt-4",
                    messages = new[]
                    {
            new { role = "system", content = $"You are a SQL assistant. You generate SQL queries based on table schema. Only bring the perfect SQL SELECT statement only. Bring data based on adid {adid}. {additionalPrompt}.IsManager: {isManager} users reporting to this adid {userAdidList}" },
            new { role = "user", content = $"Schema:\n{schema}\n\nQuestion: {prompt} Bring data based on adid {adid}.IsManager: {isManager} users reporting to this adid {userAdidList}. {additionalPrompt}" }
                }
                };

                var response = await httpClient.PostAsJsonAsync(_settings.ConnectionString, request);
                var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
                var res = result?.Choices?.FirstOrDefault()?.Message?.Content;
                var final = await ExecuteSqlQueryAsync(res);
                return Ok(final);
            }
            catch (Exception ex)
            {
                return Ok("Error");
            }

        }
        private async Task<string> ExecuteSqlQueryAsync(string sql)
        {
            string gptContent = string.Empty;
            if (sql == "No Access")
            {
                gptContent = "You dont have access to that information";
                return gptContent;
            }
            var results = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                results.Add(row);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(results, options);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.Secret}");

            var request = new
            {
                model = "gpt-4",
                messages = new[]
                {
            new { role = "system", content = $"Write the json data into human readable sentence that makes sense" },
            new { role = "user", content = $"json:\n{jsonString}\n\nQuestion: Write the json data into human readable sentence that makes sense. Just precise info no additional content. Arrange each sentence line by line" }
                }
            };

            var response = await httpClient.PostAsJsonAsync(_settings.ConnectionString,  request);
            string content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            gptContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (gptContent == "There is no data")
            {
                gptContent = "No data available for the request";
            }
            return gptContent;
        }
    }
}
