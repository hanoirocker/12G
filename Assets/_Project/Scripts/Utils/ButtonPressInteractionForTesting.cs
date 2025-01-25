namespace TwelveG.Utils
{
    using UnityEngine;

    public class ButtonPressInteractionForTesting : MonoBehaviour
    {
        [SerializeField] private GameObject prefabToInstantiate;

        // public GameEventSO onPlayerControls;
        public GameEventSO eventSO;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && prefabToInstantiate != null)
            {
                Instantiate(prefabToInstantiate);
            }
            else if (Input.GetKeyDown(KeyCode.V) && eventSO != null)
            {
                eventSO.Raise(this, null);
            }
        }
    }
}