using HueLights.Infrastructure.Models;
using Q42.HueApi;

namespace HueLights.Infrastructure.Services
{
  public class HueService
  {
    private readonly SettingsService _settingsService;

    public HueService(SettingsService settingsService)
    {
      _settingsService = settingsService;
    }

    public async Task<IEnumerable<HueBridge>> LocateBridges()
    {
      var result = new List<HueBridge>();
      var locator = new HttpBridgeLocator();
      var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
      if (bridges.Any())
        result.AddRange(bridges.Select(bridge => new HueBridge { BridgeId = bridge.BridgeId, IpAddress = bridge.IpAddress }));
      return result;
    }

    public async Task<string?> Register(string bridgeIpAddress, string appName, string deviceName)
    {
      if (string.IsNullOrWhiteSpace(bridgeIpAddress))
        return null;

      var client = new LocalHueClient(bridgeIpAddress);
      var appKey = await client.RegisterAsync(appName, deviceName);
      return appKey;
    }

    public async Task<IEnumerable<HueLight>> GetAllLights()
    {
      var result = new List<HueLight>();
      var client = GetHueClient();
      if (client == null)
        return result;

      var allLights = await client.GetLightsAsync();
      if (allLights.Any())
      {
        result.AddRange(allLights.Select(x => new HueLight 
        { 
          Id = x.Id,
          Name = x.Name,
          IsOn = x.State.On
        }));
      }

      return result;
    }

    private LocalHueClient? GetHueClient()
    {
      var appSettings = _settingsService.Load();
      if (appSettings == null || string.IsNullOrWhiteSpace(appSettings.BridgeIpAddress) || string.IsNullOrWhiteSpace(appSettings.BridgeAppKey))
        return default;
      
      var client = new LocalHueClient(appSettings.BridgeIpAddress);
      client.Initialize(appSettings.BridgeAppKey);

      return client;
    }
  }
}
