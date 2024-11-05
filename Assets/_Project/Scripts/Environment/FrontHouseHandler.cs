namespace TwelveG.Environment
{
    using UnityEngine;

    public class FrontHouseHandler : MonoBehaviour
    {
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
    }
}