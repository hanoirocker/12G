namespace TwelveG.UIManagement
{
    using TMPro;
    using UnityEngine;

    public class ContemplationCanvasHandler : MonoBehaviour
    {
        private Canvas contemplationCanvas;
        private TextMeshProUGUI contemplationCanvasText;

        private void Awake()
        {
            contemplationCanvas = GetComponent<Canvas>();
            contemplationCanvasText = GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            contemplationCanvas.enabled = false;
        }

        public void ShowContemplationText(Component sender, object data)
        {
            if ((string)data != null)
            {
                contemplationCanvas.enabled = true;
                contemplationCanvasText.text = (string)data;
            }
        }

        private void HideContemplationCanvas()
        {
            contemplationCanvas.enabled = false;
        }

        public bool IsShowingText()
        {
            return contemplationCanvas.isActiveAndEnabled;
        }

        public void ContemplationCanvasControls(Component sender, object data)
        {
            if ((string)data == "HideContemplationCanvas")
            {
                HideContemplationCanvas();
            }
        }
    }

}