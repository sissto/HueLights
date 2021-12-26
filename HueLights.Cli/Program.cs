// See https://aka.ms/new-console-template for more information

using HueLights.Cli;
using HueLights.Cli.Commands;
using HueLights.Cli.Commands.Bridges;
using HueLights.Cli.Commands.Lights;
using HueLights.Cli.Infrastructure;
using HueLights.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var registrations = new ServiceCollection();
registrations.AddSingleton<SettingsService>(factory => new SettingsService(Constants.APPNAME));
registrations.AddTransient<HueService>(factory =>
{
  var settingsService = factory.GetService<SettingsService>();
  return new HueService(settingsService);
});

var registrar = new TypeRegistrar(registrations);

var app = new CommandApp(registrar);
app.Configure(config =>
{
#if DEBUG
  config.PropagateExceptions();
  config.ValidateExamples();
#endif

  //config.SetInterceptor(new CommandInterceptor());

  config.AddCommand<InitCommand>("init");
  config.AddCommand<LocateBridgesCommand>("locate");
  config.AddBranch("lights", lights =>
  {
    lights.AddCommand<GetAllLightsCommand>("all");
  });
});
app.Run(args);