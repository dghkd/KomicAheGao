using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace KomicAheGao.Common
{
    public class RegMethod
    {
        /// <summary>
        /// REG_Key_Root = @"SOFTWARE\KomicAheGao"
        /// </summary>
        public const String REG_Key_Root = @"SOFTWARE\KomicAheGao";
        /// <summary>
        /// REG_Value_Open_Manage = "OpenManage"
        /// </summary>
        public const String REG_Value_Open_Manage = "OpenManage";

        /// <summary>
        /// Set a message at registry.
        /// </summary>
        /// <param name="valueName">The string of message name.</param>
        /// <param name="value">The RegistryValueKind.DWord of message value.</param>
        public static void SetMessage(String valueName,object value)
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(REG_Key_Root);
            reg.SetValue(valueName, value, RegistryValueKind.DWord);
            reg.Close();
        }

        /// <summary>
        /// Get a message value and delete it.
        /// </summary>
        /// <param name="valueName">The string of message name.</param>
        /// <returns>The RegistryValueKind.DWord of message value.</returns>
        public static object GetMessage(String valueName)
        {
            object ret = null;
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(REG_Key_Root);
            ret = reg.GetValue(valueName);
            if (ret!=null)
            {
                reg.DeleteValue(valueName, false);
            }
            reg.Close();
            return ret;
        }

        
    }
}
