using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;
using PANEGamepad.Bindings;
using PANEGamepad.Gamepad;
using PANEGamepad.Scenes;
using System.Linq;


namespace PANEGamepad
{
    public partial class InputTracker : MonoBehaviour
    {
        private readonly ManualLogSource Debug = Plugin.Log;


        private bool needResetPos = false;

        private readonly List<Binding> bindings = new();
        public static long FrameNum = 0;
        public static readonly GamepadController GamePad = new GamepadController();
        public static readonly SceneController Scene = new SceneController();


        private void MapKey(GamepadCode[] buttons, KeyCode key, SceneCode sceneCode = SceneCode.None) =>
            bindings.Add(new Binding(buttons, sceneCode, () => SceneKeyDown(key), () => SceneKeyUp(key)));

        private void MapKey(string buttons, KeyCode key, SceneCode sceneCode = SceneCode.None) =>
            bindings.Add(new Binding(buttons, sceneCode, () => SceneKeyDown(key), () => SceneKeyUp(key)));

        private void MapKey(KeyCode[] buttons, KeyCode key, SceneCode sceneCode = SceneCode.None) =>
            bindings.Add(new Binding(buttons, sceneCode, () => SceneKeyDown(key), () => SceneKeyUp(key)));

        private void MapKey(GamepadCode[] buttons, string sceneButtonName, SceneCode sceneCode) =>
            bindings.Add(new Binding(buttons, sceneCode, () => Scene.PressSceneButton(sceneCode, sceneButtonName)));

        private void MapKey(string buttons, string sceneButtonName, SceneCode sceneCode) =>
            bindings.Add(new Binding(buttons, sceneCode, () => Scene.PressSceneButton(sceneCode, sceneButtonName)));

        private void MapKey(KeyCode button, KeyCode key, SceneCode sceneCode = SceneCode.None) => MapKey([button], key, sceneCode);
        private void MapKey(GamepadCode button, KeyCode key, SceneCode sceneCode = SceneCode.None) => MapKey([button], key, sceneCode);
        private void MapKey(GamepadCode button, string sceneButtonName, SceneCode sceneCode) => MapKey([button], sceneButtonName, sceneCode);

        private void MapNav(GamepadCode button, Vector3 direction) =>
            bindings.Add(new Binding(button, onDown: () => Navigate(direction)));
        private void MapNav(string button, Vector3 direction) =>
            bindings.Add(new Binding(button, onDown: () => Navigate(direction)));
        private void MapNav(KeyCode button, Vector3 direction) =>
            bindings.Add(new Binding(button, onDown: () => Navigate(direction)));

        public InputTracker()
        {
            MapKey("A", KeyCode.Mouse0);
            MapKey("B", KeyCode.Mouse1);

            // MapKey(KeyCode.Mouse0, KeyCode.Mouse0);
            // MapKey(KeyCode.Mouse1, KeyCode.Mouse1);

            MapKey("RSUp", KeyCode.W);                                  // Camera Up (W)
            MapKey("RSLeft", KeyCode.A);                                // Camera Left (A)
            MapKey("RSDown", KeyCode.S);                                // Camera Right (S)
            MapKey("RSRight", KeyCode.D);                               // Camera Down (D)

            MapKey("X", KeyCode.LeftControl, SceneCode.MainGame);       // Pick building (LeftControl)
            MapKey("Y", "Undo", SceneCode.MainGame);                    // Undo
            MapKey("LB", "LowSpeed", SceneCode.MainGame);               // Slower
            MapKey("RB", "HighSpeed", SceneCode.MainGame);              // Faster

            MapKey("LT + A", "House", SceneCode.MainGame);              // House
            MapKey("LT + B", "Remove", SceneCode.MainGame);             // Remove
            MapKey("LT + X", "Road", SceneCode.MainGame);               // Road
            MapKey("LT + Y", "Roadblock", SceneCode.MainGame);          // Roadblock

            bindings.Add(new Binding("LT + Down", onDown: () => NextObserver(true)));
            bindings.Add(new Binding("LT + Up", onDown: () => NextObserver(false)));

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

            bindings.Sort((a, b) => b.Order - a.Order);
        }
        private void Update()
        {
            FrameNum++;
            if (needResetPos)
            {
                SetCursorPos(startCursorPos);
                _current = null;
                needResetPos = false;
            }

            GamePad.UpdateFrame();
            Scene.Invalidate();
            GamePad.MoveMouse();
            Binding.Prosess(bindings);
        }

        private bool SceneKeyDown(KeyCode key)
        {
            if (!CustomSceneProcess(key))
            {
                SimulateKey(key, down: true);
            }
            return true;
        }

        private bool SceneKeyUp(KeyCode key)
        {
            SimulateKey(key, down: false);
            needResetPos = (key is KeyCode.Mouse0 or KeyCode.Mouse1)
                && _current != null
                && !MouseWasMoved()
                && (key is KeyCode.Mouse1 || _current.GetComponent("BuildingBarElement")?.GetType().Name == "BuildingBarElement")
            ;
            return true;
        }

        private bool NextObserver(bool next = true)
        {
            GameObject ovs = GameObject.Find("Canvas/UI/Overseers");

            List<GameObject> overseers = new();
            if (ovs == null || !ovs.activeInHierarchy)
            {
                return false;
            }
            for (int i = 0; i < ovs.transform.childCount; i++)
            {
                overseers.Add(ovs.transform.GetChild(i).gameObject);
            }

            if (overseers.Count == 0)
            {
                return false;
            }

            List<GameObject> buttons = new();
            GameObject selectors = GameObject.Find("Canvas/UI/Bars/LeftBar/OverseerButton/OverseerSelectors");

            if (selectors == null || !selectors.activeInHierarchy)
            {
                return false;
            }

            Dictionary<string, string> DlgToButton = new()
            {
                {"ChiefOverseer", "chiefOverseer"},
                {"WorkersOverseer", "workforce"},
                {"MilitaryOverseer","military" },
                {"PoliticalOverseer", "political"},
                {"RatingsOverseer", "ratings"},
                {"CommerceOverseer", "commerce"},
                {"PopulationOverseer", "pop"},
                {"HealthOverseer", "health"},
                {"EducationOverseer", "education"},
                {"EntertainmentOverseer", "entertainment"},
                {"TempleOverseer", "religion"},
                {"TreasuryOverseer", "treasury"},
                {"MonumentOverseer", "monuments"}
            };

            for (int i = 0; i < selectors.transform.childCount; i++)
            {
                GameObject b = selectors.transform.GetChild(i).gameObject;
                if (b.activeSelf && DlgToButton.ContainsValue(b.name))
                {
                    buttons.Add(b);
                }
            }
            if (buttons.Count < 2)
            {
                return false;
            }

            GameObject activeDlg = overseers.FirstOrDefault(o => o.activeSelf);
            if (!activeDlg)
            {
                return false;
            }

            int idx = buttons.FindIndex(b => b.name == DlgToButton[activeDlg.name]);
            if (idx == -1)
            {
                return false;
            }
            if (next && idx < buttons.Count - 1)
            {
                idx++;
            }
            else if (!next && idx > 0)
            {
                idx--;
            }
            else
            {
                idx = -1;
            }

            if (idx != -1)
            {
                SceneController.PressButton(buttons[idx]);
                Scene.SetFocus(buttons[idx]);
                return true;
            }
            return false;
        }
    }
}