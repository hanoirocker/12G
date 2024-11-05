namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Flashlight : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnDisable()
        {
            animator.SetTrigger("HideItem");
        }
    }
}