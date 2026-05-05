using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Rolan.Helpers
{
    public static class HotkeyHelper
    {
        private const int WM_HOTKEY = 0x0312;
        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;

        private static IntPtr _hWnd;
        private static bool _isRegistered = false;
        private static int _currentHotkeyId = 1;

        public static event EventHandler? HotkeyPressed;

        public static bool RegisterHotkey(Window window, string hotkey)
        {
            UnregisterHotkey();

            _hWnd = new WindowInteropHelper(window).Handle;
            var source = HwndSource.FromHwnd(_hWnd);
            source?.AddHook(HwndHook);

            var parts = hotkey.Split('+');
            int modifiers = 0;
            int key = 0;

            foreach (var part in parts)
            {
                string p = part.Trim().ToLower();
                switch (p)
                {
                    case "alt": modifiers |= MOD_ALT; break;
                    case "ctrl":
                    case "control": modifiers |= MOD_CONTROL; break;
                    case "shift": modifiers |= MOD_SHIFT; break;
                    case "win": modifiers |= MOD_WIN; break;
                    default:
                        key = GetVirtualKeyCode(p);
                        break;
                }
            }

            if (key == 0)
                return false;

            _isRegistered = RegisterHotKey(_hWnd, _currentHotkeyId, modifiers, key);
            return _isRegistered;
        }

        public static void UnregisterHotkey()
        {
            if (_isRegistered && _hWnd != IntPtr.Zero)
            {
                UnregisterHotKey(_hWnd, _currentHotkeyId);
                _isRegistered = false;
            }
        }

        private static IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == _currentHotkeyId)
            {
                HotkeyPressed?.Invoke(null, EventArgs.Empty);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private static int GetVirtualKeyCode(string key)
        {
            return key.Length == 1 
                ? char.ToUpper(key[0]) 
                : key switch
                {
                    "backspace" => 0x08,
                    "tab" => 0x09,
                    "enter" => 0x0D,
                    "escape" => 0x1B,
                    "space" => 0x20,
                    "pgup" => 0x21,
                    "pgdn" => 0x22,
                    "end" => 0x23,
                    "home" => 0x24,
                    "left" => 0x25,
                    "up" => 0x26,
                    "right" => 0x27,
                    "down" => 0x28,
                    "insert" => 0x2D,
                    "delete" => 0x2E,
                    "0" => 0x30,
                    "1" => 0x31,
                    "2" => 0x32,
                    "3" => 0x33,
                    "4" => 0x34,
                    "5" => 0x35,
                    "6" => 0x36,
                    "7" => 0x37,
                    "8" => 0x38,
                    "9" => 0x39,
                    "a" => 0x41,
                    "b" => 0x42,
                    "c" => 0x43,
                    "d" => 0x44,
                    "e" => 0x45,
                    "f" => 0x46,
                    "g" => 0x47,
                    "h" => 0x48,
                    "i" => 0x49,
                    "j" => 0x4A,
                    "k" => 0x4B,
                    "l" => 0x4C,
                    "m" => 0x4D,
                    "n" => 0x4E,
                    "o" => 0x4F,
                    "p" => 0x50,
                    "q" => 0x51,
                    "r" => 0x52,
                    "s" => 0x53,
                    "t" => 0x54,
                    "u" => 0x55,
                    "v" => 0x56,
                    "w" => 0x57,
                    "x" => 0x58,
                    "y" => 0x59,
                    "z" => 0x5A,
                    "f1" => 0x70,
                    "f2" => 0x71,
                    "f3" => 0x72,
                    "f4" => 0x73,
                    "f5" => 0x74,
                    "f6" => 0x75,
                    "f7" => 0x76,
                    "f8" => 0x77,
                    "f9" => 0x78,
                    "f10" => 0x79,
                    "f11" => 0x7A,
                    "f12" => 0x7B,
                    "`" => 0xC0,
                    _ => 0
                };
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
