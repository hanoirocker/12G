namespace TwelveG.EnvironmentController
{
    using UnityEngine;

    public class FrontHouseHandler : MonoBehaviour
    {
        [Header("Fernandez Event References")]
        [SerializeField] private GameObject fernandezDeadPrefab;
        [SerializeField] private Transform chairNoBlood;
        [SerializeField] private Transform chairWithBlood;
        [SerializeField] private Light entranceLight;


        public void SwitchEntranceLight(bool turnOnLight)
        {
            entranceLight.enabled = turnOnLight;
        }

        public void TriggerFernandezSuicide()
        {
            chairNoBlood.gameObject.SetActive(false);
            chairWithBlood.gameObject.SetActive(true);
            Instantiate(fernandezDeadPrefab, chairWithBlood);
        }
    }
}