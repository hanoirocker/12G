using System.Collections;
using Cinemachine;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.Utils
{
    public class VirtualCamerasHandler : MonoBehaviour
    {
        public static VirtualCamerasHandler Instance { get; private set; }

        [Header("VC's references")]
        [SerializeField] private CinemachineVirtualCamera playerVC;
        [SerializeField] private CinemachineVirtualCamera wakeUpVC;
        [SerializeField] private CinemachineVirtualCamera kitchenDeskVC;
        [SerializeField] private CinemachineVirtualCamera pCVC;
        [SerializeField] private CinemachineVirtualCamera backpackVC;
        [SerializeField] private CinemachineVirtualCamera phoneVC;
        [SerializeField] private CinemachineVirtualCamera bedVC;
        [SerializeField] private CinemachineVirtualCamera tvVC;
        [SerializeField] private CinemachineVirtualCamera safeBox;
        [SerializeField] private CinemachineVirtualCamera sofaVC;
        [SerializeField] private CinemachineVirtualCamera flashlightVC;
        [SerializeField] private CinemachineVirtualCamera focusVC;

        [Space(10)]
        [Header("Camera Shake Profiles")]
        [SerializeField] private NoiseSettings defaultShakeProfile;
        [SerializeField] private NoiseSettings thunderShakeProfile;
        [Space(5)]
        [SerializeField, Range(0f, 5f)] private float defaultAmplitude = 1f;
        [SerializeField, Range(0f, 5f)] private float defaultFrequency = 0.25f;

        [Space(10)]
        [Header("EventsSO references")]
        public GameEventSO setCurrentCamera;
        public GameEventSO returnCurrentCamera;

        private GameObject playerCameraRoot;
        private Transform playerCapsuleTransform;
        private CinemachineVirtualCamera currentActiveCamera;
        private CinemachineVirtualCamera lastActiveCamera;

        // Variables de Noise
        private float storedAmplitude;
        private float storedFrequency;
        private bool isNoiseSuspended = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            playerCameraRoot = GameObject.FindWithTag("CinemachineTarget");
            playerCapsuleTransform = GameObject.FindGameObjectWithTag("PlayerCapsule")
                .GetComponent<Transform>();

            if (playerCameraRoot == null)
            {
                Debug.LogError("No se encontró el GameObject con el tag 'CinemachineTarget'");
            }

            playerVC.m_Follow = playerCameraRoot.transform;
            playerVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = defaultAmplitude;
            playerVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = defaultFrequency;

            focusVC.m_Follow = playerCameraRoot.transform;

            SetCurrentActiveCamera();
            setCurrentCamera.Raise(this, currentActiveCamera);
        }

        public CinemachineVirtualCamera GetCurrentActiveCamera()
        {
            return currentActiveCamera;
        }

        private void SetCurrentActiveCamera()
        {
            CinemachineVirtualCamera[] listOfVirtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (CinemachineVirtualCamera vc in listOfVirtualCameras)
            {
                if (vc.isActiveAndEnabled)
                {
                    currentActiveCamera = vc;
                }
            }
        }

        // TODO: ver la forma de simplificar el sistema, actualmente si se desactiva o activa una cam != playerVC, 
        // se asume que se debe modificar playerVC. Y si en algun momento queremos pasar de una cámara a otra que no sea playerVC?
        public void VirtualCamerasControls(Component sender, object data)
        {
            if (data is ToggleVirtualCamera cmd)
            {
                CinemachineVirtualCamera targetVC = GetVCByEnum(cmd.Target);

                targetVC.enabled = cmd.Enabled;

                if (cmd.Target != VirtualCameraTarget.Player)
                    playerVC.enabled = !cmd.Enabled;

                currentActiveCamera = cmd.Enabled ? targetVC : playerVC;

                if (cmd.Target == VirtualCameraTarget.Backpack && cmd.Enabled)
                {
                    ReturnVCInstance();
                }

                setCurrentCamera.Raise(this, currentActiveCamera);
            }
            else if (data is ToggleIntoCinematicCameras enable)
            {
                if (enable.Enabled)
                {
                    lastActiveCamera = currentActiveCamera;
                    currentActiveCamera.enabled = false;
                }
                else
                {
                    lastActiveCamera.enabled = true;
                    lastActiveCamera = null;
                }
            }
            else if (data is LookAtTarget lookAtCmd)
            {
                HandleFocusCamera(lookAtCmd.LookAtTransform);
            }
            else
            {
                Debug.LogWarning($"[VirtualCamerasHandler] Comando desconocido recibido: {data}");
            }

            setCurrentCamera.Raise(this, currentActiveCamera);
        }

        private void HandleFocusCamera(Transform target)
        {
            if (target != null)
            {
                focusVC.m_LookAt = target;

                // Para evitar que la cámara viaje desde el origen del mundo,
                // forzamos su posición a la de la cámara actual antes de encenderla.
                focusVC.transform.position = currentActiveCamera.transform.position;
                focusVC.transform.rotation = currentActiveCamera.transform.rotation;

                focusVC.enabled = true;
                focusVC.Priority = 100;
            }
            else
            {
                playerCapsuleTransform.position = focusVC.transform.position;
                Vector3 focusEuler = focusVC.transform.rotation.eulerAngles;
                playerCapsuleTransform.rotation = Quaternion.Euler(0, focusEuler.y, 0);

                playerCameraRoot.transform.localRotation = Quaternion.Euler(focusEuler.x, 0, 0);

                focusVC.Priority = 0;
                focusVC.enabled = false;
                focusVC.m_LookAt = null;

                currentActiveCamera = playerVC;

                FPController fpController = playerCameraRoot.GetComponentInParent<FPController>();

                if (fpController != null)
                {
                    fpController.SyncRotationWithCurrentCamera();
                }
            }
        }

        private void ReturnVCInstance()
        {
            returnCurrentCamera.Raise(this, currentActiveCamera);
        }

        // Perfil de Noise actualmente asignado solo a PlayerVC y FocusVC
        public IEnumerator CameraShakeRoutine(float shakeAmplitude, float shakeFrequency, float shakeDuration)
        {
            CinemachineVirtualCamera vCam = GetCurrentActiveCamera();

            if (vCam != playerVC && vCam != focusVC)
            {
                yield break;
            }

            if (vCam != null)
            {
                CinemachineBasicMultiChannelPerlin noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                if (noise == null)
                {
                    Debug.LogWarning($"[VirtualCamerasHandler] Se agregó componente de ruido a {vCam.name}");
                    noise = vCam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                }

                noise.m_NoiseProfile = thunderShakeProfile;

                noise.m_AmplitudeGain = shakeAmplitude;
                noise.m_FrequencyGain = shakeFrequency;

                float elapsedTime = 0f;
                while (elapsedTime < shakeDuration)
                {
                    float dampingFactor = 1f - (elapsedTime / shakeDuration);
                    noise.m_AmplitudeGain = Mathf.Lerp(0f, shakeAmplitude, dampingFactor);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;

                // Si la cámara sobre la que se aplicó el shake es la cámara del jugador, restauramos el perfil por defecto
                if (vCam == playerVC)
                {
                    noise.m_NoiseProfile = defaultShakeProfile;
                    noise.m_AmplitudeGain = defaultAmplitude;
                    noise.m_FrequencyGain = defaultFrequency;
                }
            }
        }

        private CinemachineVirtualCamera GetVCByEnum(VirtualCameraTarget target)
        {
            return target switch
            {
                VirtualCameraTarget.Player => playerVC,
                VirtualCameraTarget.WakeUp => wakeUpVC,
                VirtualCameraTarget.KitchenDesk => kitchenDeskVC,
                VirtualCameraTarget.Bed => bedVC,
                VirtualCameraTarget.PC => pCVC,
                VirtualCameraTarget.Backpack => backpackVC,
                VirtualCameraTarget.Phone => phoneVC,
                VirtualCameraTarget.TV => tvVC,
                VirtualCameraTarget.Sofa => sofaVC,
                VirtualCameraTarget.SafeBox => safeBox,
                VirtualCameraTarget.Flashlight => flashlightVC,
                VirtualCameraTarget.Focus => focusVC,
                _ => null
            };
        }

        public void ToggleActiveCameraNoise(bool enableNoise)
        {
            CinemachineVirtualCamera vCam = GetCurrentActiveCamera();
            if (vCam == null) return;

            var noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (noise == null) return;

            if (!enableNoise)
            {
                if (!isNoiseSuspended)
                {
                    storedAmplitude = noise.m_AmplitudeGain;
                    storedFrequency = noise.m_FrequencyGain;

                    noise.m_AmplitudeGain = 0f;
                    noise.m_FrequencyGain = 0f;

                    isNoiseSuspended = true;
                }
            }
            else
            {
                if (isNoiseSuspended)
                {
                    // Restauramos los valores guardados
                    noise.m_AmplitudeGain = storedAmplitude;
                    noise.m_FrequencyGain = storedFrequency;

                    isNoiseSuspended = false;
                }
                else
                {
                    if (vCam == playerVC)
                    {
                        noise.m_AmplitudeGain = defaultAmplitude;
                        noise.m_FrequencyGain = defaultFrequency;
                    }
                }
            }
        }
    }
}