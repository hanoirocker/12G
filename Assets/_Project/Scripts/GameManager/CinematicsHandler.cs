namespace TwelveG.GameController
{
  using System.Collections.Generic;
  using UnityEngine;
    using UnityEngine.Playables;

    public class CinematicsHandler : MonoBehaviour
    {
        [SerializeField] private List<PlayableDirector> timelineDirectors = new();

        public void PlayerDirectorsControls(Component sender, object data)
        {
            switch (data)
            {
                case ToggleTimelineDirector director:
                    timelineDirectors[director.Index].enabled = director.Enable;
                    break;
                default:
                    Debug.LogWarning($"[CinematicsHandler] Received unknown command: {data}");
                    break;
            }
        }
    }
}