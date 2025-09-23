namespace TwelveG.GameController
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CinematicsHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<GameObject> timelineDirectors = new();
        [SerializeField] private GameObject cameraGameObject;

        [Header("Game Event So's")]
        [SerializeField] private GameEventSO CutSceneFinished;

        public void PlayerDirectorsControls(Component sender, object data)
        {
            switch (data)
            {
                case ToggleTimelineDirector director:
                    timelineDirectors[director.Index].SetActive(director.Enable);
                    break;
                default:
                    Debug.LogWarning($"[CinematicsHandler] Received unknown command: {data}");
                    break;
            }
        }

        public void TimelineFinished()
        {
            // Desactivar Object de timeline
            foreach (GameObject timeline in timelineDirectors)
            {
                if (timeline.activeSelf)
                {
                    timeline.SetActive(false);
                }
            }

            // Desactivar Camaras que hayan quedado encendidas
            foreach (Transform child in cameraGameObject.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(false);
                }
            }

            CutSceneFinished.Raise(this, null);
        }
    }
}