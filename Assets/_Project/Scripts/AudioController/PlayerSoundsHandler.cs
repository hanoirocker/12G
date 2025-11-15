namespace TwelveG.AudioController
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerSoundsHandler : MonoBehaviour
    {
        [Header("Footsteps Clips")]
        public List<AudioClip> woodFS;
        public List<AudioClip> carpetFS;
        public List<AudioClip> mosaicGarageFS;
        public List<AudioClip> mosaicBathroomFS;

        [Header("Footsteps Settings")]
        public float walkPitch = 0.75f;
        public float runPitchMin = 0.9f;
        public float runPitchMax = 1.1f;
        public float walkCooldown = 0.5f;
        public float runCooldown = 0.3f;

        private float nextStepTime = 0f;
        private bool isRunning = false;

        private enum FSMaterial
        {
            Wood, Carpet, MosaicGarage, MosaicBathroom, Empty
        }

        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.pitch = walkPitch;
        }

        private void Update()
        {
            bool isMoving = Input.GetKey(KeyCode.W) ||
                            Input.GetKey(KeyCode.A) ||
                            Input.GetKey(KeyCode.S) ||
                            Input.GetKey(KeyCode.D);

            if (!isMoving) return;

            isRunning = Input.GetKey(KeyCode.LeftShift);

            // Controla cooldown entre pasos
            if (Time.time >= nextStepTime)
            {
                PlayFootStepsSounds(isRunning);
                nextStepTime = Time.time + (isRunning ? runCooldown : walkCooldown);
            }
        }

        private FSMaterial SurfaceSelect()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);

            if (Physics.Raycast(ray, out hit, 1.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Renderer surfaceRenderer = hit.collider.GetComponentInChildren<Renderer>();

                if (surfaceRenderer && surfaceRenderer.sharedMaterial)
                {
                    var matName = surfaceRenderer.sharedMaterial.name;

                    if (matName.Contains("Wood")) return FSMaterial.Wood;
                    if (matName.Contains("Carpet")) return FSMaterial.Carpet;
                    if (matName.Contains("Mosaic - Garage")) return FSMaterial.MosaicGarage;
                    if (matName.Contains("Mosaic - Bathroom")) return FSMaterial.MosaicBathroom;
                }
            }

            return FSMaterial.Empty;
        }

        private void PlayFootStepsSounds(bool isRunning)
        {
            AudioClip clip = null;
            FSMaterial surface = SurfaceSelect();

            switch (surface)
            {
                case FSMaterial.Wood:
                    clip = woodFS[Random.Range(0, woodFS.Count)];
                    break;
                case FSMaterial.MosaicBathroom:
                    clip = mosaicBathroomFS[Random.Range(0, mosaicBathroomFS.Count)];
                    break;
                case FSMaterial.MosaicGarage:
                    clip = mosaicGarageFS[Random.Range(0, mosaicGarageFS.Count)];
                    break;
                case FSMaterial.Carpet:
                    clip = carpetFS[Random.Range(0, carpetFS.Count)];
                    break;
            }
            if (clip == null) return;

            audioSource.pitch = isRunning
                ? Random.Range(runPitchMin, runPitchMax)
                : walkPitch;

            audioSource.PlayOneShot(clip);
        }
    }
}
