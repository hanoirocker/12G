namespace TwelveG.UIController
{
  using UnityEngine;

  public class CinematicBarsHandler : MonoBehaviour
  {
    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onCinematicBarsAnimationFinished;


    public void CinematicBarsAnimationFinished()
    {
      Debug.Log("Disparando onCinematicBarsAnimationFinished");
      onCinematicBarsAnimationFinished.Raise(this, null);
    }
  }
}