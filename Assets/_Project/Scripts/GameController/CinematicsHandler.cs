using System.Collections.Generic;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
    public class CinematicsHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<GameObject> timelineDirectors = new();
        [SerializeField] private GameObject cameraGameObject;

        public void PlayerDirectorsControls(Component sender, object data)
        {
            switch (data)
            {
                case ToggleTimelineDirector director:
                    timelineDirectors[director.Index].SetActive(director.Enable);
                    GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleIntoCinematicCameras(true));
                    break;
                default:
                    Debug.LogWarning($"[CinematicsHandler] Received unknown command: {data}");
                    break;
            }
        }

        public void CutSceneFinished()
        {
            GameEvents.Common.onCutSceneFinished.Raise(this, null);
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleIntoCinematicCameras(false));
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

            GameEvents.Common.onTimelineFinished.Raise(this, null);
        }
    }
}