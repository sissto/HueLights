using HueLights.Infrastructure.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace HueLights.Cli.Commands.Bridges
{
  public class LocateBridgesCommand : AsyncCommand
  {
    private readonly IAnsiConsole _console;
    private readonly HueService _service;

    public LocateBridgesCommand(IAnsiConsole console, HueService service)
    {
      _console = console;
      _service = service;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
      var result = await _console.Status()
        .Spinner(Spinner.Known.Dots)
        .StartAsync("Locating bridges...", ctx => _service.LocateBridges());

      if (!result.Any())
      {
        _console.Markup("[red]No bridges found![/]");
        return 0;
      }

      var table = new Table();
      table.AddColumn("ID");
      table.AddColumn("Ip address");

      foreach (var bridge in result)
      {
        table.AddRow(bridge.BridgeId, bridge.IpAddress);
      }
      _console.Write(table);

      return 0;
    }
  }
}
