namespace TwelveG.PlayerController
{
    public abstract class PlayerCommandBase { }

    public class EnablePlayerShortcuts : PlayerCommandBase
    {
        public bool Enabled;
        public EnablePlayerShortcuts(bool enabled) => Enabled = enabled;
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

    public class EnablePlayerControllers : PlayerCommandBase
    {
        public bool Enabled;
        public EnablePlayerControllers(bool enabled) => Enabled = enabled;
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