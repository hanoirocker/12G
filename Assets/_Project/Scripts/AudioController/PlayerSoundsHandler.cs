namespace TwelveG.AudioController
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerSoundsHandler : MonoBehaviour
    {
        [Header("Footsteps")]
        public List<AudioClip> woodFS;
        public List<AudioClip> carpetFS;
        public List<AudioClip> mosaicGarageFS;
        public List<AudioClip> mosaicBathroomFS;
        bool isRunning = false;

        enum FSMaterial
        {
            Wood, Carpet, MosaicGarage, MosaicBathroom, Empty
        }

        AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.S))
            {
                isRunning = false;
                if (Input.GetKey(KeyCode.LeftShift
                ))
                {
                    isRunning = true;
                }
                PlayFootStepsSounds(isRunning);
            }
        }

        private FSMaterial SurfaceSelect()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);
            Material surfaceMaterial;

            if (Physics.Raycast(ray, out hit, 1.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                Renderer surfaceRenderer = hit.collider.GetComponentInChildren<Renderer>();
                if (surfaceRenderer)
                {
                    surfaceMaterial = surfaceRenderer ? surfaceRenderer.sharedMaterial : null;
                    if (surfaceMaterial.name.Contains("Wood"))
                    {
                        return FSMaterial.Wood;
                    }
                    else if (surfaceMaterial.name.Contains("Carpet"))
                    {
                        return FSMaterial.Carpet;
                    }
                    else if (surfaceMaterial.name.Contains("Mosaic - Garage"))
                    {
                        return FSMaterial.MosaicGarage;
                    }
                    else if (surfaceMaterial.name.Contains("Mosaic - Bathroom"))
                    {
                        return FSMaterial.MosaicBathroom;
                    }
                    else
                    {
                        return FSMaterial.Empty;
                    }
                }
            }
            return FSMaterial.Empty;
        }

        private void PlayFootStepsSounds(bool isRunning)
        {
            if (audioSource.isPlaying) { return; }

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
                default:
                    break;
            }

            if (surface != FSMaterial.Empty)
            {
                audioSource.clip = clip;
                if (isRunning)
                {
                    audioSource.pitch = Random.Range(1.6f, 1.7f);
                }
                else
                {
                    audioSource.pitch = Random.Range(0.7f, 0.8f);
                }
                audioSource.Play();
            }
        }
    }

}