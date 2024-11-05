namespace TwelveG.Utils
{
    using System.Collections;
    using UnityEngine;

    public class WakeUpVCHandler : MonoBehaviour
    {
        private Animation _animation;

        public GameEventSO animationHasEnded;

        public void PlayAnimation()
        {
            _animation = GetComponent<Animation>();

            StartCoroutine(AnimationCoroutine());
        }

        private IEnumerator AnimationCoroutine()
        {
            _animation.Play();
            yield return new WaitUntil(() => !_animation.isPlaying);
            animationHasEnded.Raise(this, null);
        }
    }
}