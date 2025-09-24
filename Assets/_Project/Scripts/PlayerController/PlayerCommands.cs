namespace TwelveG.PlayerController
{
    public abstract class PlayerCommandBase { }

    public class EnablePlayerShortcuts : PlayerCommandBase
    {
        public bool Enabled;
        public EnablePlayerShortcuts(bool enabled) => Enabled = enabled;
    }

    public class EnablePlayerHeadLookAround : PlayerCommandBase
    {
        public bool Enabled;
        public EnablePlayerHeadLookAround(bool enabled) => Enabled = enabled;
    }

    public class TogglePlayerCapsule : PlayerCommandBase
    {
        public bool Enabled;
        public TogglePlayerCapsule(bool enabled) => Enabled = enabled;
    }

    public class EnableCanvasControlsAccess : PlayerCommandBase
    {
        public bool Enabled;
        public EnableCanvasControlsAccess(bool enabled) => Enabled = enabled;
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

    public class EnablePlayerCameraZoom : PlayerCommandBase
    {
        public bool Enabled;
        public EnablePlayerCameraZoom(bool enabled) => Enabled = enabled;
    }
}