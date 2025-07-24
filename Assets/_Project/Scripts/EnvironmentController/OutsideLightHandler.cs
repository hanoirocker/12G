namespace TwelveG.EnvironmentController
{
    using UnityEngine;

    public class OutsideLightHandler : MonoBehaviour
    {
        [SerializeField] private Light lampHeadLight;
        [SerializeField] private Renderer lampHeadRenderer;
        public bool canBeToggled = true;

        private Material lampMaterial;

        private void Start()
        {
            if (lampHeadLight)
            {
                lampMaterial = lampHeadRenderer.material;
            }
        }

        public void SwitchSmallLight(Component sender, object data)
        {
            if (data != null && canBeToggled && (bool)data == true)
            {
                lampHeadLight.enabled = true;
                lampMaterial.EnableKeyword("_EMISSION");
            }
            else if (data != null && canBeToggled && (bool)data == false)
            {
                lampHeadLight.enabled = false;
                lampMaterial.DisableKeyword("_EMISSION");
            }
            else
            {
                return;
            }
        }
    }
}