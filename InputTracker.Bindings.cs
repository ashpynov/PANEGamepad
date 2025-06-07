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

            MapFunc("A", DropdownScene.Code, DropdownScene.SelectDropdown);  // Select on dropdowns
            MapFunc("B", DropdownScene.Code, DropdownScene.CloseDropdown);   // Close dropdowns
            MapKey("RSUp", KeyCode.W, MainGameScene.Code);                  // Camera Up (W)
            MapKey("RSLeft", KeyCode.A, MainGameScene.Code);                // Camera Left (A)
            MapKey("RSDown", KeyCode.S, MainGameScene.Code);                // Camera Right (S)
            MapKey("RSRight", KeyCode.D, MainGameScene.Code);               // Camera Down (D)

            MapKeys("X", [KeyCode.LeftControl, KeyCode.Mouse0], MainGameScene.Code);          // Pick building (LeftControl)
            MapKey("Y", KeyCode.R, MainGameScene.Code);                     // Rotate
            MapKey("LT + Start", "Undo", MainGameScene.Code);               // Undo
            MapKey("LB", "Pause", MainGameScene.Code);                      // Pause
            MapKey("RB", "NormalSpeed", MainGameScene.Code);                // Resume
            MapKey("LT + LB", "LowSpeed", MainGameScene.Code);              // Slower
            MapKey("LT + RB", "HighSpeed", MainGameScene.Code);             // Faster

            MapKey("RT + LT + Start", KeyCode.F9, MainGameScene.Code);       // Load
            MapKey("RT + LT + Back", KeyCode.F5, MainGameScene.Code);       // Save

            MapKey("LT + A", "House", MainGameScene.Code);                  // House
            MapKey("LT + B", "Remove", MainGameScene.Code);                 // Remove
            MapKey("LT + X", "Road", MainGameScene.Code);                   // Road
            MapKey("LT + Y", "Roadblock", MainGameScene.Code);              // Roadblock
            MapKey("RT + A", "Gardens", MainGameScene.Code);                // Gardens
            MapKey("RT + B", "Plaza", MainGameScene.Code);                  // Plaza
            MapKey("RT + X", "Firehouse", MainGameScene.Code);              // Firepost
            MapKey("RT + Y", "ArchitectPost", MainGameScene.Code);          // Architect


            MapKey("Menu", "MenuButton", MainGameScene.Code);               // Menu
            MapKey("LS", "FlatToggle", MainGameScene.Code);                 // Flat Mode
            MapKey("LT + LS", "GridToggle", MainGameScene.Code);            // Grid Mode
            MapKey("LT + RSDown", KeyCode.Mouse5, MainGameScene.Code);      // Zoom In
            MapKey("LT + RSUp", KeyCode.Mouse6, MainGameScene.Code);        // Zoom Out

            MapKey("RSDown", KeyCode.Mouse5);                               // ScrollDown
            MapKey("RSUp", KeyCode.Mouse6);                                 // ScrollUp


            MapKey("LT + Left", "commerce", MainGameScene.Code);            // Commerse overseer

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