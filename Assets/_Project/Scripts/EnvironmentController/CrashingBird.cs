namespace TwelveG.EnvironmentController
{
    using UnityEngine;

    public class CrashingBird : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 1000f;
        [SerializeField] AudioSource audioSource;

        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            AddForce();
        }

        private void AddForce()
        {
            rb.AddForce(Vector3.right * moveSpeed);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Window")
            {
                audioSource.Play();
            }
        }
    }
}