namespace TwelveG.GameManager
{
    using System.Collections;
    using UnityEngine;

    public class GameEventBase : MonoBehaviour
    {
        public virtual IEnumerator Execute()
        {
            yield break;
        }
    }
}
