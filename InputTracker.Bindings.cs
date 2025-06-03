using UnityEngine;
using PANEGamepad.Bindings;
using PANEGamepad.Scenes;
using PANEGamepad.Scenes.Customization;

namespace PANEGamepad
{
    public partial class InputTracker : MonoBehaviour
    {
        private void RegisterBindings()
        {
            MapKey("A", KeyCode.Mouse0);
            MapKey("B", KeyCode.Mouse1);

            // MapKey(KeyCode.Mouse0, KeyCode.Mouse0);
            // MapKey(KeyCode.Mouse1, KeyCode.Mouse1);

            MapKey("RSUp", KeyCode.W);                                      // Camera Up (W)
            MapKey("RSLeft", KeyCode.A);                                    // Camera Left (A)
            MapKey("RSDown", KeyCode.S);                                    // Camera Right (S)
            MapKey("RSRight", KeyCode.D);                                   // Camera Down (D)

            MapKey("RT", KeyCode.LeftControl, MainGameScene.Code);          // Pick building (LeftControl)
            MapKey("X", KeyCode.R, MainGameScene.Code);                     // Rotate
            MapKey("Y", "Undo", MainGameScene.Code);                        // Undo
            MapKey("LB", "Pause", MainGameScene.Code);                      // Pause
            MapKey("RB", "NormalSpeed", MainGameScene.Code);                // Resume
            MapKey("LT + LB", "LowSpeed", MainGameScene.Code);              // Slower
            MapKey("LT + RB", "HighSpeed", MainGameScene.Code);             // Faster

            MapKey("LT + A", "House", MainGameScene.Code);                  // House
            MapKey("LT + B", "Remove", MainGameScene.Code);                 // Remove
            MapKey("LT + X", "Road", MainGameScene.Code);                   // Road
            MapKey("LT + Y", "Roadblock", MainGameScene.Code);              // Roadblock
            MapKey("Menu", "MenuButton", MainGameScene.Code);               // Menu
            MapKey("LS", "FlatToggle", MainGameScene.Code);                 // Flat Mode
            MapKey("LT + LS", "GridToggle", MainGameScene.Code);            // Grid Mode

            MapFunc("LT + Up", MainGameScene.Code, OverlayController.PreviousOverlay);  // Switch to previouse overlay
            MapFunc("LT + Down", MainGameScene.Code, OverlayController.NextOverlay);    // Switch to next overlay
            MapFunc("LT + Back", MainGameScene.Code, OverlayController.ToggleOverlay);  // On/off overlay

            MapFunc("Back", MainGameScene.Code, OverseersScene.OpenLast);   // Open overseers

            MapFunc("A", SceneCode.SingleConfirm, SingleConfirmScene.PressConfirm);  // Confirm on A

            MapFunc("LT + Up", OverseersScene.Code, OverseersScene.Previous);
            MapFunc("LT + Down", OverseersScene.Code, OverseersScene.Next);

            MapKey(KeyCode.Return, KeyCode.Mouse0);
            MapKey(KeyCode.KeypadEnter, KeyCode.Mouse0);

            MapNav("Up", Vector3.up);
            MapNav("Down", Vector3.down);
            MapNav("Left", Vector3.left);
            MapNav("Right", Vector3.right);

            MapNav(KeyCode.UpArrow, Vector3.up);
            MapNav(KeyCode.DownArrow, Vector3.down);
            MapNav(KeyCode.LeftArrow, Vector3.left);
            MapNav(KeyCode.RightArrow, Vector3.right);

            bindings.Sort(Binding.CompareReverse);
        }
    }
}