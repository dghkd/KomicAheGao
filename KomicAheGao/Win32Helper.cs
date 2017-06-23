using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KomicAheGao
{
    public class Win32Helper
    {
        #region Window field offsets for GetWindowLong
        public const int GWL_WNDPROC = (-4);
        public const int GWL_HINSTANCE = (-6);
        public const int GWL_HWNDPARENT = (-8);
        public const int GWL_STYLE = (-16);
        public const int GWL_EXSTYLE = (-20);
        public const int GWL_USERDATA = (-21);
        public const int GWL_ID = (-12);
        #endregion

        #region Window command offsets for GetWindow
        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;
        public const int GW_CHILD = 5;
        public const int GW_ENABLEDPOPUP = 6;
        #endregion

        #region Extended Window Styles
        public const long WS_EX_DLGMODALFRAME = 0x00000001L;
        public const long WS_EX_NOPARENTNOTIFY = 0x00000004L;
        public const long WS_EX_TOPMOST = 0x00000008L;
        public const long WS_EX_ACCEPTFILES = 0x00000010L;
        public const long WS_EX_TRANSPARENT = 0x00000020L;

        public const long WS_EX_MDICHILD = 0x00000040L;
        public const long WS_EX_TOOLWINDOW = 0x00000080L;
        public const long WS_EX_WINDOWEDGE = 0x00000100L;
        public const long WS_EX_CLIENTEDGE = 0x00000200L;
        public const long WS_EX_CONTEXTHELP = 0x00000400L;

        public const long WS_EX_RIGHT = 0x00001000L;
        public const long WS_EX_LEFT = 0x00000000L;
        public const long WS_EX_RTLREADING = 0x00002000L;
        public const long WS_EX_LTRREADING = 0x00000000L;
        public const long WS_EX_LEFTSCROLLBAR = 0x00004000L;
        public const long WS_EX_RIGHTSCROLLBAR = 0x00000000L;

        public const long WS_EX_CONTROLPARENT = 0x00010000L;
        public const long WS_EX_STATICEDGE = 0x00020000L;
        public const long WS_EX_APPWINDOW = 0x40000L;

        public const long WS_EX_NOACTIVATE = 0x08000000L;

        public const long WS_EX_AUTHOR = 0xB9D82L;
        #endregion

        #region Window Messages
        public const int WM_NULL = 0x0000;
        public const int WM_CREATE = 0x0001;
        public const int WM_DESTROY = 0x0002;
        public const int WM_MOVE = 0x0003;
        public const int WM_SIZE = 0x0005;
        public const int WM_ACTIVATE = 0x0006;

        public const uint WM_SHOWWINDOW = 0x0018;

        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int WM_EXITSIZEMOVE = 0x0232;
        public const int WM_HOTKEY = 0x0312;
        #endregion
        
        #region ShowWindow Commands
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_NORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_MAX = 11;

        /* Identifiers for the WM_SHOWWINDOW message */
        public const int SW_PARENTCLOSING = 1;
        public const int SW_OTHERZOOM = 2;
        public const int SW_PARENTOPENING = 3;
        public const int SW_OTHERUNZOOM = 4;

        #endregion

        #region Win32 API
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern uint GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetWindowText(IntPtr hWnd, StringBuilder lpString, uint nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowsProc ewp, int lParam);


        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        
        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        #endregion

        #region Custom Enum
        public enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }
        #endregion

        #region Public Method

        /// <summary>
        /// Get the window handle of process.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        /// <param name="title">The window title.</param>
        /// <returns>Return the window handle.</returns>
        public static IntPtr GetWindowHandle(int pid, String title)
        {
            IntPtr result = IntPtr.Zero;

            // define the EnumWindowsProc anonymous methods.
            EnumWindowsProc enumerateHandle = delegate(IntPtr hWnd, int lParam)
            {
                int id;
                GetWindowThreadProcessId(hWnd, out id);

                if (pid == id)
                {
                    StringBuilder clsName = new StringBuilder(256);
                    bool hasClass = GetClassName(hWnd, clsName, 256);
                    if (hasClass)
                    {
                        int maxLength = (int)GetWindowTextLength(hWnd);
                        StringBuilder builder = new StringBuilder(maxLength + 1);
                        GetWindowText(hWnd, builder, (uint)builder.Capacity);

                        String text = builder.ToString();
                        String className = clsName.ToString();
                        bool isAppWindow = (GetWindowLong(hWnd, GWL_EXSTYLE) & WS_EX_APPWINDOW) != 0;

                        // There could be multiple handle associated with our pid, 
                        // so we return the first handle that satisfy:
                        // 1) the handle title/ caption matches our window title,
                        // 2) the window class name starts with HwndWrapper (WPF specific)
                        // 3) the window has WS_EX_APPWINDOW style
                        if (title == text && className.StartsWith("HwndWrapper") && isAppWindow)
                        {
                            result = hWnd;
                            return false;
                        }
                    }
                }
                return true;
            };

            EnumDesktopWindows(IntPtr.Zero, enumerateHandle, 0);

            return result;
        }

        /// <summary>
        /// Show the window active and set to foreground.
        /// </summary>
        /// <param name="procName">The process name.</param>
        /// <param name="wndTitle">The window title.</param>
        /// <param name="lsExcludeProc">The exclude process list.</param>
        public static void ShowProcessWindowActive(String procName, String wndTitle, List<Process> lsExcludeProc)
        {
            //Get the running process by  process name.
            Process[] prcs = Process.GetProcessesByName(procName);
            List<Process> lsProcess = new List<Process>();

            //Filter exclude process.
            foreach (Process prc in prcs)
            {
                if (!lsExcludeProc.Exists(x => x.Id == prc.Id))
                {
                    lsProcess.Add(prc);
                }
            }

            //Show active the window.
            foreach (Process runningProcess in lsProcess)
            {
                var handle = GetWindowHandle(runningProcess.Id, wndTitle);
                if (handle != IntPtr.Zero)
                {
                    // show window
                    ShowWindow(handle, SW_SHOW);
                    // send WM_SHOWWINDOW message to toggle the visibility flag
                    SendMessage(handle, WM_SHOWWINDOW, IntPtr.Zero, new IntPtr(SW_PARENTOPENING));
                    // set window to foreground.
                    SetForegroundWindow(handle);
                }
            }
        }

        #endregion
    }
}
