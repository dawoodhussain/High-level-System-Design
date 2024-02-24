using Microsoft.OpenApi.Models;
using RangeHandlerService;

var builder = WebApplication.CreateBuilder(args);

var upperRangeLimit = Convert.ToInt32(builder.Configuration.GetValue<string>("UpperRangeLimit"));
var rangeSplit = Convert.ToInt32(builder.Configuration.GetValue<string>("RangeSplit"));

//swagger configuration
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Range Handler API Service",
        Version = "v1",
        Description = "App servers call this service to obtain their range for generating distributed sequence IDs"
    });
});

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
                 .ExecuteAsync(statusCodeContext.HttpContext));

//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

//end point configuration for requesting current sequence range
app.MapGet("/getrange", () => TypedResults.Ok(requestRange()));

app.UseSwagger().UseSwaggerUI();

app.Run();

Tuple<int, int> requestRange()
{
    return Utils.getCurrentRange(upperRangeLimit, rangeSplit);
}
