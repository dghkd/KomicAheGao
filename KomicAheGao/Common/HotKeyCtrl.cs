using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;

namespace KomicAheGao
{
    public class HotKeyCtrl
    {
        #region Private Member
        private static readonly Lazy<HotKeyCtrl> _lazyInstance = new Lazy<HotKeyCtrl>(() => new HotKeyCtrl(), true);

        private Win32Helper.KeyModifier _modifier;
        private Key _vKey;
        private IntPtr _hWnd;

        private const String REG_Key_Root = "Software\\KomicAheGao";
        private const String REG_Value_Modifier = "Modifier";
        private const String REG_Value_VKey = "VKey";
        #endregion

        #region Property
        /// <summary>
        /// Single instance.
        /// </summary>
        public static HotKeyCtrl Data { get { return _lazyInstance.Value; } private set { } }

        public Key VKey 
        {
            get
            {
                return _vKey;
            }
        }

        public Win32Helper.KeyModifier Modifier
        {
            get
            {
                return _modifier;
            }
        }
        #endregion

        #region Public Method

        /// <summary>
        /// Set the handle of window.
        /// </summary>
        /// <param name="hWnd">Handle of window</param>
        public void Init(IntPtr hWnd)
        {
            _hWnd = hWnd;
        }

        /// <summary>
        /// Register the hotkey combination.
        /// </summary>
        /// <param name="keyModifier"></param>
        /// <param name="key"></param>
        public bool RegisterHotKey(Win32Helper.KeyModifier keyModifier, Key key)
        {
            Win32Helper.UnregisterHotKey(_hWnd, 1);
            bool ret = Win32Helper.RegisterHotKey(_hWnd, 1, (uint)keyModifier, (uint)KeyInterop.VirtualKeyFromKey(key));
            if (ret)
            {
                _modifier = keyModifier;
                _vKey = key;
                SaveHotKey();
            }

            return ret;
        }

        /// <summary>
        /// Unregister the hotkey combination.
        /// </summary>
        public bool UnRegisterHotKey()
        {
            bool ret = Win32Helper.UnregisterHotKey(_hWnd, 1);
            return ret;
        }

        /// <summary>
        /// Load the hotkey combination from registry key. And enable the hotkey.
        /// </summary>
        public void LoadHotKey()
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(REG_Key_Root);

            _modifier = (Win32Helper.KeyModifier)reg.GetValue(REG_Value_Modifier, Win32Helper.KeyModifier.Alt);
            _vKey = (Key)reg.GetValue(REG_Value_VKey, Key.Oem3);

            reg.Close();

            bool ret = RegisterHotKey(_modifier, _vKey);
        }

        /// <summary>
        /// Save the hotkey combination into registry.
        /// </summary>
        public void SaveHotKey()
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(REG_Key_Root);

            reg.SetValue(REG_Value_Modifier, (int)_modifier, RegistryValueKind.DWord);
            reg.SetValue(REG_Value_VKey, (int)_vKey, RegistryValueKind.DWord);

            reg.Close();
        }
        #endregion
    }
}
