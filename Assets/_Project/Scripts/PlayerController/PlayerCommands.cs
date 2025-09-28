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

    public class EnableControlCanvasAccess : PlayerCommandBase
    {
        public bool Enabled;
        public EnableControlCanvasAccess(bool enabled) => Enabled = enabled;
    }

    public class EnablePauseMenuCanvasAccess : PlayerCommandBase
    {
        public bool Enabled;
        public EnablePauseMenuCanvasAccess(bool enabled) => Enabled = enabled;
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