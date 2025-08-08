namespace TwelveG.EnvironmentController
{
    using UnityEngine;

    public class FrontHouseHandler : MonoBehaviour
    {
        [Header("Objects references")]
        [SerializeField] private GameObject fernandezDeadPrefab;
        [SerializeField] private Transform chair;
        [SerializeField] private Light livingRoomLight;
        [SerializeField] private Light entranceLight;


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
            Instantiate(fernandezDeadPrefab, chair);
        }
    }
}