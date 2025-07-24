namespace TwelveG.EnvironmentController
{
    using UnityEngine;

    public class StairsWindowHandler : MonoBehaviour
    {
        [SerializeField] private GameObject bloodStain;

        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<CrashingBird>() != null)
            {
                bloodStain.SetActive(true);
            }
        }
    }
}