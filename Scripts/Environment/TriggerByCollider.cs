namespace TwelveG.Environment
{
    using UnityEngine;

    public class TriggerByCollider : MonoBehaviour
    {
        [SerializeField] private GameObject gameObjectToSetActive = null;
        [SerializeField] private GameObject gameObjectToInstatiate = null;
        [SerializeField] private bool disableColliderAfterTrigger = false;
        [SerializeField] private Collider _collider = null;

        public GameEventSO seeZoomBird;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if(disableColliderAfterTrigger)
                {
                    _collider.enabled = false;
                }
                if (gameObjectToSetActive != null)
                {
                    gameObjectToSetActive.SetActive(true);
                }
                else if (gameObjectToInstatiate != null)
                {
                    Instantiate(gameObjectToInstatiate);
                }

                seeZoomBird.Raise(this, null);
            }
        }
    }

}