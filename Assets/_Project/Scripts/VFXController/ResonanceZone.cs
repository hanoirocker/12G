using UnityEngine;

namespace TwelveG.VFXController
{
    public class ResonanceZone : MonoBehaviour
    {
        private bool zoneIsActive = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "PlayerCapsule" && zoneIsActive == false)
            {
                // Avisa al VFXManager que entre en zona de resonancia
                VFXManager.Instance.ResonanceZoneEntered(transform);
                zoneIsActive = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "PlayerCapsule" && zoneIsActive == true)
            {
                // Avisa al VFXManager que salga de zona de resonancia
                VFXManager.Instance.ResonanceZoneExited();
                zoneIsActive = false;
            }
        }
    }
}