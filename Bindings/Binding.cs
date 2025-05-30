using System.Linq;
using UnityEngine;
using PANEGamepad.Gamepad;
using PANEGamepad.Scenes;
using System.Collections.Generic;


namespace PANEGamepad.Bindings
{
    public delegate bool Handler();
    public class Binding(Button[] buttons, SceneCode sceneCode = SceneCode.None, Handler onDown = null, Handler onUp = null)
    {
        public static readonly float DelayBeforeRepeat = 0.35f;  // Initial delay (e.g., 500ms)
        public static readonly float RepeatRate = 0.07f;        // Repeat interval (e.g., 100ms)

        private readonly Button _button = buttons.Last();
        private readonly Button[] _shift = buttons.Take(buttons.Length - 1).ToArray();

        private readonly SceneCode _sceneCode = sceneCode;
        private readonly Handler _onDown = onDown;
        private readonly Handler _onUp = onUp;
        private bool _pressed = false;
        private float _holdTimer = 0;           // Time since initial press
        private float _repeatTimer = 0;         // Time since last repeat

        public int Code => _button.Code;
        public int Length => _shift.Length + 1;
        public new string ToString()
        {
            return string.Join(" + ", [.. _shift.Select(b => b.ToString()), _button.ToString()]);
        }

        public static int CompareReverse(Binding a, Binding b)
        {
            // by scenes
            int cmp = (int)b._sceneCode - (int)a._sceneCode;
            if (cmp != 0)
            {
                return cmp;
            }
            // by lengths
            cmp = b.Length - a.Length;
            if (cmp != 0)
            {
                return cmp;
            }

            // by shifts
            for (int i = 0; i < b.Length - 1; i++)
            {
                cmp = a._shift[i].Code - b._shift[i].Code;
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            // by buttons
            return a._button.Code - b._button.Code;
        }

        public Binding(string buttons, SceneCode sceneCode = SceneCode.None, Handler onDown = null, Handler onUp = null)
        : this(Parse(buttons), sceneCode, onDown, onUp) { }

        private static Button[] Parse(string buttons)
        {
            return buttons.Split('+').Select(s => new Button(s.Trim())).ToArray();
        }

        public Binding(GamepadCode[] buttons, SceneCode sceneCode = SceneCode.None, Handler onDown = null, Handler onUp = null)
        : this(buttons.Select(b => new Button(b)).ToArray(), sceneCode, onDown, onUp) { }


        public Binding(KeyCode[] keys, SceneCode sceneCode = SceneCode.None, Handler onDown = null, Handler onUp = null)
        : this(keys.Select(b => new Button(b)).ToArray(), sceneCode, onDown, onUp) { }

        public Binding(GamepadCode button, SceneCode sceneCode = SceneCode.None, Handler onDown = null, Handler onUp = null)
        : this([new Button(button)], sceneCode, onDown, onUp) { }


        public Binding(KeyCode key, SceneCode sceneCode = SceneCode.None, Handler onDown = null, Handler onUp = null)
        : this([new Button(key)], sceneCode, onDown, onUp) { }

        public bool GetHeld()
        {
            return Button.GetButton(_button) && _shift.All(Button.GetButton);
        }
        public bool GetDown()
        {
            return Button.GetButtonDown(_button) && _shift.All(Button.GetButton);
        }
        public bool GetUp()
        {
            return _pressed && (!Button.GetButton(_button) || !_shift.All(Button.GetButton));
        }

        public static Binding MapKeyboard(KeyCode key, Button[] buttons)
        {
            return new Binding(buttons, onDown: () => InputTracker.SimulateKey(key, down: true), onUp: () => InputTracker.SimulateKey(key, down: false));
        }

        public bool HandleUp()
        {
            if (GetUp())
            {
                _onUp?.Invoke();
                _pressed = false;
                Plugin.Log.LogInfo($"{ToString()} Up");
                return true;
            }
            return false;
        }
        public bool HandleDown()
        {
            if (GetDown()
            && (_sceneCode == SceneCode.None || _sceneCode == InputTracker.Scene.GetSceneCode()))
            {
                if (_onDown?.Invoke() == true)
                {
                    _pressed = true;
                    _holdTimer = 0;
                    _repeatTimer = 0;
                    Plugin.Log.LogInfo($"{ToString()} Down");
                    return true;
                }
            }
            return false;
        }

        public void HandleRepeat()
        {
            if (_pressed && GetHeld())
            {
                _holdTimer += Time.deltaTime;
                if (_holdTimer >= DelayBeforeRepeat)
                {
                    _repeatTimer += Time.deltaTime;
                    if (_repeatTimer >= RepeatRate)
                    {
                        _repeatTimer = 0f;
                        _onDown?.Invoke();  // Repeat event
                        Plugin.Log.LogInfo($"{ToString()} Repeat down");
                    }
                }
            }
            return;
        }
        public static void Prosess(List<Binding> bindings)
        {
            foreach (Binding bind in bindings)
            {
                bind.HandleUp();
                bind.HandleRepeat();
            }

            List<int> processed = new();

            foreach (Binding bind in bindings)
            {
                if (!processed.Contains(bind.Code) && bind.HandleDown())
                {
                    processed.Add(bind.Code);
                }
            }
        }
    }
}