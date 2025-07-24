namespace TwelveG.EnvironmentController
{
    using UnityEngine;

    public class WindowToReplaceHandler : MonoBehaviour
    {
        [SerializeField] private GameObject zoomBird;

        public void InstatiateZoomBird()
        {
            GameObject zoomBirdInstance = Instantiate(zoomBird);
        }
    }
}