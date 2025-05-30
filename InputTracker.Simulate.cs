using System;
using PANEGamepad.Native;
using UnityEngine;

namespace PANEGamepad
{
    public partial class InputTracker : MonoBehaviour
    {
        public static Vector2Int UnityScreenToWindows(Vector2 unityScreenPos)
        {
            Rect rect = User32.GetActiveWindowRect();   // Rect in window Screen coordinates
            unityScreenPos.y = Screen.height - unityScreenPos.y;
            float windowWidth = rect.width;
            float windowHeight = rect.height;

            float scale = Math.Min(windowWidth / Screen.width, windowHeight / Screen.height);

            Vector2 monitorOffset = rect.position;
            Vector2 screenOffset = new
            (
                (windowWidth - (Screen.width * scale)) / 2,
                (windowHeight - (Screen.height * scale)) / 2
            );

            Vector2 scaledPos = Vector2.Scale(unityScreenPos, new Vector2(scale, scale));
            return Vector2Int.FloorToInt(scaledPos + monitorOffset + screenOffset);
        }

        public static void SetCursorPos(Vector2 screenPos)
        {
            Vector2Int pos = UnityScreenToWindows(screenPos);
            User32.SetCursorPos(pos);
        }

        public static bool SimulateKey(KeyCode key, bool down)
        {
            User32.SendKey(key, down);
            return true;
        }
        public static bool SimulateMouseMove(int dx, int dy)
        {
            User32.MoveMouseBy(dx, dy);
            return true;
        }
    }
}
