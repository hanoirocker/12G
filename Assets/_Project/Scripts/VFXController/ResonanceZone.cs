using UnityEngine;

namespace TwelveG.VFXController
{
    [RequireComponent(typeof(SphereCollider))]
    public class ResonanceZone : MonoBehaviour
    {
        private bool zoneIsActive = false;
        private SphereCollider sphereCol;

        private void Awake()
        {
            sphereCol = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "PlayerCapsule" && zoneIsActive == false)
            {
                // Avisa al VFXManager que entre en zona de resonancia
                float realRadius = sphereCol.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
                VFXManager.Instance.ResonanceZoneEntered(transform, realRadius);
                zoneIsActive = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "PlayerCapsule" && zoneIsActive == true)
            {
                // Debug.Log("Resonance Zone exited");

                // Avisa al VFXManager que salga de zona de resonancia
                VFXManager.Instance?.ResonanceZoneExited();
                zoneIsActive = false;
            }
        }

        void OnDisable()
        {
            // Debug.Log("Resonance Zone exited");

            // Avisa al VFXManager que salga de zona de resonancia
            VFXManager.Instance?.ResonanceZoneExited();
            zoneIsActive = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (sphereCol == null) sphereCol = GetComponent<SphereCollider>();
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            float realRadius = sphereCol.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            Gizmos.DrawSphere(transform.position + sphereCol.center, realRadius);
        }
    }
}