namespace PANEGamepad.Configuration
{
    public static class Settings
    {
        public static readonly float DeadZone = 0.01f;
        public static readonly float PressedZone = 0.1f;
        public static readonly int ConnectingRumbleFrames = 34;

        public static readonly float MouseSpeed = 15f;
        public static readonly bool CapMouseMove = true;
        public static readonly bool TrackGameFocus = true;
        public static readonly bool VibrationOnConnect = true;
    }
}