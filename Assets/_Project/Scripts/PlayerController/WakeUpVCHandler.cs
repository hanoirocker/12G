namespace TwelveG.Utils
{
    using System.Collections;
    using Cinemachine;
    using UnityEngine;

    public class WakeUpVCHandler : MonoBehaviour
    {
        [Header("Game Event SO's")]
        public GameEventSO animationHasEnded;

        private Animation _animation;

        private void OnDisable()
        {
            GetComponent<CinemachineVirtualCamera>().LookAt = null;
        }

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