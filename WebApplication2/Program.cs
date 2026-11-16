
using Microsoft.VisualBasic;
using OpenAI.Responses;



//obj ادارة المشروع قبل تشغيله
var builder = WebApplication.CreateBuilder(args);

//add controollers لتنظيم الكود
builder.Services.AddControllers();


//build app لاستقبال الريكويست
var app = builder.Build();


//تفعيل الكونرولر
app.MapControllers();

//endpoint مسار من نوع get post delete update
//لما اليوزر بزور اللينك يقوم السيرفسر بالرد
app.MapGet("/", () => "Hello World!");
app.MapPost("/Login", () => "Login");
app.MapPost("/signup", () => "sign up");

//تشغيل السيرفر لاستقبال الريكويست 
app.Run();
