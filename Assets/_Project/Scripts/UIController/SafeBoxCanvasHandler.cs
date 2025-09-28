namespace TwelveG.GameController
{
  using UnityEngine;

  public class SafeBoxCanvasHandler : MonoBehaviour
  {

    private void OnEnable()
    {
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
    }

    void OnDisable()
    {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }
  }
}