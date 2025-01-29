namespace TwelveG.Environment
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CarAnimationSelector : MonoBehaviour
    {
        AudioSource audioSource;
        Animator animator;
        int index;

        void Start()
        {
            index = Random.Range(0, 2);
            animator = GetComponent<Animator>();
            SetCarAnimation(index);
        }

        private void SetCarAnimation(int index)
        {
            animator.SetInteger("index", index);
            animator.SetTrigger("drive");
        }
    }

}