using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public class EventTriggeredByColliders : MonoBehaviour
    {
        public GameEventSO eventTriggered;

        private void OnTriggerEnter(Collider other)
        {
            // Verifica si el objeto que entra es el jugador.
            if (other.gameObject.CompareTag("PlayerCapsule"))
            {
                eventTriggered.Raise(this, null);
                Destroy(gameObject);
            }
        }
    }
}