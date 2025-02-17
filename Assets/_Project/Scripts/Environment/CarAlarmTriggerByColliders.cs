namespace TwelveG.Environment
{
    using UnityEngine;

    public class CarAlarmTriggerByColliders : MonoBehaviour
    {
        public GameEventSO carAlarmTrigger;

        private void OnTriggerEnter(Collider other)
        {
            // Verifica si el objeto que entra es el jugador.
            if (other.gameObject.CompareTag("Player"))
            {
                carAlarmTrigger.Raise(this, null);
                Destroy(gameObject);
            }
        }
    }
}