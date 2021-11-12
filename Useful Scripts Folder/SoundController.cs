using System.Collections;

using UnityEngine;
using DG.Tweening;

namespace Util.Usefull
{

    public class SoundController : MonoBehaviour
    {
        [Header("Audio Component")]
        public AudioSource BGM;

        [Header("Effect Component")]
        public AudioSource[] Effect;

        [Header("Empty Component")]
        public AudioSource emptyAudio;

        private static SoundController instance;
        public static SoundController Instance
        {
            get { return instance; }
        }


        public void OnEnable()
        {
            instance = this;

#if EXP
            BGM.enabled = false;
#endif
            
        }


        public void SetBgmVolume(float volume)
        {
            BGM.DOFade(volume, .5f)
                .SetEase(Ease.OutFlash);
        }


        public void SetEffectVolume(float volume)
        {
            foreach (var v in Effect)
                v.volume = volume;

            emptyAudio.volume = volume;
        }

        bool isPlaying = false;
        public void PlayClip(AudioClip clip)
        {
            DownBgmVolume();
            emptyAudio.PlayOneShot(clip);
            
            if(isPlaying)
                return;
            
            isPlaying = true;


            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                yield return new WaitUntil(()=> !IsPlaying());
                InitBgmVolume();
                isPlaying = false;
            }
        }

        private void InitBgmVolume()
        {
            DOTween.Pause("Decrease Clip");

            BGM.DOFade(1f, .4f)
                .SetEase(Ease.InOutFlash)
                .SetId("Increase Clip");
        }

        private void DownBgmVolume()
        {
            DOTween.Pause("Increase Clip");

            BGM.DOFade(.1f, .4f)
                .SetEase(Ease.InOutFlash)
                .SetId("Decrease Clip");
        }

        public void PlayClipDelay(AudioClip clip, float delay)
        {
            StartCoroutine(Play());

            IEnumerator Play()
            {
                yield return new WaitForSeconds(delay);

                emptyAudio.PlayOneShot(clip);
            }
        }

        public IEnumerator PlayClip(AudioClip clip, float duration)
        {
            emptyAudio.PlayOneShot(clip);

            float currentDuration = 0f;

            while(currentDuration < duration && emptyAudio.isPlaying)
            {
                currentDuration += 1 * Time.deltaTime;
                yield return null;
            }

            emptyAudio.Stop();
        }

        public bool IsPlaying()
        {
            return emptyAudio.isPlaying;
        }

        public void PauseClip()
        {
            emptyAudio.Stop();
        }
    }
}