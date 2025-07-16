namespace TwelveG.UIManagement
{
  public abstract class CommonCanvasCommandsBase { }
  
  // Meant for setActive()
  public class ActivateCanvas : CommonCanvasCommandsBase
  {
    public bool Activate;
    public ActivateCanvas(bool activate) => Activate = activate;
  }

  // Meant to alternate current state
  public class AlternateCanvasCurrentState : CommonCanvasCommandsBase { }


  // Meant for object.enabled = <enabled>
  public class EnableCanvas : CommonCanvasCommandsBase
  {
    public bool Enabled;
    public EnableCanvas(bool enabled) => Enabled = enabled;
  }
}