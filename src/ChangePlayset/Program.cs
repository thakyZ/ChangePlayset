using System;
using System.Management.Automation;

using nucs.JsonSettings;

namespace NekoBoiNick.change_playset {
  [Cmdlet(VerbsCommon.Set, "Playset")]
  public class SetPlayset : Cmdlet, IDynamicParameters {
    [Parameter(Mandatory = true, Position = 0)]
    [ValidateGame()]
    public string Game {
      get { return _game; }
      set { _game = value; }
    }
    private string _game = "";

    [Parameter(Mandatory = true, Position = 1)]
    public string Playset {
      get { return _playset; }
      set { _playset = value; }
    }
    private string _playset = "";

    [Parameter(Mandatory = false, Position = 2)]
    public string Command {
      get { return _command; }
      set { _command = value; }
    }
    private string _command = "";

    internal static Config config = new();

    protected override void ProcessRecord() {
      LoadConfig();
      HandleCommandLine(_game, _playset, _command);
    }

    internal static void LoadConfig() {
      if (File.Exists(Path.Join(System.Reflection.Assembly.GetExecutingAssembly().Location, config.FileName))) {
        config = JsonSettings.Load<Config>("conifg.json");
      } else {
        config = JsonSettings.Construct<Config>("config.json");
      }
    }

    static bool HandleCommandLine(string game, string playset, string command = "") {
      var sGame = TranslateGameToSupportedGames(game);
      if (sGame == Supported.SupportedGames.none) {
        return false;
      }
      switch (sGame) {
        case Supported.SupportedGames.barotrauma:
          return HandleBarotrauma(playset, command);
          break;
        default:
          return false;
      }
    }

    internal static Supported.SupportedGames TranslateGameToSupportedGames(string game) {
      if (Enum.TryParse(game, out Supported.SupportedGames value)) {
        return value;
      } else {
        return Supported.SupportedGames.none;
      }
    }

    public object GetDynamicParameters() {
      if (!string.IsNullOrEmpty(_playset)) {

      }
    }
  }
}
