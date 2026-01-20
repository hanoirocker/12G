using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public class EventTriggeredByColliders : MonoBehaviour
    {
        public GameEventSO eventTriggered;
        public GameEventSO eventTriggerOnExit;
        public bool disableOnTriggerEnter = true;
        public bool disableOnTriggerExit = true;
        public bool IsPlayerInside { get; private set; } = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PlayerCapsule"))
            {
                IsPlayerInside = true;

                eventTriggered?.Raise(this, null);

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
                IsPlayerInside = false;

                eventTriggerOnExit?.Raise(this, null);

                if (disableOnTriggerExit)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}