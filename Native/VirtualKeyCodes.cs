using UnityEngine;

namespace PANEGamepad.Native
{
    public static class VirtualKeyCodes
    {
        // Mouse buttons
        public const uint VK_LBUTTON = 0x01;
        public const uint VK_RBUTTON = 0x02;
        public const uint VK_CANCEL = 0x03;
        public const uint VK_MBUTTON = 0x04;

        // Control keys
        public const uint VK_BACK = 0x08;
        public const uint VK_TAB = 0x09;
        public const uint VK_RETURN = 0x0D;
        public const uint VK_SHIFT = 0x10;
        public const uint VK_CONTROL = 0x11;
        public const uint VK_MENU = 0x12; // ALT
        public const uint VK_PAUSE = 0x13;
        public const uint VK_CAPITAL = 0x14; // CAPS LOCK
        public const uint VK_ESCAPE = 0x1B;
        public const uint VK_SPACE = 0x20;
        public const uint VK_PRIOR = 0x21; // PAGE UP
        public const uint VK_NEXT = 0x22;  // PAGE DOWN
        public const uint VK_END = 0x23;
        public const uint VK_HOME = 0x24;
        public const uint VK_LEFT = 0x25;
        public const uint VK_UP = 0x26;
        public const uint VK_RIGHT = 0x27;
        public const uint VK_DOWN = 0x28;
        public const uint VK_PRINT = 0x2A;

        public const uint VK_SNAPSHOT = 0x2C;
        public const uint VK_INSERT = 0x2D;
        public const uint VK_DELETE = 0x2E;

        // Numbers 0-9
        public const uint VK_0 = 0x30;
        public const uint VK_1 = 0x31;
        public const uint VK_2 = 0x32;
        public const uint VK_3 = 0x33;
        public const uint VK_4 = 0x34;
        public const uint VK_5 = 0x35;
        public const uint VK_6 = 0x36;
        public const uint VK_7 = 0x37;
        public const uint VK_8 = 0x38;
        public const uint VK_9 = 0x39;

        // Letters A-Z
        public const uint VK_A = 0x41;
        public const uint VK_B = 0x42;
        public const uint VK_C = 0x43;
        public const uint VK_D = 0x44;
        public const uint VK_E = 0x45;
        public const uint VK_F = 0x46;
        public const uint VK_G = 0x47;
        public const uint VK_H = 0x48;
        public const uint VK_I = 0x49;
        public const uint VK_J = 0x4A;
        public const uint VK_K = 0x4B;
        public const uint VK_L = 0x4C;
        public const uint VK_M = 0x4D;
        public const uint VK_N = 0x4E;
        public const uint VK_O = 0x4F;
        public const uint VK_P = 0x50;
        public const uint VK_Q = 0x51;
        public const uint VK_R = 0x52;
        public const uint VK_S = 0x53;
        public const uint VK_T = 0x54;
        public const uint VK_U = 0x55;
        public const uint VK_V = 0x56;
        public const uint VK_W = 0x57;
        public const uint VK_X = 0x58;
        public const uint VK_Y = 0x59;
        public const uint VK_Z = 0x5A;

        // Windows keys
        public const uint VK_LWIN = 0x5B;
        public const uint VK_RWIN = 0x5C;
        public const uint VK_APPS = 0x5D; // Menu key

        // Numpad keys
        public const uint VK_NUMPAD0 = 0x60;
        public const uint VK_NUMPAD1 = 0x61;
        public const uint VK_NUMPAD2 = 0x62;
        public const uint VK_NUMPAD3 = 0x63;
        public const uint VK_NUMPAD4 = 0x64;
        public const uint VK_NUMPAD5 = 0x65;
        public const uint VK_NUMPAD6 = 0x66;
        public const uint VK_NUMPAD7 = 0x67;
        public const uint VK_NUMPAD8 = 0x68;
        public const uint VK_NUMPAD9 = 0x69;
        public const uint VK_MULTIPLY = 0x6A;
        public const uint VK_ADD = 0x6B;
        public const uint VK_SUBTRACT = 0x6D;
        public const uint VK_DECIMAL = 0x6E;
        public const uint VK_DIVIDE = 0x6F;

        // Function keys
        public const uint VK_F1 = 0x70;
        public const uint VK_F2 = 0x71;
        public const uint VK_F3 = 0x72;
        public const uint VK_F4 = 0x73;
        public const uint VK_F5 = 0x74;
        public const uint VK_F6 = 0x75;
        public const uint VK_F7 = 0x76;
        public const uint VK_F8 = 0x77;
        public const uint VK_F9 = 0x78;
        public const uint VK_F10 = 0x79;
        public const uint VK_F11 = 0x7A;
        public const uint VK_F12 = 0x7B;
        public const uint VK_F13 = 0x7C;
        public const uint VK_F14 = 0x7D;
        public const uint VK_F15 = 0x7E;

        // Special characters
        public const uint VK_NUMLOCK = 0x90;
        public const uint VK_SCROLL = 0x91;
        public const uint VK_LSHIFT = 0xA0;
        public const uint VK_RSHIFT = 0xA1;
        public const uint VK_LCONTROL = 0xA2;
        public const uint VK_RCONTROL = 0xA3;
        public const uint VK_LMENU = 0xA4;
        public const uint VK_RMENU = 0xA5;

        public const uint VK_OEM_1 = 0xBA; // ;:
        public const uint VK_OEM_PLUS = 0xBB; // =+
        public const uint VK_OEM_COMMA = 0xBC; // ,<
        public const uint VK_OEM_MINUS = 0xBD; // -_
        public const uint VK_OEM_PERIOD = 0xBE; // .>
        public const uint VK_OEM_2 = 0xBF; // /?
        public const uint VK_OEM_3 = 0xC0; // `~
        public const uint VK_OEM_4 = 0xDB; // [{
        public const uint VK_OEM_5 = 0xDC; // \|
        public const uint VK_OEM_6 = 0xDD; // ]}
        public const uint VK_OEM_7 = 0xDE; // '"


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>")]
        public static uint FromKeyCode(KeyCode keyCode)
        {
            // Handle alphabetical keys (A-Z)
            if (keyCode is >= KeyCode.A and <= KeyCode.Z)
            {
                return (uint)(keyCode - KeyCode.A + VK_A);
            }

            // Handle number keys (0-9)
            if (keyCode is >= KeyCode.Alpha0 and <= KeyCode.Alpha9)
            {
                return (uint)(keyCode - KeyCode.Alpha0 + VK_0);
            }

            // Handle numpad keys
            if (keyCode is >= KeyCode.Keypad0 and <= KeyCode.Keypad9)
            {
                return (uint)(keyCode - KeyCode.Keypad0 + VK_NUMPAD0);
            }

            // Handle function keys (F1-F15)
            if (keyCode is >= KeyCode.F1 and <= KeyCode.F15)
            {
                return (uint)(keyCode - KeyCode.F1 + VK_F1);
            }

            return keyCode switch
            {
                KeyCode.Backspace => VK_BACK,  // 0x08
                KeyCode.Tab => VK_TAB,  // 0x09
                KeyCode.Return => VK_RETURN,  // 0x0D
                KeyCode.Escape => VK_ESCAPE,  // 0x1B
                KeyCode.Space => VK_SPACE,  // 0x20
                KeyCode.LeftArrow => VK_LEFT,  // 0x25
                KeyCode.UpArrow => VK_UP,  // 0x26
                KeyCode.RightArrow => VK_RIGHT,  // 0x27
                KeyCode.DownArrow => VK_DOWN,  // 0x28
                KeyCode.LeftShift => VK_LSHIFT,  // 0xA0
                KeyCode.RightShift => VK_RSHIFT,  // 0xA1
                KeyCode.LeftControl => VK_LCONTROL,  // 0xA2
                KeyCode.RightControl => VK_RCONTROL,  // 0xA3
                KeyCode.LeftAlt => VK_LMENU,  // 0xA4
                KeyCode.RightAlt => VK_RMENU,  // 0xA5
                KeyCode.Mouse0 => VK_LBUTTON,  // 0x01
                KeyCode.Mouse1 => VK_RBUTTON,  // 0x02
                KeyCode.Mouse2 => VK_MBUTTON,  // 0x04
                KeyCode.Insert => VK_INSERT,  // 0x2D
                KeyCode.Delete => VK_DELETE,  // 0x2E
                KeyCode.Home => VK_HOME,  // 0x24
                KeyCode.End => VK_END,  // 0x23
                KeyCode.PageUp => VK_PRIOR,  // 0x21
                KeyCode.PageDown => VK_NEXT,  // 0x22
                KeyCode.CapsLock => VK_CAPITAL,  // 0x14
                KeyCode.Numlock => VK_NUMLOCK,  // 0x90
                KeyCode.ScrollLock => VK_SCROLL,  // 0x91
                KeyCode.Print => VK_PRINT,  // 0x2A
                KeyCode.Pause => VK_PAUSE,  // 0x13
                KeyCode.KeypadEnter => VK_RETURN,  // 0x0D
                KeyCode.KeypadPlus => VK_ADD,  // 0x6B
                KeyCode.KeypadMinus => VK_SUBTRACT,  // 0x6D
                KeyCode.KeypadMultiply => VK_MULTIPLY,  // 0x6A
                KeyCode.KeypadDivide => VK_DIVIDE,  // 0x6F
                KeyCode.KeypadPeriod => VK_DECIMAL,  // 0x6E
                KeyCode.LeftCommand => VK_LWIN,  // 0x5B
                KeyCode.RightCommand => VK_RWIN,  // 0x5C
                KeyCode.Menu => VK_APPS,  // 0x5D
                KeyCode.Semicolon => VK_OEM_1,  // 0xBA
                KeyCode.Equals => VK_OEM_PLUS,  // 0xBB
                KeyCode.Comma => VK_OEM_COMMA,  // 0xBC
                KeyCode.Minus => VK_OEM_MINUS,  // 0xBD
                KeyCode.Period => VK_OEM_PERIOD,  // 0xBE
                KeyCode.Slash => VK_OEM_2,  // 0xBF
                KeyCode.BackQuote => VK_OEM_3,  // 0xC0
                KeyCode.LeftBracket => VK_OEM_4,  // 0xDB
                KeyCode.Backslash => VK_OEM_5,  // 0xDC
                KeyCode.RightBracket => VK_OEM_6,  // 0xDD
                KeyCode.Quote => VK_OEM_7,  // 0xDE
                _ => 0xFF
            };
        }
        public static bool IsExtendedKey(uint keyCode)
        {
            return keyCode is
                VK_MENU or
                VK_LMENU or
                VK_RMENU or
                VK_CONTROL or
                VK_RCONTROL or
                VK_INSERT or
                VK_DELETE or
                VK_HOME or
                VK_END or
                VK_PRIOR or
                VK_NEXT or
                VK_RIGHT or
                VK_UP or
                VK_LEFT or
                VK_DOWN or
                VK_NUMLOCK or
                VK_CANCEL or
                VK_SNAPSHOT or
                VK_DIVIDE;
        }
    }
}
