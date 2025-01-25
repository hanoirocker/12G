namespace TwelveG.Environment
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PortraitsChanger : MonoBehaviour
    {
        [SerializeField] GameObject normalSprite;
        [SerializeField] GameObject alternateSprite;

        void Start()
        {
            
        }

        public void SwitchToAlternate()
        {
            if (normalSprite != null && alternateSprite != null)
            {
                normalSprite.SetActive(false);
                alternateSprite.SetActive(true);
            }
        }

        public void SwitchToNormal()
        {
            if (normalSprite != null && normalSprite != null)
            {
                normalSprite.SetActive(true);
                alternateSprite.SetActive(false);
            }
        }
    }

}