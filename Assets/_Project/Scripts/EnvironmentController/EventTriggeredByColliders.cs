using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public class EventTriggeredByColliders : MonoBehaviour
    {
        public GameEventSO eventTriggered;
        public GameEventSO eventTriggerOnExit;
        public bool disableOnTriggerEnter = true;
        public bool disableOnTriggerExit = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PlayerCapsule"))
            {
                eventTriggered.Raise(this, null);

                if (disableOnTriggerEnter)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("PlayerCapsule"))
            {
                eventTriggerOnExit.Raise(this, null);
                if (disableOnTriggerExit)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}