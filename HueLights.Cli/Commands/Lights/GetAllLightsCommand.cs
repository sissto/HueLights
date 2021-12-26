using HueLights.Infrastructure.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace HueLights.Cli.Commands.Lights
{
  public class GetAllLightsCommand : AsyncCommand
  {
    private readonly IAnsiConsole _console;
    private readonly HueService _hueService;

    public GetAllLightsCommand(IAnsiConsole console, HueService hueService)
    {
      _console = console;
      _hueService = hueService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
      var allLights = await _hueService.GetAllLights();

      if (!allLights.Any())
      {
        _console.WriteLine("No lights found!");
        return 0;
      }

      var table = new Table();
      table.AddColumn("ID");
      table.AddColumn("Name");
      table.AddColumn("State");

      foreach (var light in allLights)
      {
        table.AddRow(light.Id, light.Name, light.IsOn ? "On" : "Off");
      }
      _console.Write(table);

      return 0;
    }
  }
}
