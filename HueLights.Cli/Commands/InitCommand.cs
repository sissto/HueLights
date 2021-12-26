using HueLights.Infrastructure.Models;
using HueLights.Infrastructure.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace HueLights.Cli.Commands
{
  public class InitCommand : AsyncCommand
  {
    private readonly IAnsiConsole _console;
    private readonly HueService _hueService;
    private readonly SettingsService _settingsService;

    public InitCommand(IAnsiConsole console, HueService hueService, SettingsService settingsService)
    {
      _console = console;
      _hueService = hueService;
      _settingsService = settingsService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
      var bridges = await _hueService.LocateBridges();
      if (!bridges.Any())
      {
        _console.Markup("[red]No bridges found![/]");
        return -1;
      }

      var selectedBridge = _console.Prompt(new SelectionPrompt<HueBridge>()
        .Title("Please select a bridge:")
        .UseConverter(x => x.IpAddress)
        .AddChoices(bridges));

      if (selectedBridge == null)
      {
        _console.MarkupLine("[red]No bridge selected![/]");
        return -1;
      }
      
      _console.MarkupLine($"[yellow]The following bridge was selected: {selectedBridge.IpAddress}[/]");

      _console.WriteLine("Please press the button on the bridge.");
      if (_console.Confirm("Button pressed?"))
      {
        try
        {
          var appKey = await _hueService.Register(selectedBridge.IpAddress, Constants.APPNAME, Environment.MachineName);
          if (!string.IsNullOrEmpty(appKey))
          {
            var newSettings = new Settings { BridgeIpAddress = selectedBridge.IpAddress, BridgeAppKey = appKey };
            _settingsService.Save(newSettings);
        
            _console.MarkupLine($"[green]Successfully initialized![/]");
          }
        }
        catch (Exception)
        {
          _console.MarkupLine("[red]An error occured. You possible didn't press the button on the bridge.[/]");
        }
      }

      return 0;
    }
  }
}
