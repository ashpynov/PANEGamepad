namespace PANEGamepad.Configuration
{
    public static class Settings
    {
        public static readonly float DeadZone = 0.02f;
        public static readonly float PressedZone = 0.1f;
        public static readonly int ConnectingRumbleFrames = 34;

        public static readonly float MouseSpeed = 10f;
        public static readonly float MouseMaxAcceleration = 2f;
        public static readonly float MouseAccelerationRate = 20;
        public static readonly float MouseAccelerationFactor = 10f;
        public static readonly bool CapMouseMove = true;
        public static readonly bool TrackGameFocus = true;
    }
}