using System.Collections;
using Cinemachine;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.Utils
{
    public class VCAnimationHandler : MonoBehaviour
    {
        private Animation _animation;

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        private void OnDisable()
        {
            var vCam = GetComponent<CinemachineVirtualCamera>();
            if (vCam != null) vCam.LookAt = null;
        }

        public void PlayAnimation()
        {
            if (_animation == null) _animation = GetComponent<Animation>();

            if (_animation.clip == null && _animation.GetClipCount() == 0)
            {
                Debug.LogWarning("[VCAnimationHandler] No hay clip de animación asignado.");
                GameEvents.Common.onAnimationHasEnded.Raise(this, null);
                return;
            }

            StartCoroutine(AnimationCoroutine());
        }

        private IEnumerator AnimationCoroutine()
        {
            _animation.Play();

            yield return null;

            while (_animation.isPlaying)
            {
                yield return null;
            }

            GameEvents.Common.onAnimationHasEnded.Raise(this, null);
        }
    }
}