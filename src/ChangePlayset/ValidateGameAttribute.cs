using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.Globalization;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace NekoBoiNick.change_playset {
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ValidateGameAttribute : ValidateEnumeratedArgumentsAttribute {
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
    public IList<string> ValidValues { get { return _validValues; } }

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
    /// Initializes a new instance of the <see cref="ValidateGameAttribute"/> class.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">For invalid arguments.</exception>
    public ValidateGameAttribute() {
      SetPlayset.LoadConfig();
      if (SetPlayset.config.GameConfigs.Count == 0) {
        throw new ArgumentOutOfRangeException(nameof(SetPlayset.config.GameConfigs), "Game configs cotain nothing in the config.");
      }
      string[] validValues = Array.Empty<string>();
      SetPlayset.config.GameConfigs.ForEach((g) => validValues = validValues.Append(g.GameName).ToArray());
      _validValues = validValues;
    }
  }
}