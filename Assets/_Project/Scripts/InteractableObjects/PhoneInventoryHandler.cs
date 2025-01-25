namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using UnityEngine;

    public class PhoneInventoryHandler : MonoBehaviour
    {
        [Header("Screens references")]
        [SerializeField] private GameObject[] phoneScreens;

        [Header("Sound settings")]
        [SerializeField] private AudioClip messageSendClip;

        [Header("EventSOs settings")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;

        [Header("Specific EventSOs settings")]
        [SerializeField] private GameEventSO finishedUsingPhone;

        private int screenIndex = 0;

        void Start()
        {
            GetComponent<Animation>().PlayQueued("Phone Inventory - Show Phone");
            StartCoroutine(PhoneInventoryCoroutine());
        }

        private IEnumerator PhoneInventoryCoroutine()
        {
            // Aca cambia a la pantalla de apps
            ChangePhoneScreen();
            yield return new WaitForSeconds(3f);
            onObservationCanvasShowText.Raise(this, "MANDAR MENSAJE A MICA: LOCALIZATION!");
            yield return new WaitForSeconds(2f);
            // Aca cambia a la pantalla de contactos
            ChangePhoneScreen();
            yield return new WaitForSeconds(1.5f);
            // Aca cambia a la pantalla de Mica 1
            ChangePhoneScreen();
            yield return new WaitForSeconds(3f);
            // Aca cambia a la pantalla de Mica 2 (envia 1er mensaje)
            ChangePhoneScreen();
            GetComponent<AudioSource>().PlayOneShot(messageSendClip);
            yield return new WaitForSeconds(8f);
            onObservationCanvasShowText.Raise(this, "...........: LOCALIZATION!");
            // Aca cambia a la pantalla de Mica 3 (envia 2do mensaje)
            yield return new WaitForSeconds(4f);
            ChangePhoneScreen();
            GetComponent<AudioSource>().PlayOneShot(messageSendClip);
            yield return new WaitForSeconds(7f);
            onObservationCanvasShowText.Raise(this, "FUCK THIS SHIT: LOCALIZATION!");

            GetComponent<Animation>().PlayQueued("Phone Inventory - Hide Phone");

            finishedUsingPhone.Raise(this, null);

            yield return new WaitForSeconds(5f);
            // Destruye el Inventory - Phone pero no lo elimina de la lista de objetos
            // adquiridos en el inventario. Esto es por si mas adelante se llega a precisar
            // posee el telefono para algun puzzle.
            Destroy(gameObject);
        }

        private void ChangePhoneScreen()
        {
            print("screenIndex: " + screenIndex);

            if (screenIndex == 0)
            {
                phoneScreens[0].SetActive(true);
            }
            else
            {
                phoneScreens[screenIndex - 1].SetActive(false);
                phoneScreens[screenIndex].SetActive(true);
            }

            screenIndex += 1;
        }
    }
}