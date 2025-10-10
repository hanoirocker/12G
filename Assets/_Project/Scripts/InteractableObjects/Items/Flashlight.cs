namespace TwelveG.InteractableObjects
{
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