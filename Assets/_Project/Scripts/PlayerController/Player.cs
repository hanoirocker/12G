namespace TwelveG.PlayerController
{
    using UnityEngine;

    public class Player : MonoBehaviour
    {
        private string currentLanguage;

        public string CurrentLanguage
        {
            get { return currentLanguage; }
            private set { currentLanguage = value; }
        }

        public void SetCurrentLanguage(Component sender, object data)
        {
            print("Recibiendo en Player, desde "+ sender.gameObject.name + ", lang code: " + (string)data);
            CurrentLanguage = (string)data;
        }
    }
}