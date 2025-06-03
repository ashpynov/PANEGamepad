using PANEGamepad.Configuration;
using UnityEngine;
using System;
using PANEGamepad.Native;

namespace PANEGamepad.Gamepad
{
    public class GamepadController
    {
        private Vector2 lastThumb;
        private float acceleration = 1.0f;

        public GamepadController()
        {
            ResetState(currentValue);
            ResetState(previousValue);
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

            if (vibrationFrames != 0)
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

        public void MoveMouse()
        {
            Vector2 thumb = new(GetValue(GamepadCode.LeftStickX), GetValue(GamepadCode.LeftStickY));

            float acc = Vector2.Dot(thumb, lastThumb);
            if (acc > 0)
            {
                float len = thumb.sqrMagnitude;
                acceleration = Math.Min(acceleration + (float)(acc / Settings.MouseAccelerationRate * Math.Exp(Settings.MouseAccelerationFactor * len) / Math.Exp(Settings.MouseAccelerationFactor)), Settings.MouseMaxAcceleration);
            }
            else
            {
                acceleration = 1;
            }

            lastThumb = thumb;

            float mouseDX = thumb.x * Settings.MouseSpeed * acceleration;
            float mouseDY = -thumb.y * Settings.MouseSpeed * acceleration;

            if (Settings.CapMouseMove && (mouseDX != 0 || mouseDY != 0))
            {
                Vector2 mousePos = Input.mousePosition;
                if (mousePos.x + mouseDX < 0)
                {
                    mouseDX = -mousePos.x;
                }
                else if (mousePos.x + mouseDX >= Screen.width)
                {
                    mouseDX = Screen.width - mousePos.x;
                }

                if (mousePos.y - mouseDY < 0)
                {
                    mouseDY = mousePos.y;
                }
                else if (mousePos.y - mouseDY >= Screen.height)
                {
                    mouseDY = -(Screen.height - mousePos.y);
                }
            }

            if (mouseDX != 0 || mouseDY != 0)
            {
                InputTracker.SimulateMouseMove((int)mouseDX, (int)mouseDY);
            }
        }

    }
}
