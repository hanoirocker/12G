namespace TwelveG.GameController
{
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(GameEventListener))]
    public class GameEventBase : MonoBehaviour
    {
        public EventsEnum eventEnum;
        public bool isCheckpointEvent = false; // Solo para lectura visual rápida
        public virtual IEnumerator Execute()
        {
            yield break;
        }
    }
}
