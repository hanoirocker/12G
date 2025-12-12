using System.Collections;
using Cinemachine;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.Utils
{
    public class WakeUpVCHandler : MonoBehaviour
    {
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
            GameEvents.Common.onAnimationHasEnded.Raise(this, null);
        }
    }
}