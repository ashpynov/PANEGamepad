using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;
using PANEGamepad.Bindings;
using PANEGamepad.Gamepad;
using PANEGamepad.Scenes;
using PANEGamepad.Native;
using PANEGamepad.Configuration;


namespace PANEGamepad
{
    public partial class InputTracker : MonoBehaviour
    {
        private readonly ManualLogSource Debug = Plugin.Log;


        private bool needResetPos = false;
        private bool needDetectScene = false;

        private readonly List<Binding> bindings = new();
        public static readonly GamepadController GamePad = new GamepadController();
        public static readonly SceneController Scene = new SceneController();

        public static GameObject delayedFocusObject = null;
        public static bool delayedFocus = false;

        public InputTracker()
        {
            RegisterBindings();
        }
        private void Update()
        {
            if (needResetPos)
            {
                SetCursorPos(startCursorPos);
                _current = null;
                needResetPos = false;
            }

            if (delayedFocus)
            {
                Scene.SetFocus(delayedFocusObject);
                delayedFocus = false;
            }

            if (Settings.TrackGameFocus && !User32.IsWindowFocused())
            {
                return;
            }

            GamePad.UpdateFrame();
            Scene.Invalidate();
            Scene.CheckSceneChanged(needDetectScene);
            needDetectScene = false;
            GamePad.MoveMouse();
            Binding.Prosess(bindings);

            if (Input.GetKeyUp(KeyCode.Mouse0)
            || Input.GetKeyUp(KeyCode.Mouse1)
            || GamePad.GetButtonUp(GamepadCode.A)
            || GamePad.GetButtonUp(GamepadCode.B))
            {
                needDetectScene = true;
            }
        }

        private bool SceneKeyDown(KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
            {
                SceneKeyDown(key);
            }
            return true;
        }

        private bool SceneKeyUp(KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
            {
                SceneKeyUp(key);
            }
            return true;
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

        public static void SetFocused(GameObject go)
        {
            delayedFocus = true;
            delayedFocusObject = go;
        }
        public static void SetMoused(GameObject go)
        {
            Plugin.Log.LogInfo($"SetMoused {go.name}");
            SceneController.EnsureVisible(go);
            SetCursorPos(SceneController.CenterPoint(go));
        }
    }
}