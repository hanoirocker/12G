using UnityEngine;

namespace TwelveG.VFXController
{
    public class ResonanceZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerCapsule"))
            {
                // Avisa al VFXManager que entre en zona de resonancia
                VFXManager.Instance.ResonanceZoneEntered(transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PlayerCapsule"))
            {
                // Avisa al VFXManager que salga de zona de resonancia
                VFXManager.Instance.ResonanceZoneExited(transform);
            }
        }
    }
}