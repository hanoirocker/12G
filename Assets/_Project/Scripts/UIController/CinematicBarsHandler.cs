namespace TwelveG.UIController
{
  using UnityEngine;

  public class CinematicBarsHandler : MonoBehaviour
  {
    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onCinematicBarsAnimationFinished;


    public void CinematicBarsAnimationFinished()
    {
      onCinematicBarsAnimationFinished.Raise(this, null);
    }
  }
}