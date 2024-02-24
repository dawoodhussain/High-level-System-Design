using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    rateLimiterOptions.AddFixedWindowLimiter("customPolicy", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(5);
    });
}); //RateLimiting congfigured to be invoked if we get more than 10 requests within 5secs/

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: "LBCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRateLimiter();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("LBCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy().RequireRateLimiting("customPolicy"); /*Disable this when testing concurrent requests*/
//app.MapReverseProxy(); /*Enable this when testing concurrent requests*/

app.MapHealthChecks("health");

app.Run();
