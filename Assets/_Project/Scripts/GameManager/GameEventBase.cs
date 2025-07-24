namespace TwelveG.GameController
{
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(GameEventListener))]
    public class GameEventBase : MonoBehaviour
    {
        public virtual IEnumerator Execute()
        {
            yield break;
        }
    }
}
