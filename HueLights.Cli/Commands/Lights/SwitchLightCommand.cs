using HueLights.Cli.Commands.Settings.Lights;
using HueLights.Infrastructure.Services;
using Spectre.Console.Cli;

namespace HueLights.Cli.Commands.Lights
{
  public class SwitchLightCommand : AsyncCommand<SwitchLightCommandSettings>
  {
    private readonly HueService _hueService;

    public SwitchLightCommand(HueService hueService)
    {
      _hueService = hueService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, SwitchLightCommandSettings settings)
    {
      if (settings == null || string.IsNullOrWhiteSpace(settings.Id))
        return -1;

      var light = await _hueService.GetLightById(settings.Id);
      if (light == null)
        return -1;

      bool result = await _hueService.SwitchLight(!light.IsOn, new List<string> { settings.Id });
      return result ? 0 : -1;
    }
  }
}
