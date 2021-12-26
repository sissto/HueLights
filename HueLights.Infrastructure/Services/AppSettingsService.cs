using HueLights.Infrastructure.Models;
using Newtonsoft.Json;

namespace HueLights.Infrastructure.Services
{
  public class AppSettingsService
  {
    private readonly string _appName;

    public AppSettingsService(string appName)
    {
      _appName = appName;
    }

    public string FolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appName);

    public string FilePath => Path.Combine(FolderPath, "settings.json");

    public AppSettings? Load()
    {
      try
      {
        if (!File.Exists(FilePath))
          return default;

        string fileContent = File.ReadAllText(FilePath);
        return JsonConvert.DeserializeObject<AppSettings>(fileContent);
      }
      catch (Exception)
      {
      }
      return default;
    }

    public void Save(AppSettings settings)
    {
      if (settings == null)
        return;

      try
      {
        if (!Directory.Exists(FolderPath))
          Directory.CreateDirectory(FolderPath);

        string fileContent = JsonConvert.SerializeObject(settings);
        File.WriteAllText(FilePath, fileContent);
      }
      catch (Exception)
      {
      }
    }
  }
}
