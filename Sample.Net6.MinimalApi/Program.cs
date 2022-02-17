var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/Query", () =>
{
    return new
    {
        Id = 1,
        Name = "Odin",
        Age = 20
    };
}).WithName("Query");

app.MapPost("/Add", () =>
{
    return new
    {
        Success = true,
        Message = "新增成功"
    };
}).WithName("Add");

app.MapPut("/Update", () =>
{
    return new
    {
        Success = true,
        Message = "更新成功"
    };
}).WithName("Update");

app.MapDelete("/Delete", () =>
{
    return new
    {
        Success = true,
        Message = "刪除成功"
    };
}).WithName("Delete");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}