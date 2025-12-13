using System;
using UnityEngine;

namespace TwelveG.VFXController
{
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PostProcessingHandler postProcessingHandler;

        [SerializeField] private float pulseSpeed = 4.0f;

        private float targetHeadacheIntensity = 0f;

        private Transform playerTransform;
        private Transform currentResonanceZone = null;

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
            if (currentResonanceZone == null) { return; }
            DynamicHeadacheLogic();
        }

        private void DynamicHeadacheLogic()
        {
            Debug.Log("Player position: " + playerTransform.position);
        }

        public void ResonanceZoneEntered(Transform senderTransform)
        {
            Debug.Log("Player entered resonance zone.");
            Debug.Log($"Resonant object position is {senderTransform.position}");
            currentResonanceZone = senderTransform;
        }

        public void ResonanceZoneExited()
        {
            Debug.Log("Player exited resonance zone.");
            currentResonanceZone = null;
        }

        // Llamada publica para ajustar la intensidad del dolor de cabeza (0-1)
        public void SetHeadacheIntensity(float intensity)
        {
            targetHeadacheIntensity = Mathf.Clamp01(intensity);
        }

        /// Útil para triggers de "zona". Llama a esto con (1 - distancia / radio).
        public void SetResonanceFromDistance(float normalizedDistance)
        {
            SetHeadacheIntensity(normalizedDistance);
        }

        public void RegisterPlayer(Transform pTransform)
        {
            playerTransform = pTransform;
            Debug.Log("VFXManager: Player registered for VFX effects.");
        }
    }
}