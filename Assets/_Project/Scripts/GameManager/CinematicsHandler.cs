namespace TwelveG.GameManager
{
    using UnityEngine;
    using UnityEngine.Playables;

    public class CinematicsHandler : MonoBehaviour
    {
        [SerializeField] private PlayableDirector timeline1Director;
        [SerializeField] private PlayableDirector timeline2Director;

        public void PlayerDirectorsControls(Component sender, object data)
        {
            if(data != null)
            {
                if((string)data == "EnableTimeline1Director")
                {
                    timeline1Director.enabled = true;
                }
                else if((string)data == "EnableTimeline2Director")
                {
                    timeline2Director.enabled = true;
                }
            }
        }
    }
}