namespace TwelveG.PlayerController
{
    public abstract class PlayerCommandBase { }

    public class TogglePlayerShortcuts : PlayerCommandBase
    {
        public bool Enabled;
        public TogglePlayerShortcuts(bool enabled) => Enabled = enabled;
    }

    public class TogglePlayerHeadLookAround : PlayerCommandBase
    {
        public bool Enabled;
        public TogglePlayerHeadLookAround(bool enabled) => Enabled = enabled;
    }

    public class TogglePlayerCapsule : PlayerCommandBase
    {
        public bool Enabled;
        public TogglePlayerCapsule(bool enabled) => Enabled = enabled;
    }

    public class TogglePlayerMainCamera : PlayerCommandBase
    {
        public bool Enabled;
        public TogglePlayerMainCamera(bool enabled) => Enabled = enabled;
    }

    public class TogglePlayerCameraZoom : PlayerCommandBase
    {
        public bool Enabled;
        public TogglePlayerCameraZoom(bool enabled) => Enabled = enabled;
    }
}