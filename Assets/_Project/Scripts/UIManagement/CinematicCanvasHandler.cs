namespace TwelveG.UIManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CinematicCanvasHandler : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Animator containerAnimator;

        private void ShowBars()
        {
            container.SetActive(true);
        }

        private IEnumerator HideBars()
        {
            if (container.activeSelf)
            {
                containerAnimator.SetTrigger("HideBars");
                yield return new WaitForSeconds(2f);
                container.SetActive(false);
            }
        }

        public void CinematicCanvasControls(Component sender, object data)
        {
            if((string)data == "ShowBars")
            {
                ShowBars();
            }
            else if((string)data == "HideBars")
            {
                StartCoroutine(HideBars());
            }
        }
    }

}