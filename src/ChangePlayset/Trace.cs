using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace NekoBoiNick.change_playset {
  internal class Trace {
    internal static PSArgumentOutOfRangeException NewArgumentOutOfRangeException(string paramName, object actualValue) {
      if (string.IsNullOrEmpty(paramName)) {
        throw new ArgumentNullException(nameof(paramName));
      }

      string message = StringUtil.Format($"Cannot process argument because the value of argument \"{0}\" is null. Change the value of argument \"{0}\" to a non-null value.", paramName);
      var e = new PSArgumentOutOfRangeException(paramName, actualValue, message);

      return e;
    }
  }
}
