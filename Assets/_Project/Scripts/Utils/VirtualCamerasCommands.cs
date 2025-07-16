namespace TwelveG.Utils
{
  public abstract class VirtualCamerasCommandsBase { }

  public enum VirtualCameraTarget
  {
    Player,
    WakeUp,
    KitchenDesk,
    Bed,
    PC,
    Backpack,
    Phone
  }

  public class ToggleVirtualCamera : VirtualCamerasCommandsBase
  {
    public VirtualCameraTarget Target;
    public bool Enabled;

    public ToggleVirtualCamera(VirtualCameraTarget target, bool enabled)
    {
      Target = target;
      Enabled = enabled;
    }
  }
}