using AppServer4;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var url = builder.Configuration.GetValue<string>("rangeHandlerServiceUrl");
var seqGen = new SequenceGenerator();
app.MapGet("/", () => "Hello From Server 4");
app.MapGet("/getId", () => seqGen.getSequenceId(url));

app.Run();
