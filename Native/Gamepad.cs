using System.Runtime.InteropServices;

namespace PANEGamepad.Native
{
    internal class Imports
    {
        internal const string DLLName = "xinput1_4.dll";

        [DllImport(DLLName)]
        public static extern uint XInputGetState(uint playerIndex, out GamePadState.RawState state);
        [DllImport(DLLName)]
        public static extern int XInputSetState(int dwUserIndex, ref XInputVibration pVibration);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInputVibration
    {
        public ushort wLeftMotorSpeed;
        public ushort wRightMotorSpeed;
    }

    public enum ButtonState
    {
        Pressed,
        Released
    }

    public struct GamePadButtons
    {
        internal ButtonState start, back, leftStick, rightStick, leftShoulder, rightShoulder, guide, a, b, x, y;

        internal GamePadButtons(ButtonState start, ButtonState back, ButtonState leftStick, ButtonState rightStick,
                                ButtonState leftShoulder, ButtonState rightShoulder, ButtonState guide,
                                ButtonState a, ButtonState b, ButtonState x, ButtonState y)
        {
            this.start = start;
            this.back = back;
            this.leftStick = leftStick;
            this.rightStick = rightStick;
            this.leftShoulder = leftShoulder;
            this.rightShoulder = rightShoulder;
            this.guide = guide;
            this.a = a;
            this.b = b;
            this.x = x;
            this.y = y;
        }

        public readonly ButtonState Start => start;

        public readonly ButtonState Back => back;

        public readonly ButtonState LeftStick => leftStick;

        public readonly ButtonState RightStick => rightStick;

        public readonly ButtonState LeftShoulder => leftShoulder;

        public readonly ButtonState RightShoulder => rightShoulder;

        public readonly ButtonState Guide => guide;

        public readonly ButtonState A => a;

        public readonly ButtonState B => b;

        public readonly ButtonState X => x;

        public readonly ButtonState Y => y;
    }

    public struct GamePadDPad
    {
        internal ButtonState up, down, left, right;

        internal GamePadDPad(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }

        public readonly ButtonState Up => up;

        public readonly ButtonState Down => down;

        public readonly ButtonState Left => left;

        public readonly ButtonState Right => right;
    }

    public struct GamePadThumbSticks
    {
        public struct StickValue
        {
            internal float x, y;

            internal StickValue(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public readonly float X => x;

            public readonly float Y => y;
        }

        internal StickValue left, right;

        internal GamePadThumbSticks(StickValue left, StickValue right)
        {
            this.left = left;
            this.right = right;
        }

        public readonly StickValue Left => left;

        public readonly StickValue Right => right;
    }

    public struct GamePadTriggers
    {
        internal float left;
        internal float right;

        internal GamePadTriggers(float left, float right)
        {
            this.left = left;
            this.right = right;
        }

        public readonly float Left => left;

        public readonly float Right => right;
    }

    public struct GamePadState
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct RawState
        {
            public uint dwPacketNumber;
            public GamePad Gamepad;

            [StructLayout(LayoutKind.Sequential)]
            public struct GamePad
            {
                public ushort wButtons;
                public byte bLeftTrigger;
                public byte bRightTrigger;
                public short sThumbLX;
                public short sThumbLY;
                public short sThumbRX;
                public short sThumbRY;
            }
        }

        private readonly bool isConnected;
        private GamePadButtons buttons;
        private GamePadDPad dPad;
        private GamePadThumbSticks thumbSticks;
        private GamePadTriggers triggers;

        private enum ButtonsConstants
        {
            DPadUp = 0x00000001,
            DPadDown = 0x00000002,
            DPadLeft = 0x00000004,
            DPadRight = 0x00000008,
            Start = 0x00000010,
            Back = 0x00000020,
            LeftThumb = 0x00000040,
            RightThumb = 0x00000080,
            LeftShoulder = 0x0100,
            RightShoulder = 0x0200,
            Guide = 0x0400,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000
        }

        internal GamePadState(bool isConnected, RawState rawState, GamePadDeadZone deadZone)
        {
            this.isConnected = isConnected;

            if (!isConnected)
            {
                rawState.dwPacketNumber = 0;
                rawState.Gamepad.wButtons = 0;
                rawState.Gamepad.bLeftTrigger = 0;
                rawState.Gamepad.bRightTrigger = 0;
                rawState.Gamepad.sThumbLX = 0;
                rawState.Gamepad.sThumbLY = 0;
                rawState.Gamepad.sThumbRX = 0;
                rawState.Gamepad.sThumbRY = 0;
            }

            PacketNumber = rawState.dwPacketNumber;
            buttons = new GamePadButtons(
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.Start) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.Back) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.LeftThumb) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.RightThumb) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.LeftShoulder) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.RightShoulder) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.Guide) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.A) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.B) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.X) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.Y) != 0 ? ButtonState.Pressed : ButtonState.Released
            );
            dPad = new GamePadDPad(
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.DPadUp) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.DPadDown) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.DPadLeft) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.DPadRight) != 0 ? ButtonState.Pressed : ButtonState.Released
            );

            thumbSticks = new GamePadThumbSticks(
                Utils.ApplyLeftStickDeadZone(rawState.Gamepad.sThumbLX, rawState.Gamepad.sThumbLY, deadZone),
                Utils.ApplyRightStickDeadZone(rawState.Gamepad.sThumbRX, rawState.Gamepad.sThumbRY, deadZone)
            );
            triggers = new GamePadTriggers(
                Utils.ApplyTriggerDeadZone(rawState.Gamepad.bLeftTrigger, deadZone),
                Utils.ApplyTriggerDeadZone(rawState.Gamepad.bRightTrigger, deadZone)
            );
        }

        public readonly uint PacketNumber { get; }

        public readonly bool IsConnected => isConnected;

        public readonly GamePadButtons Buttons => buttons;

        public readonly GamePadDPad DPad => dPad;

        public readonly GamePadTriggers Triggers => triggers;

        public readonly GamePadThumbSticks ThumbSticks => thumbSticks;
    }

    public enum PlayerIndex
    {
        One = 0,
        Two,
        Three,
        Four
    }

    public enum GamePadDeadZone
    {
        Circular,
        IndependentAxes,
        None
    }

    public class GamePad
    {
        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            return GetState(playerIndex, GamePadDeadZone.IndependentAxes);
        }

        public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZone)
        {
            uint result = Imports.XInputGetState((uint)playerIndex, out GamePadState.RawState state);
            return new GamePadState(result == Utils.Success, state, deadZone);
        }

        public static void SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
        {
            XInputVibration vibration = new()
            {
                wLeftMotorSpeed = (ushort)(65535 * leftMotor),
                wRightMotorSpeed = (ushort)(65535 * rightMotor)
            };
            Imports.XInputSetState((int)playerIndex, ref vibration);
        }
    }
    internal static class Utils
    {
        public const uint Success = 0x000;
        public const uint NotConnected = 0x000;

        private const int LeftStickDeadZone = 7849;
        private const int RightStickDeadZone = 8689;
        private const int TriggerDeadZone = 30;

        public static float ApplyTriggerDeadZone(byte value, GamePadDeadZone deadZoneMode)
        {
            return deadZoneMode == GamePadDeadZone.None
                ? ApplyDeadZone(value, byte.MaxValue, 0.0f)
                : ApplyDeadZone(value, byte.MaxValue, TriggerDeadZone);
        }

        public static GamePadThumbSticks.StickValue ApplyLeftStickDeadZone(short valueX, short valueY, GamePadDeadZone deadZoneMode)
        {
            return ApplyStickDeadZone(valueX, valueY, deadZoneMode, LeftStickDeadZone);
        }

        public static GamePadThumbSticks.StickValue ApplyRightStickDeadZone(short valueX, short valueY, GamePadDeadZone deadZoneMode)
        {
            return ApplyStickDeadZone(valueX, valueY, deadZoneMode, RightStickDeadZone);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0004:Remove Unnecessary Cast", Justification = "<Pending>")]
        private static GamePadThumbSticks.StickValue ApplyStickDeadZone(short valueX, short valueY, GamePadDeadZone deadZoneMode, int deadZoneSize)
        {
            if (deadZoneMode == GamePadDeadZone.Circular)
            {
                // Cast to long to avoid int overflow if valueX and valueY are both 32768, which would result in a negative number and Sqrt returns NaN
                float distanceFromCenter = (float)System.Math.Sqrt(((long)valueX * (long)valueX) + ((long)valueY * (long)valueY));
                float coefficient = ApplyDeadZone(distanceFromCenter, short.MaxValue, deadZoneSize);
                coefficient = coefficient > 0.0f ? coefficient / distanceFromCenter : 0.0f;
                return new GamePadThumbSticks.StickValue(
                    Clamp(valueX * coefficient),
                    Clamp(valueY * coefficient)
                );
            }
            else
            {
                return deadZoneMode == GamePadDeadZone.IndependentAxes
                    ? new GamePadThumbSticks.StickValue(
                                    ApplyDeadZone(valueX, short.MaxValue, deadZoneSize),
                                    ApplyDeadZone(valueY, short.MaxValue, deadZoneSize)
                                )
                    : new GamePadThumbSticks.StickValue(
                                    ApplyDeadZone(valueX, short.MaxValue, 0.0f),
                                    ApplyDeadZone(valueY, short.MaxValue, 0.0f)
                                );
            }
        }

        private static float Clamp(float value)
        {
            return value < -1.0f ? -1.0f : (value > 1.0f ? 1.0f : value);
        }

        private static float ApplyDeadZone(float value, float maxValue, float deadZoneSize)
        {
            if (value < -deadZoneSize)
            {
                value += deadZoneSize;
            }
            else if (value > deadZoneSize)
            {
                value -= deadZoneSize;
            }
            else
            {
                return 0.0f;
            }

            value /= maxValue - deadZoneSize;

            return Clamp(value);
        }
    }
}