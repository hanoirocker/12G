using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.UIController
{
  public class CinematicBarsHandler : MonoBehaviour
  {
    public void CinematicBarsAnimationFinished()
    {
      Debug.Log("Disparando onCinematicBarsAnimationFinished");
      GameEvents.Common.onCinematicBarsAnimationFinished.Raise(this, null);
    }
  }
}