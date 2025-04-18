// See https://aka.ms/new-console-template for more information
using Bardez.EmptyFileFinder.Business;
using Bardez.EmptyFileFinder.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

//Dependency Injection
builder.Services.AddTransient<EmptyReporter>();
builder.Services.Configure<CheckerOptions>(builder.Configuration.GetSection(CheckerOptions.ConfigurationSection));

using IHost host = builder.Build();
var checker = host.Services.GetRequiredService<Checker>();
await checker.CheckForEmptyFiles(false, CancellationToken.None);
