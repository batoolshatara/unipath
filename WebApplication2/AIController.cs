using Microsoft.AspNetCore.Mvc;
using OpenAI.Responses;
using AIIntegration.Models;
using System.Text.Json;


namespace AIIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly ResponsesClient _client;

        public AIController(IConfiguration configuration)
        {
            string apiKey = configuration["OpenAI:ApiKey"]
                ?? throw new Exception("API Key is missing");

            _client = new ResponsesClient(apiKey);
        }
        // 1. بيانات طلاب
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = new[]
            {
                new { FirstName = "أحمد", MiddleName = "محمد", LastName = "علي", BirthDate = "2001-05-12", Major = "Software Engineering", Courses = new[] { "Data Structures", "Operating Systems" } },
                new { FirstName = "سارة", MiddleName = "أحمد", LastName = "خالد", BirthDate = "2002-08-20", Major = "Computer Science", Courses = new[] { "Artificial Intelligence", "Computer Networks" } },
                new { FirstName = "عمر", MiddleName = "محمود", LastName = "حسن", BirthDate = "2000-03-15", Major = "Cybersecurity", Courses = new[] { "Advanced Cryptography", "Penetration Testing" } },
                new { FirstName = "فاطمة", MiddleName = "رائد", LastName = "السعيد", BirthDate = "2003-11-02", Major = "Computer Information Systems", Courses = new[] { "Systems Analysis and Design", "Advanced Databases" } },
                new { FirstName = "يوسف", MiddleName = "إبراهيم", LastName = "ناصر", BirthDate = "2001-01-25", Major = "Data Science", Courses = new[] { "Statistical Analysis", "Data Mining" } },
                new { FirstName = "مريم", MiddleName = "سامي", LastName = "العبدالله", BirthDate = "2002-07-14", Major = "Computer Engineering", Courses = new[] { "Logic Design", "Signals and Systems" } },
                new { FirstName = "حمزة", MiddleName = "توفيق", LastName = "العمري", BirthDate = "2000-09-30", Major = "Computer Science", Courses = new[] { "Advanced Algorithms", "Parallel Programming" } },
                new { FirstName = "لين", MiddleName = "بسام", LastName = "الخطيب", BirthDate = "2003-04-18", Major = "Web Design and Development", Courses = new[] { "Frontend Development", "Interactive Web Applications" } },
                new { FirstName = "خالد", MiddleName = "جمال", LastName = "التركماني", BirthDate = "2001-12-05", Major = "Networks and Communications", Courses = new[] { "Network Security", "Router Configuration" } },
                new { FirstName = "ريم", MiddleName = "معاذ", LastName = "الشريف", BirthDate = "2002-06-22", Major = "Software Engineering", Courses = new[] { "Advanced Software Engineering", "Software Testing and Quality Assurance" } }
            };

            return Ok(users);
        }

       [HttpPost("match-opportunity")]
public async Task<IActionResult> MatchStudentWithOpportunity([FromBody] ChatRequest request)
{
    // 1. مصفوفة الشركات والفرص المعرفة برمجياً تماماً مثل طريقة الطلاب
    var companies = new[]
    {
        new {  CompanyName = "Tech Solutions", Industry = "Software Development", Location = "Amman, Jordan", Opportunities = new[] { new { Title = "Frontend Developer" }, new { Title = "QA Engineer" } } },
        new { CompanyName = "Cyber Shield",    Industry = "Information Security", Location = "Irbid, Jordan", Opportunities = new[] { new { Title = "Junior Penetration Tester" }, new { Title = "SOC Analyst" } } },
        new { CompanyName = "Data Minds",      Industry = "Data Science & AI",    Location = "Remote",        Opportunities = new[] { new { Title = "Junior Data Analyst" }, new { Title = "AI Research Assistant" } } }
    };

    // تحويل مصفوفة الشركات إلى نص JSON لتمريره تلقائياً للذكاء الاصطناعي
    string companiesJson = JsonSerializer.Serialize(companies);

    // 2. بناء الـ Prompt مع دمج داتا الشركات بصيغة نظيفة
    string prompt = $@"
    Based on the following student information:
    ""{request.Question}""

    And these are the available companies and opportunities in the system:
    {companiesJson}
    
    Based on the student's major and courses, select the most suitable opportunity from this list, and explain why you chose it in English. Return the response in clean JSON format.";

    var options = new CreateResponseOptions
    {
        Model = "gpt-6-luna",
    };

    options.InputItems.Add(ResponseItem.CreateUserMessageItem(prompt));

    var response = await _client.CreateResponseAsync(options);
    string answer = response.Value.GetOutputText();
    
    var json = JsonSerializer.Deserialize<JsonElement>(answer);

    return Ok(new
    {
        StudentInput = request.Question,
        AiRecommendation = json
    });
}

        [HttpPost("Ask")]
        public async Task<IActionResult> AskAI(
            
            [FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Question is required");
            }


            var options = new CreateResponseOptions
            {
                Model = "gpt-6-luna",
                ReasoningOptions = new ResponseReasoningOptions
                {
                    ReasoningEffortLevel =
                        ResponseReasoningEffortLevel.None
                }
            };


            options.InputItems.Add(
                ResponseItem.CreateUserMessageItem(request.Question)
            );




            var response = await _client.CreateResponseAsync(options);


            string answer = response.Value.GetOutputText();

            return Ok(new
            {
                Question = request.Question,
                Answer = answer
            });
        }
    }
    
}