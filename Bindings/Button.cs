using UnityEngine;
using PANEGamepad.Gamepad;

namespace PANEGamepad.Bindings
{
    public class Button
    {
        private readonly KeyCode keyCode = default;
        private readonly GamepadCode buttonCode = GamepadCode.Count;

        public Button(KeyCode keyCode)
        {
            this.keyCode = keyCode;
        }
        public Button(GamepadCode buttonCode)
        {
            this.buttonCode = buttonCode;
        }

        public Button(string buttonName)
        {
            buttonCode = (buttonName == "") ? GamepadCode.None : (GamepadCode)(((int)System.Enum.Parse(typeof(GamepadCode), buttonName, ignoreCase: true)) % (int)GamepadCode.Count);
        }

        public new string ToString()
        {
            return buttonCode != GamepadCode.Count ? (buttonCode == GamepadCode.None ? "" : buttonCode.ToString()) : keyCode.ToString();
        }

        public int Code => 1 + (int)buttonCode + ((int)keyCode * (int)GamepadCode.Count);

        public static bool GetButtonDown(Button button)
        {
            return button.keyCode != default ? Input.GetKeyDown(button.keyCode) : InputTracker.GamePad.GetButtonDown(button.buttonCode);
        }
        public static bool GetButtonUp(Button button)
        {
            return button.keyCode != default ? Input.GetKeyUp(button.keyCode) : InputTracker.GamePad.GetButtonUp(button.buttonCode);
        }
        public static bool GetButton(Button button)
        {
            return button.keyCode != default ? Input.GetKey(button.keyCode) : InputTracker.GamePad.GetButton(button.buttonCode);
        }
    }
}