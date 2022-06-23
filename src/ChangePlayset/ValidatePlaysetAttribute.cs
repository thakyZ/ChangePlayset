using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.Globalization;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace NekoBoiNick.change_playset {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  internal class ValidatePlaysetAttribute : ValidateEnumeratedArgumentsAttribute {
    private readonly string[] _validValues;

    /// <summary>
    /// Gets or sets the custom error message that is displayed to the user.
    /// The item being validated and a text representation of the validation set is passed as the
    /// first and second formatting argument to the <see cref="ErrorMessage"/> formatting pattern.
    /// <example>
    /// <code>
    /// [ValidateSet("A","B","C", ErrorMessage="The item '{0}' is not part of the set '{1}'.")
    /// </code>
    /// </example>
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets a flag specifying if we should ignore the case when performing string comparison.
    /// The default is true.
    /// </summary>
    public bool IgnoreCase { get; set; } = true;

    /// <summary>
    /// Gets the valid values in the set.
    /// </summary>
    public IList<string> ValidValues {
      get {
        return _validValues;
      }
    }

    /// <summary>
    /// Validates that each parameter argument is present in the specified set.
    /// </summary>
    /// <param name="element">Object to validate.</param>
    /// <exception cref="ValidationMetadataException">
    /// if element is not in the set
    /// for invalid argument
    /// </exception>
    protected override void ValidateElement(object element) {
      if (element == null) {
        throw new ValidationMetadataException("The argument is null. Provide a valid value for the argument, and then try running the command again.");
      }

      string objString = element.ToString();
      foreach (string setString in ValidValues) {
        if (CultureInfo.InvariantCulture.CompareInfo.Compare(
            setString,
            objString,
            IgnoreCase ? CompareOptions.IgnoreCase : CompareOptions.None) == 0) {
          return;
        }
      }

      var errorMessageFormat = string.IsNullOrEmpty(ErrorMessage) ? $"The argument \"{0}\" does not belong to the set \"{1}\" specified by the ValidateSet attribute. Supply an argument that is in the set and then try the command again." : ErrorMessage;
      throw new ValidationMetadataException(string.Format(errorMessageFormat, element.ToString(), SetAsString()));
    }

    private string SetAsString() => string.Join(CultureInfo.CurrentUICulture.TextInfo.ListSeparator, ValidValues);

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateSetAttribute"/> class.
    /// </summary>
    /// <param name="validValues">List of valid values.</param>
    /// <exception cref="ArgumentNullException">For null arguments.</exception>
    /// <exception cref="ArgumentOutOfRangeException">For invalid arguments.</exception>
    public ValidatePlaysetAttribute(SetPlayset playset) {
      var game = playset.Game;
      if (game is null || game == string.Empty) {
        throw new ArgumentNullException(nameof(SetPlayset.Game), "Game specification needs to come before playset name");
      }
      if (SetPlayset.config.GameConfigs.Count == 0) {
        SetPlayset.LoadConfig();
      }
      if (SetPlayset.config.GameConfigs.Count == 0) {
        throw new ArgumentOutOfRangeException(nameof(SetPlayset.config.GameConfigs), "Game configs cotain nothing in the config.");
      }
      var loaded = LoadSavedPlaysets(game, SetPlayset.config.GameConfigs);
      if (loaded[0] == string.Empty) {
        throw new ArgumentOutOfRangeException(nameof(SetPlayset.config.GameConfigs), "No saved playsets.");
      }
      _validValues = loaded;
    }

    internal static string[] LoadSavedPlaysets(string game, List<GameConfig> gameGonfigs) {
      var gameConfig = gameGonfigs.Where(g => g.GameName.ToLower() == game.ToLower()).First();
      return game.ToLower() switch {
        "barotrauma" => FindSavedPlaysets(Path.Join(gameConfig.GamePath, gameConfig.ConfigFileName)),
        _ => Array.Empty<string>(),
      };
    }

    internal static string[] FindSavedPlaysets(string path) {
      string[] list = Array.Empty<string>();
      string[] stringSlice = { Path.GetDirectoryName(path) ?? "", Path.GetFileNameWithoutExtension(path), Path.GetExtension(path) };
      Regex regex = new Regex(@$"^{Path.Join(stringSlice[0], stringSlice[1])}\.(.*)+\.{stringSlice[2]}$", RegexOptions.Compiled);
      foreach (string f in Directory.GetFiles(stringSlice[0])) {
        if (regex.Match(f).Success) {
          list = list.Append(f).ToArray();
        }
      }
      if (list.Length == 0) {
        list = list.Append(String.Empty).ToArray();
      }
      return list;
    }
  }
}