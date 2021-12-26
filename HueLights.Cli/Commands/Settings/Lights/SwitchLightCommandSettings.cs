using Spectre.Console.Cli;

namespace HueLights.Cli.Commands.Settings.Lights
{
  public class SwitchLightCommandSettings : CommandSettings
  {
    [CommandArgument(0, "[id]")]
    public string Id { get; set; }
  }
}
