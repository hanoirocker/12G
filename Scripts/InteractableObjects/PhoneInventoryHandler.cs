namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
    using UnityEngine;

    public class PhoneInventoryHandler : MonoBehaviour
    {
        [Header("Screens references")]
        [SerializeField] private GameObject[] phoneScreens_es;
        [SerializeField] private GameObject[] phoneScreens_en;

        [Header("Sound settings")]
        [SerializeField] private AudioClip messageSendClip;

        [Header("Interaction Texts SO's")]
        [SerializeField] private List<ObservationTextSO> observationTextSOs = new List<ObservationTextSO>();

        [Header("EventSOs settings")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;

        [Header("Specific EventSOs settings")]
        [SerializeField] private GameEventSO finishedUsingPhone;

        private GameObject[] currentPhoneScreenList = null;
        private LocalizationManager localizationManager;
        private int screenIndex = 0;
        private string currentLanguage = null;

        private void Awake()
        {
            localizationManager = LocalizationManager.Instance;
        }

        void Start()
        {
            currentLanguage = localizationManager.GetCurrentLanguageCode();
            SetCurrentPhoneScreenList();
            GetComponent<Animation>().PlayQueued("Phone Inventory - Show Phone");
            StartCoroutine(PhoneInventoryCoroutine());
        }

        private void SetCurrentPhoneScreenList()
        {
            if (currentLanguage == "es")
            {
                currentPhoneScreenList = phoneScreens_es;
            }
            else if (currentLanguage == "en")
            {
                currentPhoneScreenList = phoneScreens_en;
            }
        }

        public void UpdatePhoneScreenListOnLanguageChanged()
        {
            string updatedLanguage = localizationManager.GetCurrentLanguageCode();

            if (currentLanguage == updatedLanguage) { return; }

            currentLanguage = updatedLanguage;
            currentPhoneScreenList[screenIndex - 1].SetActive(false);
            SetCurrentPhoneScreenList();
            currentPhoneScreenList[screenIndex - 1].SetActive(true);
        }

        private IEnumerator PhoneInventoryCoroutine()
        {
            // Aca cambia a la pantalla de apps
            ChangePhoneScreen();
            yield return new WaitForSeconds(3f);

            // Mejor le escribo a Mica sobre todo esto ..
            onObservationCanvasShowText.Raise(this, observationTextSOs[0]);
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
            // ............................................;
            onObservationCanvasShowText.Raise(this, observationTextSOs[1]);

            // Aca cambia a la pantalla de Mica 3 (envia 2do mensaje)
            yield return new WaitForSeconds(4f);
            ChangePhoneScreen();
            GetComponent<AudioSource>().PlayOneShot(messageSendClip);

            yield return new WaitForSeconds(7f);
            // ME CAGO EN TODO. Voy directo a su casa.
            onObservationCanvasShowText.Raise(this, observationTextSOs[2]);

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
                currentPhoneScreenList[0].SetActive(true);
            }
            else
            {
                currentPhoneScreenList[screenIndex - 1].SetActive(false);
                currentPhoneScreenList[screenIndex].SetActive(true);
            }

            screenIndex += 1;
        }
    }
}