namespace TwelveG.Environment
{
    using UnityEngine;

    public class FrontHouseHandler : MonoBehaviour
    {
        [Header("Objects references")]
        [SerializeField] private GameObject fernandezDeadPrefab;
        [SerializeField] private Light livingRoomLight;
        [SerializeField] private Light entranceLight;

        [Header("Audio references")]
        [SerializeField] private AudioSource shotGunAudioSource;


        public void SwitchLivingRoomLight(bool turnOnLight)
        {
            livingRoomLight.enabled = turnOnLight;
        }

        public void SwitchEntranceLight(bool turnOnLight)
        {
            entranceLight.enabled = turnOnLight;
        }

        public void TriggerFernandezSuicide()
        {
            fernandezDeadPrefab.SetActive(true);
            shotGunAudioSource.Play();
        }
    }
}