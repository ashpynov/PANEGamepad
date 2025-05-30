using UnityEngine;

namespace PANEGamepad.Native
{
    public static partial class User32
    {
        private static uint KeyCodeToMouseEventFlag(KeyCode key, bool down)
        {
            return key switch
            {
                KeyCode.Mouse0 => down ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP,
                KeyCode.Mouse1 => down ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP,
                KeyCode.Mouse2 => down ? MOUSEEVENTF_MIDDLEDOWN : MOUSEEVENTF_MIDDLEUP,
                _ => MOUSEEVENTF_MOVE
            };
        }
    }
}