namespace TwelveG.Environment
{
    using UnityEngine;

    public class StreetLightHandler : MonoBehaviour
    {
        [SerializeField] private GameObject streetLight;

        public bool canBeToggled = true;

        private Material lampMaterial;

        public void SwitchStreetLight(Component sender, object data)
        {
            if(data != null)
            {
                streetLight.SetActive((bool)data);
            }
        }
    }
}