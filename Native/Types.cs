using System;
using System.Runtime.InteropServices;

namespace PANEGamepad.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;   // X position of top-left corner
        public int Top;    // Y position of top-left corner
        public int Right;  // X position of bottom-right corner
        public int Bottom; // Y position of bottom-right corner
    }

    internal enum InputType : UInt32
    {
        Mouse = 0,
        Keyboard = 1,
        Hardware = 2,
    }

    internal struct INPUT
    {
        public InputType Type;

        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    [Flags]
    internal enum KeyboardFlag : UInt32
    {
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        ScanCode = 0x0008,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct KEYBDINPUT
    {
        public UInt16 KeyCode;
        public UInt16 Scan;
        public KeyboardFlag Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }

    internal struct MOUSEINPUT
    {
        public Int32 X;
        public Int32 Y;
        public UInt32 MouseData;
        public UInt32 Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }

    internal struct HARDWAREINPUT
    {
        public UInt32 Msg;
        public UInt16 ParamL;
        public UInt16 ParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
    }

}