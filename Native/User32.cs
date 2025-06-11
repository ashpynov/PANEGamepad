using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PANEGamepad.Native
{
    public static partial class User32
    {
        [DllImport("user32.dll")]
        private static extern System.IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(System.IntPtr hwnd, out RECT lpRect); // Gets window bounds in screen coordinates

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 SendInput(UInt32 numberOfInputs, INPUT[] inputs, Int32 sizeOfInputStructure);

        [DllImport("user32.dll")]
        public static extern UInt32 MapVirtualKey(UInt32 uCode, UInt32 uMapType);

        private static int? _currentProcessId;
        public static bool IsWindowFocused()
        {
            _currentProcessId ??= System.Diagnostics.Process.GetCurrentProcess().Id;

            IntPtr activeWindowHandle = GetForegroundWindow();
            if (activeWindowHandle == IntPtr.Zero)
            {
                return false; // No window is focused
            }
            GetWindowThreadProcessId(activeWindowHandle, out int activeProcessId);
            return activeProcessId == _currentProcessId;
        }

        public static void SendMouseKey(KeyCode key, bool down)
        {
            INPUT input = new()
            {
                Type = InputType.Mouse,
                Data =
                {
                    Mouse = new MOUSEINPUT()
                    {
                        Flags = KeyCodeToMouseEventFlag(key, down)
                    }
                }
            };

            _ = SendInput(1, [input], Marshal.SizeOf<INPUT>());

            // mouse_event(KeyCodeToMouseEventFlag(key, down), 0, 0, 0, 0);
        }

        public static void SendKey(KeyCode key, bool down)
        {
            if (key is KeyCode.Mouse0 or KeyCode.Mouse1 or KeyCode.Mouse2)
            {
                SendMouseKey(key, down);
                return;
            }

            if (key is KeyCode.Mouse5)
            {
                SendMouseWheel(-60);
                return;
            }

            if (key is KeyCode.Mouse6)
            {
                SendMouseWheel(60);
                return;
            }

            uint keyCode = VirtualKeyCodes.FromKeyCode(key);
            INPUT input = new()
            {
                Type = InputType.Keyboard,
                Data =
                {
                    Keyboard =
                        new KEYBDINPUT
                            {
                                KeyCode = (ushort) keyCode,
                                Scan = (ushort)(MapVirtualKey(keyCode, 0) & 0xFFU),
                                Flags = (VirtualKeyCodes.IsExtendedKey(keyCode) ? (UInt32) KeyboardFlag.ExtendedKey : 0)
                                        + (down ? 0 : KeyboardFlag.KeyUp),
                                Time = 0,
                                ExtraInfo = IntPtr.Zero
                            }
                }
            };

            _ = User32.SendInput(1, [input], Marshal.SizeOf<INPUT>());

            // mouse_event(KeyCodeToMouseEventFlag(key, down), 0, 0, 0, 0);
        }

        public static void MoveMouseBy(int dx, int dy)
        {
            INPUT input = new()
            {
                Type = InputType.Mouse,
                Data =
                {
                    Mouse = new MOUSEINPUT()
                    {
                        Flags = MOUSEEVENTF_MOVE,
                        X = dx,
                        Y = dy
                    }
                }
            };

            _ = User32.SendInput(1, [input], Marshal.SizeOf<INPUT>());

            // mouse_event(MOUSEEVENTF_MOVE, dx, dy, 0, 0);
        }
        public static void SendMouseWheel(int dx)
        {
            INPUT input = new()
            {
                Type = InputType.Mouse,
                Data =
                {
                    Mouse = new MOUSEINPUT()
                    {
                        Flags = MOUSEEVENTF_WHEEL,
                        MouseData = (uint)dx
                    }
                }
            };
            _ = User32.SendInput(1, [input], Marshal.SizeOf<INPUT>());
        }

        public static Rect GetActiveWindowRect()
        {
            System.IntPtr hwnd = User32.GetActiveWindow(); // Handle to the Unity game window
            User32.GetWindowRect(hwnd, out RECT windowRect);
            return new Rect(
                windowRect.Left,
                windowRect.Top,
                windowRect.Right - windowRect.Left,
                windowRect.Bottom - windowRect.Top);
        }

        public static bool SetCursorPos(Vector2Int pos)
        {
            return User32.SetCursorPos(pos.x, pos.y);
        }
        public static Vector2 GetCursorPos()
        {
            User32.GetCursorPos(out POINT point);
            return new Vector2(point.X, point.Y);
        }

    }
}