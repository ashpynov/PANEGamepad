using PANEGamepad.Configuration;
using UnityEngine;
using PANEGamepad.Native;
using System.Threading;
using System;

namespace PANEGamepad.Gamepad
{
    public class GamepadController : IDisposable
    {
        private readonly Thread _inputThread;
        private readonly object _framelock = new object();
        private bool _shouldRun = true;

        public Rect _screenRect = new Rect(0, 0, 0, 0);
        public Rect ScreenRect
        {
            get
            {
                lock (_framelock)
                {
                    return _screenRect;
                }
            }
            set
            {
                lock (_framelock)
                {
                    _screenRect = value;
                }
            }
        }

        public GamepadController()
        {

            ResetState(currentValue);
            ResetState(previousValue);

            _inputThread = new Thread(InputUpdateLoop)
            {
                IsBackground = true,
                Priority = System.Threading.ThreadPriority.AboveNormal
            };
            _inputThread.Start();
        }
        private void InputUpdateLoop()
        {
            while (_shouldRun)
            {
                if (Settings.TrackGameFocus && !User32.IsWindowFocused())
                {
                    Thread.Sleep(1000);
                    continue;
                }

                GamePadState state = GamePad.GetState(playerIndex);

                if (!state.IsConnected)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                MoveMouse(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y, ScreenRect);
                Thread.Sleep(16);
            }
        }

        public void Dispose()
        {
            _shouldRun = false;
            if (_inputThread != null && _inputThread.IsAlive)
            {
                _inputThread.Join(1000);
            }
        }

        private int vibrationFrames = 0;

        private bool IsConnected = false;
        private readonly PlayerIndex playerIndex = PlayerIndex.One; // Player 1

        private float[] currentValue = new float[(int)GamepadCode.Count];
        private float[] previousValue = new float[(int)GamepadCode.Count];

        private float StateToValue(ButtonState state)
        {
            return state == ButtonState.Pressed ? 1.0f : 0.0f;
        }
        private bool ValueToBool(float value)
        {
            return value > Settings.PressedZone || value < -Settings.PressedZone;
        }
        private float ValueToValue(float value)
        {
            return (value > Settings.DeadZone || value < -Settings.DeadZone) ? value : 0;
        }

        private void ResetState(float[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = 0;
            }
        }

        private void PollState(GamePadState state, float[] states)
        {
            if (states.Length != (int)GamepadCode.Count)
            {
                return;
            }

            states[(int)GamepadCode.None] = 1.0f;
            states[(int)GamepadCode.DPadUp] = StateToValue(state.DPad.Up);
            states[(int)GamepadCode.DPadDown] = StateToValue(state.DPad.Down);
            states[(int)GamepadCode.DPadLeft] = StateToValue(state.DPad.Left);
            states[(int)GamepadCode.DPadRight] = StateToValue(state.DPad.Right);
            states[(int)GamepadCode.A] = StateToValue(state.Buttons.A);
            states[(int)GamepadCode.B] = StateToValue(state.Buttons.B);
            states[(int)GamepadCode.X] = StateToValue(state.Buttons.X);
            states[(int)GamepadCode.Y] = StateToValue(state.Buttons.Y);
            states[(int)GamepadCode.Menu] = StateToValue(state.Buttons.Start);
            states[(int)GamepadCode.View] = StateToValue(state.Buttons.Back);
            states[(int)GamepadCode.Guide] = StateToValue(state.Buttons.Guide);
            states[(int)GamepadCode.LB] = StateToValue(state.Buttons.LeftShoulder);
            states[(int)GamepadCode.RB] = StateToValue(state.Buttons.RightShoulder);
            states[(int)GamepadCode.LT] = state.Triggers.Left;
            states[(int)GamepadCode.RT] = state.Triggers.Right;
            states[(int)GamepadCode.LS] = StateToValue(state.Buttons.LeftStick);
            states[(int)GamepadCode.RS] = StateToValue(state.Buttons.RightStick);
            states[(int)GamepadCode.LeftStickX] = state.ThumbSticks.Left.X;
            states[(int)GamepadCode.LeftStickY] = state.ThumbSticks.Left.Y;
            states[(int)GamepadCode.LeftStickLeft] = state.ThumbSticks.Left.X < 0 ? state.ThumbSticks.Left.X : 0.0f;
            states[(int)GamepadCode.LeftStickRight] = state.ThumbSticks.Left.X > 0 ? state.ThumbSticks.Left.X : 0.0f;
            states[(int)GamepadCode.LeftStickUp] = state.ThumbSticks.Left.Y > 0 ? state.ThumbSticks.Left.Y : 0.0f;
            states[(int)GamepadCode.LeftStickDown] = state.ThumbSticks.Left.Y < 0 ? state.ThumbSticks.Left.Y : 0.0f;
            states[(int)GamepadCode.RightStickX] = state.ThumbSticks.Right.X;
            states[(int)GamepadCode.RightStickY] = state.ThumbSticks.Right.Y;
            states[(int)GamepadCode.RightStickLeft] = state.ThumbSticks.Right.X < 0 ? state.ThumbSticks.Right.X : 0.0f;
            states[(int)GamepadCode.RightStickRight] = state.ThumbSticks.Right.X > 0 ? state.ThumbSticks.Right.X : 0.0f;
            states[(int)GamepadCode.RightStickUp] = state.ThumbSticks.Right.Y > 0 ? state.ThumbSticks.Right.Y : 0.0f;
            states[(int)GamepadCode.RightStickDown] = state.ThumbSticks.Right.Y < 0 ? state.ThumbSticks.Right.Y : 0.0f;
        }

        public void UpdateFrame()
        {
            Vector2 offset = InputTracker.UnityScreenToWindows(new Vector2(0, Screen.height));
            ScreenRect = new(offset, new Vector2(Screen.width, Screen.height));

            GamePadState state = GamePad.GetState(playerIndex);

            if (!IsConnected && !state.IsConnected)
            {
                return;
            }

            if (!state.IsConnected)
            {
                ResetState(currentValue);
                ResetState(previousValue);
                IsConnected = false;
                Plugin.Log.LogInfo($"GamePad Disconnected");
                return;
            }

            if (IsConnected != state.IsConnected)
            {
                vibrationFrames = Settings.ConnectingRumbleFrames;
                IsConnected = true;
                Plugin.Log.LogInfo($"GamePad Connected");
            }

            if (vibrationFrames != 0 && Settings.VibrationOnConnect)
            {
                if (vibrationFrames % 5 == 0)
                {
                    bool leftMotor = vibrationFrames / 5 % 2 == 0;
                    GamePad.SetVibration(playerIndex, leftMotor ? 1.0f : 0.0f, leftMotor ? 0.0f : 1.0f);
                }

                if (vibrationFrames == 1)
                {
                    GamePad.SetVibration(playerIndex, 0.0f, 0.0f);
                }
                vibrationFrames--;
            }

            previousValue = currentValue;
            currentValue = new float[(int)GamepadCode.Count];
            PollState(state, currentValue);
        }
        public bool GetButtonDown(GamepadCode button)
        {
            return !ValueToBool(previousValue[(int)button]) && ValueToBool(currentValue[(int)button]);
        }
        public bool GetButtonUp(GamepadCode button)
        {
            return ValueToBool(previousValue[(int)button]) && !ValueToBool(currentValue[(int)button]);
        }
        public bool GetButton(GamepadCode button)
        {
            return ValueToBool(currentValue[(int)button]);
        }
        public float GetValue(GamepadCode button)
        {
            return ValueToValue(currentValue[(int)button]);
        }

        private float CubicSmooth(float v)
        {
            float sign = v < 0 ? -1f : 1f;
            float t = Mathf.Abs(v);
            return sign * Mathf.Clamp((6 * Mathf.Pow(t, 5)) - (15 * Mathf.Pow(t, 4)) + (10 * Mathf.Pow(t, 3)), 0, 1);
        }

        private float Smooth(float v)
        {
            return v;
        }

        public void MoveMouse(float dx, float dy, Rect screen)
        {
            if (screen.width == 0)
            {
                return;
            }

            float screenScale = screen.height / 1080;
            float mouseDX = Smooth(dx) * Settings.MouseSpeed * screenScale;
            float mouseDY = -Smooth(dy) * Settings.MouseSpeed * screenScale;

            if (Settings.CapMouseMove && (mouseDX != 0 || mouseDY != 0) && screen.width != 0)
            {
                Vector2 mousePos = User32.GetCursorPos();
                if (mousePos.x + mouseDX <= screen.min.x)
                {
                    mouseDX = screen.min.x - mousePos.x;
                }
                else if (mousePos.x + mouseDX >= screen.max.x)
                {
                    mouseDX = screen.max.x - mousePos.x - 1;
                }

                if (mousePos.y + mouseDY <= screen.min.y)
                {
                    mouseDY = screen.min.y - mousePos.y;
                }
                else if (mousePos.y + mouseDY >= screen.max.y)
                {
                    mouseDY = screen.max.y - mousePos.y - 1;
                }
            }

            if (mouseDX != 0 || mouseDY != 0)
            {
                InputTracker.SimulateMouseMove((int)mouseDX, (int)mouseDY);
            }
        }

    }
}
