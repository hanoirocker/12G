namespace TwelveG.GameController
{
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(GameEventListener))]
    public class GameEventBase : MonoBehaviour
    {
        public bool isCheckpointEvent = false;
        public virtual IEnumerator Execute()
        {
            yield break;
        }
    }
}
