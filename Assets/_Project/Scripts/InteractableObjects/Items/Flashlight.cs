namespace TwelveG.InteractableObjects
{
    using UnityEngine;

    public class Flashlight : PlayerItemBase
    {
        [Header("References")]
        [SerializeField] private Light flashlightLight;

        private void OnEnable() {
            
        }

        private void OnDisable() {
            
        }

        public void ToogleFlashlightLight(int state)
        {
            if (flashlightLight != null)
            {
                if(state == 1)
                {
                    flashlightLight.enabled = true;
                }
                else if(state == 0)
                {
                    flashlightLight.enabled = false;
                }
            }
        }
    }
}