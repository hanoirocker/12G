namespace TwelveG.Utils
{
  using Cinemachine;
  using UnityEngine;

  public abstract class VirtualCamerasCommandsBase { }

  public enum VirtualCameraTarget
  {
    Player,
    WakeUp,
    KitchenDesk,
    Bed,
    PC,
    Backpack,
    Phone,
    TV,
    Sofa,
    SafeBox,
    Flashlight,
    Focus,
    KitchenDepot
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

  public class LookAtTarget : VirtualCamerasCommandsBase
  {
    public Transform LookAtTransform;

    public LookAtTarget(Transform lookAtTransform) =>
      (LookAtTransform) = (lookAtTransform);
  }

  public class ResetCinemachineBrain : VirtualCamerasCommandsBase { }

  public class ToggleIntoCinematicCameras : VirtualCamerasCommandsBase
  {
    public bool Enabled;
    public ToggleIntoCinematicCameras(bool enabled) => Enabled = enabled;
  }

  public class SetCameraBlend : VirtualCamerasCommandsBase
  {
    public CinemachineBlendDefinition.Style Style;
    public int Time;
    public SetCameraBlend(CinemachineBlendDefinition.Style style, int time) =>
      (Style, Time) = (style, time);
  }

  public class SetLinealCameraBlend : VirtualCamerasCommandsBase
  {
    public int Time;
    public SetLinealCameraBlend(int time) => Time = time;
  }

  public class SetCustomCameraBlend : VirtualCamerasCommandsBase
  {
    public int Time;
    public SetCustomCameraBlend(int time) => Time = time;
  }
}