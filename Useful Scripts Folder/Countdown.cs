using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Util;
using DG.Tweening;

namespace Util.Usefull
{
    public class Countdown : MonoBehaviour
    {
        [Header("Elements")] private AudioSource audioSource;
        public AudioClip scrollClip;

        public Transform[] countObj;
        private int length = -1;

        public bool IsPlay;

        private void Awake()
        {
            if (GetComponent<AudioSource>())
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        private void OnEnable()
        {
            Init();
            DoCountdown();
        }

        private void OnDisable() 
        {
            Function.TouchEnable(true);
        }

        void Init()
        {
            foreach (var v in countObj)
            {
                v.gameObject.SetActive(false);
                v.localScale = Vector3.one;
            }

            IsPlay = true;
            length = countObj.Length;
            audioSource.clip = scrollClip;
        }

        public void DoCountdown()
        {
            StartCoroutine(Action());

            IEnumerator Action()
            {
                Function.TouchEnable(false);
                yield return new WaitForSeconds(.1f);
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                else
                    SoundController.Instance.PlayClip(scrollClip);

                for (int i = 0; i < length-1; i++)
                {
                    countObj[i].gameObject.SetActive(true);
                    countObj[i].DOScale(Vector3.zero, .4f)
                        .SetDelay(.5f)
                        .SetEase(Ease.InQuad);

                    yield return new WaitForSeconds(1f);
                }

                countObj[length - 1].gameObject.SetActive(true);
                countObj[length - 1].localScale = Vector3.zero;
                countObj[length - 1].DOScale(Vector3.one, .3f)
                    .SetEase(Ease.OutBack);

                yield return new WaitForSeconds(1.2f);
                IsPlay = false;
                this.gameObject.SetActive(false);
                Function.TouchEnable(true);
            }
        }



    }
}