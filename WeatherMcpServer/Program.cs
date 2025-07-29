using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using WeatherMcpServer.Services;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddScoped<WeatherService>()
    .AddScoped<WeatherTools>()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<WeatherTools>();

var host = builder.Build();
#if DEBUG

//Console.WriteLine("1. ������� ������");
//Console.WriteLine("2. �������");
//Console.WriteLine("3. ��������������");
//var choice = Console.ReadLine();

var tools = host.Services.GetRequiredService<WeatherTools>();

//Console.Write("�����: ");
//var city = Console.ReadLine();

//var result = choice switch
//{
//    "1" => await tools.GetCityWeather(city),
//    "2" => await tools.GetWeatherForecast(city, days: 3),
//    "3" => await tools.GetWeatherAlerts(city),
//    _ => "�������� �����"
//};
//Console.WriteLine(result);

await tools.GetCurrentWeather("Moscow");

#endif
await host.RunAsync();