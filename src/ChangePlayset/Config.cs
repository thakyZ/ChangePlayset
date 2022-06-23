using nucs.JsonSettings;

namespace NekoBoiNick.change_playset {
  public class Config : JsonSettings {
    public override string FileName { get; set; } = "config.json";

    #region Settings
    public List<GameConfig> GameConfigs { get; set; } = new List<GameConfig>();
    public string Version { get; set; } = "1";
    #endregion Settings

    public Config() { }

    public Config(string fileName) : base(fileName) { }
  }

  public class GameConfig {
    public enum ConfigTypes {
      XML
    }

    public ConfigTypes ConfigType { get; set; }
    public string GameName { get; set; }
    public string GamePath { get; set; }
    public string ConfigFileName { get; set; }
  }
}