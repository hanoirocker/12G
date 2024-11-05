namespace TwelveG.UIManagement
{
    using TMPro;
    using UnityEngine;

    public class ControlCanvasHandler : MonoBehaviour
    {
        private Canvas controlCanvas;
        [SerializeField] private TextMeshProUGUI options;
        private string defaultOptionText = "";

        void Awake()
        {
            controlCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            HideControlCanvas();
        }

        private void ToogleControlCanvas()
        {
            controlCanvas.enabled = !controlCanvas.enabled;
        }

        private void ShowControlCanvas()
        {
            controlCanvas.enabled = true;
        }

        private void HideControlCanvas()
        {
            controlCanvas.enabled = false;
        }

        private void DeactivateControlCanvas()
        {
            controlCanvas.gameObject.SetActive(false);
        }

        private void ActivateControlCanvas()
        {
            controlCanvas.gameObject.SetActive(true);
        }

        public void SetInteractionOptions(Component sender, object data)
        {
            if ((string)data != null)
            {
                defaultOptionText = (string)data;
                options.text = defaultOptionText;
            }
        }

        public void SetInteractingOptions(Component sender, object data)
        {
            if ((string)data != null)
            {
                defaultOptionText = (string)data;
                options.text = defaultOptionText;
            }
        }

        private void ResetEventControlsOptions()
        {
            print("Recibido!");
            options.text = defaultOptionText;
            print("options.text ahora es: !" + options.text);
        }

        public void ControlCanvasControls(Component sender, object data)
        {
            if ((string)data == "DeactivateControlCanvas")
            {
                DeactivateControlCanvas();
            }
            else if ((string)data == "ActivateControlCanvas")
            {
                ActivateControlCanvas();
            }
            else if ((string)data == "ShowControlCanvas")
            {
                ShowControlCanvas();
            }
            else if ((string)data == "ToogleControlCanvas")
            {
                ToogleControlCanvas();
            }
            else if ((string)data == "ResetEventControlsOptions")
            {
                ResetEventControlsOptions();
            }
            else if ((string)data == "HideControlCanvas")
            {
                HideControlCanvas();
            }
        }
    }
}