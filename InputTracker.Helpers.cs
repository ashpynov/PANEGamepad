using UnityEngine;
using PANEGamepad.Bindings;
using PANEGamepad.Gamepad;
using PANEGamepad.Scenes;


namespace PANEGamepad
{
    public partial class InputTracker : MonoBehaviour
    {
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

        private void MapKey(KeyCode button, KeyCode key, SceneCode sceneCode = SceneCode.None) =>
            MapKey([button], key, sceneCode);

        private void MapKey(GamepadCode button, KeyCode key, SceneCode sceneCode = SceneCode.None) =>
            MapKey([button], key, sceneCode);

        private void MapKey(GamepadCode button, string sceneButtonName, SceneCode sceneCode) =>
            MapKey([button], sceneButtonName, sceneCode);

        private void MapNav(GamepadCode button, Vector3 direction) =>
            bindings.Add(new Binding(button, onDown: () => Navigate(direction)));

        private void MapNav(string button, Vector3 direction) =>
            bindings.Add(new Binding(button, onDown: () => Navigate(direction)));

        private void MapNav(KeyCode button, Vector3 direction) =>
            bindings.Add(new Binding(button, onDown: () => Navigate(direction)));

        private void MapFunc(string button, SceneCode sceneCode, Handler onDown) =>
            bindings.Add(new Binding(button, sceneCode: sceneCode, onDown: onDown));

        private void MapFunc(string button, Handler onDown) =>
            bindings.Add(new Binding(button, onDown: onDown));

    }
}