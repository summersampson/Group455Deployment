var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // <-- your frontend dev server
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});



builder.Services.AddControllers(); // keep this
var app = builder.Build();

app.UseCors("AllowReactApp");

app.UseAuthorization();
app.MapControllers();
app.Run();
