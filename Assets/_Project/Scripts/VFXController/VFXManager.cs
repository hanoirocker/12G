using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwelveG.VFXController
{
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PostProcessingHandler postProcessingHandler;

        [Header("Settings")]
        [SerializeField] private float effectSmoothSpeed = 5f;
        [SerializeField] private float maxEffectDistance = 5f;

        // Multiplicador narrativo (modificable por eventos)
        [SerializeField] private float resonanceIntensityMultiplier = 1.0f; 
        private List<Transform> activeResonanceZones = new List<Transform>();
        private float currentAppliedIntensity = 0f;

        private Transform playerTransform;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            if (postProcessingHandler == null)
            {
                Debug.LogError("VFXManager: PostProcessingHandler reference is missing!");
                this.enabled = false;
            }

            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (playerTransform == null)
            {
                Debug.LogError("VFXManager: Player object with tag 'Player' not found in the scene!");
                this.enabled = false;
            }
        }

        void Update()
        {
            if (activeResonanceZones.Count > 0 || currentAppliedIntensity > 0.01f)
            {
                CalculateAndApplyHeadache();
            }
        }

        // TODO: No me cierra del todo el mini zoom que se genera al activar el efecto, revisar stack Headache
        private void CalculateAndApplyHeadache()
        {
            float targetIntensity = 0f;

            if (activeResonanceZones.Count > 0 && playerTransform != null)
            {
                float closestDistanceSqr = float.MaxValue;

                // Objeto más cercano de la lista
                foreach (Transform zone in activeResonanceZones)
                {
                    if (zone == null) continue; // Protección por si se destruye el objeto

                    float distSqr = (zone.position - playerTransform.position).sqrMagnitude;
                    if (distSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distSqr;
                    }
                }

                // Convertir a distancia real
                float closestDistance = Mathf.Sqrt(closestDistanceSqr);

                // Inversa a la distancia
                float rawIntensity = Mathf.InverseLerp(maxEffectDistance, 0f, closestDistance);

                targetIntensity = rawIntensity * resonanceIntensityMultiplier;
            }   

            // Interpolación suave para que no salte de golpe
            currentAppliedIntensity = Mathf.Lerp(currentAppliedIntensity, targetIntensity, Time.deltaTime * effectSmoothSpeed);

            if (postProcessingHandler != null)
            {
                postProcessingHandler.SetHeadacheWeight(currentAppliedIntensity);

                // TODO: El resto ( Sonido, Canvas, etc)
            }
        }

        public void ResonanceZoneEntered(Transform senderTransform)
        {
            Debug.Log("Player entered resonance zone.");
            if (!activeResonanceZones.Contains(senderTransform))
            {
                activeResonanceZones.Add(senderTransform);
            }
        }

        public void ResonanceZoneExited(Transform senderTransform)
        {   
            Debug.Log("Player exited resonance zone.");
            if (activeResonanceZones.Contains(senderTransform))
            {
                activeResonanceZones.Remove(senderTransform);
            }
        }

        // Método a llamar desde eventos corrutina o cualquier otro script
        public void SetResonanceIntensityMultiplier(float newMultiplier)
        {
            resonanceIntensityMultiplier = newMultiplier;
        }

        public void RegisterPlayer(Transform pTransform)
        {
            playerTransform = pTransform;
            Debug.Log("VFXManager: Player registered for VFX effects.");
        }
    }
}