using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using MiMFa.Service;
using MiMFa.General;
using MiMFa.Exclusive.DateAndTime;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using MiMFa.Exclusive.ProgramingTechnology.Tools;

namespace MiMFa.Exclusive.ProgramingTechnology.CommandLanguage
{
    public class MCLTools : ProviderTools
    {
        #region MRL Dictionary 
        public static string StartSignFirstCommandTag { get; } = "#<{";
        public static string StartSignCommandTag { get; } = "#{";
        public static string StartSignLastCommandTag { get; } = "#>{";
        public static string EndSignCommandTag { get; } = "}#";
        public static string StartSignIndex { get; } = "[";
        public static string EndSignIndex { get; } = "]";
        public static string StartSignAllSwitchValue { get; } = StartSignStrong;
        public static string EndSignAllSwitchValue { get; } = EndSignStrong;
        public static string SignCharacterSwitch { get; } = "-";
        public static string SignCharacterSSwitch { get; } = StartSignStrong + SignCharacterSwitch ;
        public static string SignNot { get; } = "!";
        public static string SignTransfer { get; } = ":";
        public static string SignMultiSlice { get; } = ".";
        public static string SignFinish { get; } = ";";
        public static string SignPointer { get; } = "->";
        #endregion

        #region Tools
        #endregion
    }
}
