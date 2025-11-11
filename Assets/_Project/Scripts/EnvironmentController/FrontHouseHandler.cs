namespace TwelveG.EnvironmentController
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Video;

    public class FrontHouseHandler : MonoBehaviour
    {
        [Header("Fernandez Event References")]
        [SerializeField] private GameObject fernandezDeadPrefab;
        [SerializeField] private Transform chairNoBlood;
        [SerializeField] private Transform chairWithBlood;
        [SerializeField] private Light entranceLight;

        [SerializeField, Range(10f, 1000f)] private float timeBeforeTVTurnedOff = 180f;
        [SerializeField] private GameObject tvObject;


        public void SwitchEntranceLight(bool turnOnLight)
        {
            entranceLight.enabled = turnOnLight;
        }

        public void TriggerFernandezSuicide()
        {
            chairNoBlood.gameObject.SetActive(false);
            chairWithBlood.gameObject.SetActive(true);
            Instantiate(fernandezDeadPrefab, chairWithBlood);
            StartCoroutine(TurnOffTVAfterDelay(timeBeforeTVTurnedOff));
        }

        private IEnumerator TurnOffTVAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            tvObject.GetComponentInChildren<VideoPlayer>().gameObject.SetActive(false);
            tvObject.GetComponentInChildren<Light>().gameObject.SetActive(false);
        }
    }
}